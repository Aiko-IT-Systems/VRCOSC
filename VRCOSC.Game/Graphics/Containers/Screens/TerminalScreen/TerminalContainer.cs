﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;

namespace VRCOSC.Game.Graphics.Containers.Screens.TerminalScreen;

public class TerminalContainer : Container
{
    private BasicScrollContainer terminalScroll;
    private FillFlowContainer<SpriteText> terminalFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Logger.NewEntry += logEntry =>
        {
            if (logEntry.LoggerName == "terminal")
                log(logEntry.Message);
        };

        InternalChild = new Container
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 20,
            Children = new Drawable[]
            {
                new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = VRCOSCColour.Gray3
                },
                new Container
                {
                    Name = "Content",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(10),
                    Child = terminalScroll = new BasicScrollContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        ClampExtension = 20,
                        ScrollbarVisible = false,
                        Child = terminalFlow = new FillFlowContainer<SpriteText>
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical
                        }
                    }
                }
            }
        };
    }

    private void log(string text)
    {
        Scheduler.Add(() =>
        {
            if (terminalFlow.Count >= 50) terminalFlow[0].RemoveAndDisposeImmediately();
            var formattedText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}";
            terminalFlow.Add(new SpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Font = FrameworkFont.Regular.With(size: 20),
                Colour = VRCOSCColour.Gray8,
                Text = formattedText
            });
            Scheduler.Add(() => terminalScroll.ScrollToEnd());
        });
    }
}
