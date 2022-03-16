﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Net.Sockets;
using CoreOSC;
using CoreOSC.IO;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using VRCOSC.Game.Util;

#pragma warning disable CS8618

// ReSharper disable InconsistentNaming

namespace VRCOSC.Game.Modules;

public abstract class Module
{
    private const string osc_ip_address = "127.0.0.1";
    private const int osc_port = 9000;

    public virtual string Title => string.Empty;
    public virtual string Description => string.Empty;
    public virtual string Author => string.Empty;
    public virtual Colour4 Colour => Colour4.Black;
    public virtual ModuleType Type => ModuleType.General;
    public virtual double DeltaUpdate => double.PositiveInfinity;

    public ModuleMetadata Metadata { get; } = new();

    protected TerminalLogger Terminal { get; private set; }

    private readonly UdpClient OscClient;

    public readonly ModuleDataManager DataManager;

    protected Module(Storage storage)
    {
        OscClient = new UdpClient(osc_ip_address, osc_port);
        DataManager = new ModuleDataManager(storage, GetType().Name);
    }

    internal void Start()
    {
        Terminal = new TerminalLogger(GetType().Name);
        Terminal.Log("Starting");
        OnStart();
    }

    protected virtual void OnStart() { }

    internal void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    internal void Stop()
    {
        Terminal.Log("Stopping");
        OnStop();
    }

    protected virtual void OnStop() { }

    protected void CreateSetting(Enum key, string displayName, string description, string defaultValue)
    {
        createSetting(key, displayName, description, defaultValue);
    }

    protected void CreateSetting(Enum key, string displayName, string description, int defaultValue)
    {
        createSetting(key, displayName, description, defaultValue);
    }

    protected void CreateSetting(Enum key, string displayName, string description, bool defaultValue)
    {
        createSetting(key, displayName, description, defaultValue);
    }

    private void createSetting(Enum key, string displayName, string description, object defaultValue)
    {
        var moduleSettingMetadata = new ModuleAttributeMetadata
        {
            DisplayName = displayName,
            Description = description
        };

        DataManager.SetSetting(key, defaultValue);
        Metadata.Settings.Add(key.ToString().ToLower(), moduleSettingMetadata);
    }

    protected void CreateParameter(Enum key, string displayName, string description, string defaultAddress)
    {
        var moduleOscParameterMetadata = new ModuleAttributeMetadata
        {
            DisplayName = displayName,
            Description = description,
        };

        DataManager.SetParameter(key, defaultAddress);
        Metadata.Parameters.Add(key.ToString().ToLower(), moduleOscParameterMetadata);
    }

    protected T GetSettingAs<T>(Enum key)
    {
        return DataManager.GetSettingAs<T>(key);
    }

    protected void SendParameter(Enum key, bool value)
    {
        SendParameter(key, value ? OscTrue.True : OscFalse.False);
    }

    protected void SendParameter(Enum key, object value)
    {
        var address = new Address(DataManager.GetParameter(key));
        var message = new OscMessage(address, new[] { value });
        OscClient.SendMessageAsync(message);
    }
}
