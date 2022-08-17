﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;

namespace VRCOSC.Game.Modules;

public abstract class ModuleAttribute
{
    public readonly ModuleAttributeMetadata Metadata;

    protected ModuleAttribute(ModuleAttributeMetadata metadata)
    {
        Metadata = metadata;
    }

    public abstract void SetDefault();
}

public class ModuleAttributeSingle : ModuleAttribute
{
    public readonly Bindable<object> Attribute;

    public ModuleAttributeSingle(ModuleAttributeMetadata metadata, object defaultValue)
        : base(metadata)
    {
        Attribute = new Bindable<object>(defaultValue);
    }

    public override void SetDefault()
    {
        Attribute.SetDefault();
    }
}

public sealed class ModuleAttributeSingleWithBounds : ModuleAttributeSingle
{
    public readonly object MinValue;
    public readonly object MaxValue;

    public ModuleAttributeSingleWithBounds(ModuleAttributeMetadata metadata, object defaultValue, object minValue, object maxValue)
        : base(metadata, defaultValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}

public sealed class ModuleAttributeList : ModuleAttribute
{
    // listen for changes in this to bind to save in Module class
    public readonly BindableList<Bindable<object>> AttributeList;
    public readonly List<object> DefaultValues;
    public readonly Type Type;

    public ModuleAttributeList(ModuleAttributeMetadata metadata, List<object> defaultValues, Type type)
        : base(metadata)
    {
        AttributeList = new BindableList<Bindable<object>>();
        DefaultValues = defaultValues;
        Type = type;

        SetDefault();
    }

    public override void SetDefault()
    {
        AttributeList.Clear();
        DefaultValues.ForEach(value => AttributeList.Add(new Bindable<object>(value)));
    }
}

public class ModuleAttributeMetadata
{
    public readonly string DisplayName;
    public readonly string Description;

    public ModuleAttributeMetadata(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
    }
}
