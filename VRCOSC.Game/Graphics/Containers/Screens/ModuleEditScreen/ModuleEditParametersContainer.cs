﻿using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using VRCOSC.Game.Graphics.Containers.Module.ModulOscParameter;

namespace VRCOSC.Game.Graphics.Containers.Screens.ModuleEditScreen;

public class ModuleEditParametersContainer : Container
{
    public Modules.Module SourceModule { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        FillFlowContainer<ModuleOscParameterContainer> parametersFlow;

        InternalChildren = new Drawable[]
        {
            new SpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Font = FrameworkFont.Regular.With(size: 50),
                Text = "Values"
            },
            parametersFlow = new FillFlowContainer<ModuleOscParameterContainer>
            {
                Name = "Values",
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 5)
            }
        };

        SourceModule.Data.Parameters.Keys.ForEach(key =>
        {
            parametersFlow.Add(new ModuleOscParameterContainer
            {
                Key = key,
                SourceModule = SourceModule
            });
        });
    }
}
