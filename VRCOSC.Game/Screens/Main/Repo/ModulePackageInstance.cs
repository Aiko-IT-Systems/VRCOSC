﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using VRCOSC.Game.Graphics;
using VRCOSC.Game.Graphics.UI;
using VRCOSC.Game.Modules.Remote;

namespace VRCOSC.Game.Screens.Main.Repo;

public partial class ModulePackageInstance : Container
{
    [Resolved]
    private RepoTab repoTab { get; set; } = null!;

    private readonly RemoteModuleSource remoteModuleSource;
    private readonly bool even;

    private Container infoButton = null!;

    public ModulePackageInstance(RemoteModuleSource remoteModuleSource, bool even)
    {
        this.remoteModuleSource = remoteModuleSource;
        this.even = even;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        RelativeSizeAxes = Axes.X;
        Height = 50;

        FillFlowContainer actionContainer;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = even ? Colours.GRAY4 : Colours.GRAY2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                Child = new GridContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 315),
                        new Dimension(GridSizeMode.Absolute, 242),
                        new Dimension(GridSizeMode.Absolute, 242),
                        new Dimension(GridSizeMode.Absolute, 135),
                        new Dimension(GridSizeMode.Absolute, 130),
                        new Dimension()
                    },
                    Content = new[]
                    {
                        new Drawable?[]
                        {
                            new InstanceSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = remoteModuleSource.DisplayName
                            },
                            new LatestVersionSpriteText(remoteModuleSource)
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            new InstanceSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = remoteModuleSource.GetInstalledVersion()
                            },
                            new InstanceSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = remoteModuleSource.SourceType.ToString()
                            },
                            actionContainer = new FillFlowContainer
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                AutoSizeAxes = Axes.Both,
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
                                            Size = new Vector2(36),
                                            Icon = FontAwesome.Solid.Info,
                                            CornerRadius = 5,
                                            BackgroundColour = Colours.BLUE0,
                                            Action = () => repoTab.PackageInfo.CurrentRemoteModuleSource.Value = remoteModuleSource
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        if (remoteModuleSource.IsUpdateAvailable())
        {
            actionContainer.Add(new UpdateButton(remoteModuleSource));
        }

        if (remoteModuleSource.IsAvailable() && !remoteModuleSource.IsInstalled())
        {
            actionContainer.Add(new InstallButton(remoteModuleSource));
        }

        if (remoteModuleSource.IsInstalled())
        {
            actionContainer.Add(new UninstallButton(remoteModuleSource));
        }

        if (remoteModuleSource.IsUnavailable())
        {
            infoButton.Hide();
        }
    }

    private partial class InstanceSpriteText : SpriteText
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Font = Fonts.REGULAR.With(size: 27);
            Colour = Colours.WHITE0;
        }
    }

    private partial class LatestVersionSpriteText : InstanceSpriteText
    {
        private readonly RemoteModuleSource remoteModuleSource;

        public LatestVersionSpriteText(RemoteModuleSource remoteModuleSource)
        {
            this.remoteModuleSource = remoteModuleSource;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (remoteModuleSource.IsIncompatible())
            {
                Text = "Incompatible";
                Colour = Colours.ORANGE1;
            }
            else if (remoteModuleSource.IsUnavailable())
            {
                Text = "Unavailable";
                Colour = Colours.RED1;
            }
            else
            {
                Text = remoteModuleSource.LatestRelease!.TagName;
            }
        }
    }

    private partial class ActionButton : IconButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        [Resolved]
        private RepoTab repoTab { get; set; } = null!;

        private readonly RemoteModuleSource remoteModuleSource;

        protected ActionButton(RemoteModuleSource remoteModuleSource)
        {
            this.remoteModuleSource = remoteModuleSource;

            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            Size = new Vector2(36);
            CornerRadius = 5;

            Action += () =>
            {
                game.LoadingScreen.Show();
                remoteModuleSource.Progress = loadingInfo =>
                {
                    game.LoadingScreen.Action.Value = loadingInfo.Action;
                    game.LoadingScreen.Progress.Value = loadingInfo.Progress;

                    if (loadingInfo.Complete)
                    {
                        repoTab.Refresh();
                        game.LoadingScreen.Hide();
                    }
                };
            };
        }
    }

    private partial class InstallButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public InstallButton(RemoteModuleSource remoteModuleSource)
            : base(remoteModuleSource)
        {
            BackgroundColour = Colours.GREEN0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Plus;

            Action += async () =>
            {
                game.LoadingScreen.Title.Value = "Installing...";
                game.LoadingScreen.Description.Value = $"Sit tight while {remoteModuleSource.DisplayName} is installed!";
                await remoteModuleSource.Install();
            };
        }
    }

    private partial class UninstallButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public UninstallButton(RemoteModuleSource remoteModuleSource)
            : base(remoteModuleSource)
        {
            BackgroundColour = Colours.RED0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Minus;

            Action += () =>
            {
                game.LoadingScreen.Title.Value = "Uninstalling...";
                game.LoadingScreen.Description.Value = "So long and thanks for all the fish";
                remoteModuleSource.Uninstall();
            };
        }
    }

    private partial class UpdateButton : ActionButton
    {
        [Resolved]
        private VRCOSCGame game { get; set; } = null!;

        public UpdateButton(RemoteModuleSource remoteModuleSource)
            : base(remoteModuleSource)
        {
            BackgroundColour = Colours.BLUE0;
            IconColour = Colours.WHITE0;
            Icon = FontAwesome.Solid.Redo;

            Action += async () =>
            {
                game.LoadingScreen.Title.Value = "Updating...";
                game.LoadingScreen.Description.Value = $"Sit tight! {remoteModuleSource.DisplayName} is being updated!";
                await remoteModuleSource.Install();
            };
        }
    }
}