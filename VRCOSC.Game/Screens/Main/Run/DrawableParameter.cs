﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using VRCOSC.Game.Graphics;
using VRCOSC.Game.Graphics.UI.List;

namespace VRCOSC.Game.Screens.Main.Run;

public partial class DrawableParameter : HeightLimitedScrollableListItem
{
    private readonly string parameterName;
    private readonly object parameterInitialValue;

    private SpriteText valueSpriteText = null!;

    public DrawableParameter(string parameterName, object parameterInitialValue)
    {
        this.parameterName = parameterName;
        this.parameterInitialValue = parameterInitialValue;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding
            {
                Vertical = 2,
                Horizontal = 5
            },
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = parameterName,
                    Font = Fonts.REGULAR.With(size: 20),
                    Colour = Colours.WHITE2
                },
                valueSpriteText = new SpriteText
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Font = Fonts.REGULAR.With(size: 20),
                    Colour = Colours.WHITE2,
                    Text = parameterInitialValue.ToString() ?? "INVALID"
                }
            }
        };
    }

    public void UpdateValue(object value)
    {
        if (valueSpriteText.Text == (value.ToString() ?? "INVALID")) return;

        valueSpriteText.Text = value.ToString() ?? "INVALID";
        Background.FlashColour(Colours.WHITE0.Opacity(0.5f), 500, Easing.OutQuint);
    }
}
