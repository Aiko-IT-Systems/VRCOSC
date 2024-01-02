﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using VRCOSC.Graphics;

namespace VRCOSC.Screens.Main.Modules.Parameters;

public partial class ModuleParametersListHeader : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.GRAY0
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(7),
                Child = new GridContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 200),
                        new Dimension(GridSizeMode.Absolute, 7),
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 7),
                        new Dimension(GridSizeMode.Absolute, 100),
                        new Dimension(GridSizeMode.Absolute, 7),
                        new Dimension(GridSizeMode.Absolute, 100),
                        new Dimension(GridSizeMode.Absolute, 7),
                        new Dimension(GridSizeMode.Absolute, 300)
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable?[]
                        {
                            new HeaderSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = "Title"
                            },
                            null,
                            new HeaderSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = "Description"
                            },
                            null,
                            new HeaderSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Type"
                            },
                            null,
                            new HeaderSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Read/Write"
                            },
                            null,
                            new HeaderSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Parameter Name"
                            }
                        }
                    }
                }
            }
        };
    }

    private partial class HeaderSpriteText : SpriteText
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Font = Fonts.BOLD.With(size: 23);
            Colour = Colours.WHITE2;
        }
    }
}