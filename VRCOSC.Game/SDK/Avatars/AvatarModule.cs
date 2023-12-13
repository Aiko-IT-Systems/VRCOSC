﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using VRCOSC.OSC.VRChat;
using VRCOSC.SDK.Parameters;
using VRCOSC.SDK.VRChat;

namespace VRCOSC.SDK.Avatars;

public abstract class AvatarModule : Module
{
    protected Player Player => AppManager.VRChatClient.Player;

    private void avatarChange()
    {
        try
        {
            OnAvatarChange();
        }
        catch (Exception e)
        {
            PushException(e);
        }
    }

    internal void PlayerUpdate()
    {
        try
        {
            OnPlayerUpdate();
        }
        catch (Exception e)
        {
            PushException(e);
        }
    }

    protected virtual void OnAvatarChange()
    {
    }

    protected virtual void OnPlayerUpdate()
    {
    }

    internal override void OnParameterReceived(VRChatOscMessage message)
    {
        if (message.IsAvatarChangeEvent)
        {
            avatarChange();
            return;
        }

        base.OnParameterReceived(message);
    }

    protected internal override void InternalOnRegisteredParameterReceived(RegisteredParameter registeredParameter)
    {
        OnRegisteredParameterReceived(new AvatarParameter(registeredParameter));
    }

    protected virtual void OnRegisteredParameterReceived(AvatarParameter avatarParameter)
    {
    }
}
