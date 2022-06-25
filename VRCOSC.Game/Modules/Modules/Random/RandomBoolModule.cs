﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using osu.Framework.Graphics;

namespace VRCOSC.Game.Modules.Modules.Random;

public class RandomBoolModule : Module
{
    public override string Title => "Random Bool";
    public override string Description => "Sends a random bool over a variable time period";
    public override string Author => "VolcanicArts";
    public override Colour4 Colour => Colour4.Coral.Darken(0.5f);
    public override ModuleType ModuleType => ModuleType.General;
    protected override double DeltaUpdate => GetSetting<int>(RandomBoolSetting.DeltaUpdate);

    private readonly System.Random random = new();

    public override void CreateAttributes()
    {
        CreateSetting(RandomBoolSetting.DeltaUpdate, "Time Between Update", "The amount of time, in milliseconds, between each random value", 1000);

        CreateOutputParameter(RandomBoolOutputParameter.RandomBool, "Random Bool", "A random bool value", "/avatar/parameters/RandomBool");
    }

    protected override void OnUpdate()
    {
        float randomFloat = (float)random.NextDouble();
        bool randomBool = (int)MathF.Round(randomFloat) == 1;
        SendParameter(RandomBoolOutputParameter.RandomBool, randomBool);
    }

    private enum RandomBoolSetting
    {
        DeltaUpdate
    }

    private enum RandomBoolOutputParameter
    {
        RandomBool
    }
}