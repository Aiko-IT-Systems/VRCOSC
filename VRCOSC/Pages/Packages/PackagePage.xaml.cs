﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

namespace VRCOSC.Pages.Packages;

public partial class PackagePage
{
    private readonly PackageViewModel packageViewModel = new();

    public PackagePage()
    {
        InitializeComponent();

        PackageGrid.DataContext = packageViewModel;
    }
}