﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;
using VRCOSC.Game.Config;
using VRCOSC.Game.Graphics.Notifications;
using VRCOSC.OSC.VRChat;

namespace VRCOSC.Game.Modules;

public partial class GameManager : CompositeComponent
{
    private const double vrchat_process_check_interval = 5000;
    private const double openvr_interface_init_delay = 50;
    private const int startstop_delay = 250;

    [Resolved]
    private VRCOSCConfigManager configManager { get; set; } = null!;

    [Resolved]
    private NotificationContainer notifications { get; set; } = null!;

    private Bindable<bool> autoStartStop = null!;
    private CancellationTokenSource? startTokenSource;
    private bool hasAutoStarted;

    public readonly VRChatOscClient OscClient = new();
    public readonly ModuleManager ModuleManager = new();
    public readonly Bindable<GameManagerState> State = new(GameManagerState.Stopped);
    public Player Player = null!;
    public OpenVRInterface OpenVRInterface = null!;
    public ChatBoxInterface ChatBoxInterface = null!;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        autoStartStop = configManager.GetBindable<bool>(VRCOSCSetting.AutoStartStop);

        Player = new Player(OscClient);
        OpenVRInterface = new OpenVRInterface(storage);
        ChatBoxInterface = new ChatBoxInterface(OscClient, configManager.GetBindable<int>(VRCOSCSetting.ChatBoxTimeSpan));

        LoadComponent(ModuleManager);
        AddInternal(ModuleManager);
    }

    protected override void Update()
    {
        OpenVRInterface.Update();
        ChatBoxInterface.Update();
    }

    protected override void LoadComplete()
    {
        Scheduler.AddDelayed(() => Task.Run(() => OpenVRInterface.Init()), openvr_interface_init_delay, true);
        Scheduler.AddDelayed(checkForVRChat, vrchat_process_check_interval, true);

        State.BindValueChanged(e => Logger.Log($"{nameof(GameManager)} state changed to {e.NewValue}"));

        // We reset hasAutoStarted here so that turning auto start off and on again will cause it to work normally
        autoStartStop.BindValueChanged(e =>
        {
            if (!e.NewValue) hasAutoStarted = false;
        });
    }

    public void Start() => Schedule(() => _ = startAsync());

    private async Task startAsync()
    {
        if (State.Value is not (GameManagerState.Stopping or GameManagerState.Stopped))
            throw new InvalidOperationException($"Cannot start {nameof(GameManager)} when state is {State.Value}");

        try
        {
            startTokenSource = new CancellationTokenSource();

            if (!initialiseOscClient()) return;

            State.Value = GameManagerState.Starting;

            await Task.Delay(startstop_delay, startTokenSource.Token);

            OscClient.OnParameterReceived += onParameterReceived;
            Player.Initialise();
            ChatBoxInterface.Initialise();
            sendControlValues();
            ModuleManager.Start();

            State.Value = GameManagerState.Started;
        }
        catch (TaskCanceledException) { }
    }

    public void Stop() => Schedule(() => _ = stopAsync());

    private async Task stopAsync()
    {
        if (State.Value is not (GameManagerState.Starting or GameManagerState.Started))
            throw new InvalidOperationException($"Cannot stop {nameof(GameManager)} when state is {State.Value}");

        startTokenSource?.Cancel();
        startTokenSource = null;

        State.Value = GameManagerState.Stopping;

        await OscClient.DisableReceive();
        ModuleManager.Stop();
        ChatBoxInterface.Shutdown();
        Player.ResetAll();
        OscClient.OnParameterReceived -= onParameterReceived;
        OscClient.DisableSend();

        await Task.Delay(startstop_delay);

        State.Value = GameManagerState.Stopped;
    }

    private bool initialiseOscClient()
    {
        try
        {
            var ipAddress = configManager.Get<string>(VRCOSCSetting.IPAddress);
            var sendPort = configManager.Get<int>(VRCOSCSetting.SendPort);
            var receivePort = configManager.Get<int>(VRCOSCSetting.ReceivePort);

            OscClient.Initialise(ipAddress, sendPort, receivePort);
            OscClient.Enable();
            return true;
        }
        catch (SocketException)
        {
            notifications.Notify(new InvalidOSCAttributeNotification("IP address"));
            return false;
        }
        catch (FormatException)
        {
            notifications.Notify(new InvalidOSCAttributeNotification("IP address"));
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            notifications.Notify(new InvalidOSCAttributeNotification("port"));
            return false;
        }
    }

    private void checkForVRChat()
    {
        if (!configManager.Get<bool>(VRCOSCSetting.AutoStartStop)) return;

        static bool isVRChatOpen() => Process.GetProcessesByName("vrchat").Any();

        // hasAutoStarted is checked here to ensure that modules aren't started immediately
        // after a user has manually stopped the modules
        if (isVRChatOpen() && State.Value == GameManagerState.Stopped && !hasAutoStarted)
        {
            Start();
            hasAutoStarted = true;
        }

        if (!isVRChatOpen() && State.Value == GameManagerState.Started)
        {
            Stop();
            hasAutoStarted = false;
        }
    }

    private void sendControlValues()
    {
        OscClient.SendValue($"{VRChatOscConstants.ADDRESS_AVATAR_PARAMETERS_PREFIX}/VRCOSC/Controls/ChatBox", ChatBoxInterface.SendEnabled);
    }

    private void onParameterReceived(VRChatOscData data)
    {
        if (data.IsAvatarChangeEvent)
        {
            sendControlValues();
            return;
        }

        if (!data.IsAvatarParameter) return;

        Player.Update(data.ParameterName, data.Values[0]);

        switch (data.ParameterName)
        {
            case "VRCOSC/Controls/ChatBox":
                ChatBoxInterface.SendEnabled = (bool)data.Values[0];
                break;
        }
    }
}

public enum GameManagerState
{
    Starting,
    Started,
    Stopping,
    Stopped
}
