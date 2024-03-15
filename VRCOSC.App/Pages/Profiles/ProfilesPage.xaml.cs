﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VRCOSC.App.Profiles;

namespace VRCOSC.App.Pages.Profiles;

public partial class ProfilesPage
{
    public ProfilesPage()
    {
        DataContext = ProfileManager.GetInstance();
    }
}

public class BackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var index = System.Convert.ToInt32(value);
        return index % 2 == 0 ? Application.Current.Resources["CBackground3"] : Application.Current.Resources["CBackground2"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}