// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro;
using Prism.Mvvm;
using ControlzEx.Theming;
using System.Windows.Media;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for Shell.
    /// </summary>
    public class ShellViewModel : BindableBase
    {
        private AppThemeMenuData selectedTheme;
        private AccentColorMenuData selectedAccent;

        public ShellViewModel()
        {
            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.Current.Themes
                                         .GroupBy(x => x.BaseColorScheme)
                                         .Select(x => x.First())
                                         .Select(a => new AppThemeMenuData() { Name = a.BaseColorScheme, BorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, ColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();

            // create accent color menu items for the demo
            this.Accents = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorMenuData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                                            .ToList();


            var current = ThemeManager.Current.DetectTheme(Application.Current);
            if (current != null)
            {
                //this.selectedTheme = current;
                //this.selectedAccent = current.;
            }
            //else
            //{
            //    this.selectedTheme = ThemeManager.Current.GetTheme("BaseLight");
            //    //this.selectedAccent = ThemeManager.Current.GetAccent("Blue");
            //}
        }

        public IEnumerable<AppThemeMenuData> AppThemes { get; private set; }

        public AppThemeMenuData SelectedTheme
        {
            get
            {
                return this.selectedTheme;
            }

            set
            {
                this.SetProperty(ref this.selectedTheme, value);
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, this.selectedTheme.Name);
            }
        }

        public IEnumerable<AccentColorMenuData> Accents { get; private set; }

        public AccentColorMenuData SelectedAccent
        {
            get
            {
                return this.selectedAccent;
            }

            set
            {
                this.SetProperty(ref this.selectedAccent, value);
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, this.selectedAccent.Name);
            }
        }
    }
    public class AccentColorMenuData
    {
        public string Name { get; set; }

        public Brush BorderColorBrush { get; set; }

        public Brush ColorBrush { get; set; }

        //public AccentColorMenuData()
        //{
        //    this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        //}

        //public ICommand ChangeAccentCommand { get; }

        //protected virtual void DoChangeTheme(object sender)
        //{
        //    ThemeManager.Current.ChangeThemeColorScheme(Application.Current, this.Name);
        //}
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        //protected override void DoChangeTheme(object sender)
        //{
        //    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, this.Name);
        //}
    }
}