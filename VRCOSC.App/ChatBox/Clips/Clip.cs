﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using VRCOSC.App.Utils;

namespace VRCOSC.App.ChatBox.Clips;

public class Clip
{
    public Observable<bool> Enabled { get; } = new(true);
    public Observable<string> Name { get; } = new("New Clip");

    public Observable<int> Start { get; } = new();
    public Observable<int> End { get; } = new();
    public ObservableCollection<string> LinkedModules { get; } = new();
}
