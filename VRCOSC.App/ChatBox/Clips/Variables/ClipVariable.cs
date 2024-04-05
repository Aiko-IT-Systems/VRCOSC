﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;

namespace VRCOSC.App.ChatBox.Clips.Variables;

public abstract class ClipVariable
{
    public string ModuleID { get; } = null!;
    public string VariableID { get; } = null!;

    public string DisplayName => ChatBoxManager.GetInstance().GetVariable(ModuleID, VariableID)!.DisplayName.Value;

    [JsonConstructor]
    internal ClipVariable()
    {
    }

    protected ClipVariable(ClipVariableReference reference)
    {
        ModuleID = reference.ModuleID;
        VariableID = reference.VariableID;
    }

    public string GetFormattedValue()
    {
        var variableValue = ChatBoxManager.GetInstance().GetVariable(ModuleID, VariableID)!.Value.Value;
        return variableValue is not null ? Format(variableValue) : string.Empty;
    }

    protected abstract string Format(object value);
}
