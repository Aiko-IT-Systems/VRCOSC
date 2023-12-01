﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using VRCOSC.Game.Graphics;
using VRCOSC.Game.Graphics.UI;
using VRCOSC.Game.Graphics.UI.List;
using VRCOSC.Game.Packages;

namespace VRCOSC.Game.Screens.Main.Repo;

public partial class ModulePackageInstance : HeightLimitedScrollableListItem
{
    [Resolved]
    private RepoTab repoTab { get; set; } = null!;

    private readonly PackageSource packageSource;

    private readonly Box background = null!;

    public ModulePackageInstance(PackageSource packageSource)
    {
        this.packageSource = packageSource;

        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Container infoButton;
        FillFlowContainer actionContainer;

        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding(10),
            Child = new GridContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 315),
                    new Dimension(GridSizeMode.Absolute, 242),
                    new Dimension(GridSizeMode.Absolute, 242),
                    new Dimension(GridSizeMode.Absolute, 135),
                    new Dimension(GridSizeMode.Absolute, 130),
                    new Dimension()
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable?[]
                    {
                        new InstanceSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = packageSource.GetDisplayName()
                        },
                        new LatestVersionSpriteText(packageSource)
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        new InstanceSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = packageSource.GetInstalledVersion()
                        },
                        new InstanceSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = packageSource.PackageType.ToString()
                        },
                        actionContainer = new FillFlowContainer
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Y,
                            AutoSizeAxes = Axes.X,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(8, 0)
                        },
                        new FillFlowContainer
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            RelativeSizeAxes = Axes.Y,
                            AutoSizeAxes = Axes.X,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(8, 0),
                            Children = new Drawable[]
                            {
                                infoButton = new Container
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    AutoSizeAxes = Axes.Both,
                                    Child = new IconButton
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(32),
                                        Icon = FontAwesome.Solid.Info,
                                        CornerRadius = 5,
                                        BackgroundColour = Colours.BLUE0,
                                        Action = () => repoTab.PackageInfo.CurrentPackageSource.Value = packageSource
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        if (packageSource.IsUpdateAvailable())
        {
            actionContainer.Add(new UpdateButton(packageSource));
        }

        if (packageSource.IsAvailable() && !packageSource.IsInstalled())
        {
            actionContainer.Add(new InstallButton(packageSource));
        }

        if (packageSource.IsInstalled())
        {
            actionContainer.Add(new UninstallButton(packageSource));
        }

        if (packageSource.IsUnavailable())
        {
            infoButton.Hide();
        }
    }

    public bool Satisfies(PackageListingFilter filter)
    {
        var satisfies = (((packageSource.PackageType == PackageType.Official && filter.HasFlagFast(PackageListingFilter.Type_Official)) ||
                          (packageSource.PackageType == PackageType.Curated && filter.HasFlagFast(PackageListingFilter.Type_Curated)) ||
                          (packageSource.PackageType == PackageType.Community && filter.HasFlagFast(PackageListingFilter.Type_Community))) &&
                         ((packageSource.IsUnavailable() && filter.HasFlagFast(PackageListingFilter.Release_Unavailable)) ||
                          (packageSource.IsIncompatible() && filter.HasFlagFast(PackageListingFilter.Release_Incompatible)) ||
                          packageSource.IsAvailable())) ||
                        packageSource.IsInstalled();

        Content.Alpha = satisfies ? 1 : 0;

        return satisfies;
    }

    private partial class InstanceSpriteText : SpriteText
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Font = Fonts.REGULAR.With(size: 23);
            Colour = Colours.WHITE0;
        }
    }

    private partial class LatestVersionSpriteText : InstanceSpriteText
    {
        private readonly PackageSource packageSource;

        public LatestVersionSpriteText(PackageSource packageSource)
        {
            this.packageSource = packageSource;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (packageSource.IsUnavailable())
            {
                Text = "Unavailable";
                Colour = Colours.RED1;
            }
            else if (packageSource.IsIncompatible())
            {
                Text = "Incompatible";
                Colour = Colours.ORANGE1;
            }
            else
            {
                Text = packageSource.LatestVersion!;
            }
        }
    }

    private partial class ActionButton : IconButton
    {
        protected readonly PackageSource PackageSource;

        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        [Resolved]
        private AppManager appManager { get; set; } = null!;

        protected ActionButton(PackageSource packageSource)
        {
            PackageSource = packageSource;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            Size = new Vector2(32);
            CornerRadius = 5;

            Action += async () => await ExecuteAction();
        }

        protected virtual async Task ExecuteAction()
        {
            PackageSource.Progress = loadingInfo =>
            {
                game.LoadingScreen.Action.Value = loadingInfo.Action;
                game.LoadingScreen.Progress.Value = loadingInfo.Progress;

                if (loadingInfo.Complete)
                {
                    appManager.ModuleManager.ReloadAllModules();
                    game.OnListingRefresh?.Invoke();
                    game.LoadingScreen.Hide();
                }
            };

            await appManager.StopAsync();
            game.LoadingScreen.Show();
        }
    }

    private partial class InstallButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public InstallButton(PackageSource packageSource)
            : base(packageSource)
        {
            BackgroundColour = Colours.GREEN0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Plus;
        }

        protected override async Task ExecuteAction()
        {
            await base.ExecuteAction();

            game.LoadingScreen.Title.Value = "Installing...";
            game.LoadingScreen.Description.Value = $"Sit tight while {PackageSource.GetDisplayName()} is installed!";
            await PackageSource.Install();
        }
    }

    private partial class UninstallButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public UninstallButton(PackageSource remoteModuleSource)
            : base(remoteModuleSource)
        {
            BackgroundColour = Colours.RED0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Minus;
        }

        protected override async Task ExecuteAction()
        {
            await base.ExecuteAction();

            game.LoadingScreen.Title.Value = "Uninstalling...";
            game.LoadingScreen.Description.Value = "So long and thanks for all the fish";
            PackageSource.Uninstall();
        }
    }

    private partial class UpdateButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public UpdateButton(PackageSource remoteModuleSource)
            : base(remoteModuleSource)
        {
            BackgroundColour = Colours.BLUE0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Redo;
        }

        protected override async Task ExecuteAction()
        {
            await base.ExecuteAction();

            game.LoadingScreen.Title.Value = "Updating...";
            game.LoadingScreen.Description.Value = $"Sit tight! {PackageSource.GetDisplayName()} is being updated!";
            await PackageSource.Install();
        }
    }
}
