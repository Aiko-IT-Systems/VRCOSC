﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using osu.Framework.Logging;
using VRCOSC.Game.OSC.Client;
using VRCOSC.Game.OSC.VRChat;

namespace VRCOSC.Game.OSC;

public class OSCRouter
{
    private readonly VRChatOscClient vrChatOscClient;

    public readonly List<OscSender> Senders = new();
    public readonly List<OscReceiver> Receivers = new();

    public OSCRouter(VRChatOscClient vrChatOscClient)
    {
        this.vrChatOscClient = vrChatOscClient;
    }

    public void Initialise(List<OSCRouterPair> pairs)
    {
        Senders.Clear();
        Receivers.Clear();

        pairs.ForEach(pair =>
        {
            var sender = new OscSender();
            var receiver = new OscReceiver();

            sender.Initialise(new IPEndPoint(IPAddress.Parse("127.0.0.1"), pair.Send));
            receiver.Initialise(new IPEndPoint(IPAddress.Parse("127.0.0.1"), pair.Listen));

            Logger.Log($"Initialising new router on {pair.Listen}:{pair.Send}");

            Senders.Add(sender);
            Receivers.Add(receiver);
        });
    }

    // Anything coming from VRC has to be parsed first, hence listening for parameters and not the raw data
    // I have no idea why, it should just work forwarding the raw bytes
    public void Enable()
    {
        Senders.ForEach(sender =>
        {
            vrChatOscClient.OnParameterReceived += parameter =>
            {
                sender.Send(OscEncoder.Encode(new OscMessage(parameter.Address, parameter.Values)));
            };

            sender.Enable();
        });

        Receivers.ForEach(receiver =>
        {
            receiver.OnRawDataReceived += byteData =>
            {
                vrChatOscClient.SendByteData(byteData);
            };

            receiver.Enable();
        });
    }

    public async Task Disable()
    {
        foreach (var sender in Senders) sender.Disable();
        foreach (var receiver in Receivers) await receiver.Disable();
    }
}

public class OSCRouterPair
{
    public required int Send { get; init; }
    public required int Listen { get; init; }
}
