// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Control;
using VRCOSC.Game.Util;

namespace VRCOSC.Game.Modules.Modules.Media;

public class MediaProvider
{
    private MediaManager? mediaManager;
    private Process? trackedProcess;
    private string? lastSender;

    public MediaState State { get; private set; } = null!;

    public GlobalSystemMediaTransportControlsSession? Controller
        => mediaManager?.CurrentMediaSessions.ContainsKey(lastSender ?? string.Empty) ?? false ? mediaManager.CurrentMediaSessions[lastSender!].ControlSession : null;

    public Action? OnMediaSessionOpened;
    public Action? OnMediaUpdate;

    public async Task StartMediaHook()
    {
        State = new MediaState();

        mediaManager = new MediaManager();
        mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
        mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
        mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
        mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;
        await mediaManager.Start();
    }

    public void StopMediaHook()
    {
        mediaManager?.Dispose();
        mediaManager = null;
        lastSender = null;
        trackedProcess = null;
    }

    private void MediaManager_OnAnySessionOpened(MediaManager.MediaSession sender)
    {
        updateTrackedProcess(sender.Id);
        OnMediaSessionOpened?.Invoke();
    }

    public async Task ForceUpdate()
    {
        if (Controller?.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
        {
            await Controller.TryPauseAsync();
            await Task.Delay(50);
            await Controller.TryPlayAsync();
        }
        else if (Controller?.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
        {
            await Controller.TryPlayAsync();
            await Task.Delay(50);
            await Controller.TryPauseAsync();
        }
    }

    private void MediaManager_OnAnySessionClosed(MediaManager.MediaSession sender)
    {
        updateTrackedProcess(mediaManager?.CurrentMediaSessions.FirstOrDefault().Value.Id ?? string.Empty);
        _ = ForceUpdate();
    }

    private void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionPlaybackInfo args)
    {
        updateTrackedProcess(sender.Id);

        var mediaProperties = sender.ControlSession?.TryGetMediaPropertiesAsync().GetResults();

        if (mediaProperties is not null)
        {
            State.Title = mediaProperties.Title;
            State.Artist = mediaProperties.Artist;
        }

        State.IsShuffle = args.IsShuffleActive ?? false;
        State.RepeatMode = args.AutoRepeatMode ?? 0;
        State.Status = args.PlaybackStatus;

        OnMediaUpdate?.Invoke();
    }

    private void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionMediaProperties args)
    {
        updateTrackedProcess(sender.Id);

        var playbackInfo = sender.ControlSession?.GetPlaybackInfo();
        if (playbackInfo is null) return;

        State.IsShuffle = playbackInfo.IsShuffleActive ?? false;
        State.RepeatMode = playbackInfo.AutoRepeatMode ?? 0;
        State.Status = playbackInfo.PlaybackStatus;
        State.Title = args.Title;
        State.Artist = args.Artist;

        OnMediaUpdate?.Invoke();
    }

    private void updateTrackedProcess(string senderId)
    {
        if (lastSender is null || lastSender != senderId)
        {
            trackedProcess = Process.GetProcessesByName(senderId.Replace(".exe", string.Empty)).FirstOrDefault();
            lastSender = trackedProcess is null ? null : senderId;
        }
    }

    public void SetVolume(float percentage) => ProcessExtensions.SetProcessVolume(lastSender ?? string.Empty, percentage);
    public void SetMuted(bool muted) => ProcessExtensions.SetProcessMuted(lastSender ?? string.Empty, muted);

    public float GetVolume() => ProcessExtensions.RetrieveProcessVolume(lastSender ?? string.Empty);
    public bool IsMuted() => ProcessExtensions.IsProcessMuted(lastSender ?? string.Empty);
}

public class MediaState
{
    public string Title = string.Empty;
    public string Artist = string.Empty;
    public MediaPlaybackAutoRepeatMode RepeatMode;
    public bool IsShuffle;
    public GlobalSystemMediaTransportControlsSessionPlaybackStatus Status;
    public GlobalSystemMediaTransportControlsSessionTimelineProperties? Position;
    public bool IsPlaying => Status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
}