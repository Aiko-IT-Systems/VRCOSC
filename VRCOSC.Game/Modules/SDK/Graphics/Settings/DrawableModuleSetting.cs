﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using VRCOSC.Game.Graphics;
using VRCOSC.Game.Modules.SDK.Attributes;

namespace VRCOSC.Game.Modules.SDK.Graphics.Settings;

public abstract partial class DrawableModuleSetting<T> : Container where T : ModuleSetting
{
    protected T ModuleSetting;

    protected override Container<Drawable> Content { get; }

    protected readonly Container SideContainer;

    protected DrawableModuleSetting(T moduleSetting)
    {
        ModuleSetting = moduleSetting;

        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Masking = true;
        CornerRadius = 5;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.GRAY4
            },
            new FillFlowContainer
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Padding = new MarginPadding(7),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Font = Fonts.REGULAR.With(size: 25),
                                        Colour = Colours.WHITE0,
                                        Text = ModuleSetting.Metadata.Title
                                    },
                                    new TextFlowContainer(t =>
                                    {
                                        t.Font = Fonts.REGULAR.With(size: 20);
                                        t.Colour = Colours.WHITE2;
                                    })
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Text = ModuleSetting.Metadata.Description
                                    },
                                    Content = new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Margin = new MarginPadding
                                        {
                                            Top = 7
                                        }
                                    }
                                }
                            },
                            SideContainer = new Container
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y
                            }
                        }
                    }
                }
            }
        };
    }
}

public abstract partial class DrawableValueModuleSetting<T> : DrawableModuleSetting<T> where T : ModuleSetting
{
    protected DrawableValueModuleSetting(T moduleAttribute)
        : base(moduleAttribute)
    {
    }

    protected internal void AddSide(Drawable drawable) => SideContainer.Add(drawable);
}

public abstract partial class DrawableListModuleSetting<T> : DrawableModuleSetting<T> where T : ModuleSetting
{
    protected DrawableListModuleSetting(T moduleSetting)
        : base(moduleSetting)
    {
    }
}