// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using VRCOSC.Game.Graphics.Containers.UI.Button;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Graphics.Containers.Screens.ModuleSelect;

public sealed class ModuleListingGroup : Container, IFilterable
{
    private readonly ModuleGroup moduleGroup;

    [Resolved]
    private ModuleSelection moduleSelection { get; set; }

    [Resolved]
    private ModuleManager moduleManager { get; set; }

    public ModuleListingGroup(ModuleGroup moduleGroup)
    {
        this.moduleGroup = moduleGroup;

        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(5);

        populateFilter();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        SearchContainer<ModuleCard> moduleCardFlow;

        DropdownButton dropdownButton;
        Children = new Drawable[]
        {
            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(5),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 50,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Aqua
                            },
                            new FillFlowContainer
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.Y,
                                AutoSizeAxes = Axes.X,
                                Direction = FillDirection.Horizontal,
                                Children = new Drawable[]
                                {
                                    dropdownButton = new DropdownButton
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.Both,
                                        FillMode = FillMode.Fit,
                                        State = { Value = true }
                                    },
                                    new SpriteText
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Text = moduleGroup.Type.ToString(),
                                        Font = FrameworkFont.Regular.With(size: 30)
                                    }
                                }
                            }
                        }
                    },
                    moduleCardFlow = new SearchContainer<ModuleCard>
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical
                    }
                }
            }
        };

        moduleGroup.ForEach(moduleContainer => moduleCardFlow.Add(new ModuleCard(moduleContainer.Module)));
        moduleSelection.SearchString.ValueChanged += (searchTerm) => moduleCardFlow.SearchTerm = searchTerm.NewValue;
        moduleSelection.ShowExperimental.BindValueChanged(e =>
        {
            moduleCardFlow.ForEach(card =>
            {
                if (!card.SourceModule.Experimental) return;

                if (e.NewValue)
                {
                    card.Show();
                }
                else
                {
                    card.Hide();
                }
            });
        }, true);

        dropdownButton.State.ValueChanged += (e) =>
        {
            if (e.NewValue)
            {
                moduleCardFlow.ScaleTo(new Vector2(1), 500, Easing.OutElastic);
            }
            else
            {
                moduleCardFlow.ScaleTo(new Vector2(1, 0), 500, Easing.OutQuart);
            }
        };
    }

    private void populateFilter()
    {
        List<LocalisableString> localFilters = new List<LocalisableString>();

        localFilters.Add(moduleGroup.Type.ToString());

        moduleGroup.ForEach(module =>
        {
            localFilters.Add(module.Module.Title);
            localFilters.Add(module.Module.Author);
            module.Module.Tags.ForEach(tag => localFilters.Add(tag));
        });

        FilterTerms = localFilters;
    }

    public IEnumerable<LocalisableString> FilterTerms { get; set; }

    public bool MatchingFilter
    {
        set
        {
            if (value)
                this.FadeIn();
            else
                this.FadeOut();
        }
    }

    public bool FilteringActive { get; set; }
}
