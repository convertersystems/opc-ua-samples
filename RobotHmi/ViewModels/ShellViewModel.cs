// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro;
using Prism.Mvvm;

namespace RobotHmi.ViewModels
{
    /// <summary>
    /// A view model for Shell.
    /// </summary>
    public class ShellViewModel : BindableBase
    {
        private Accent selectedAccent;
        private AppTheme selectedTheme;

        public ShellViewModel()
        {
            this.AppThemes = ThemeManager.AppThemes.ToList();
            this.Accents = ThemeManager.Accents.ToList();

            var current = ThemeManager.DetectAppStyle(Application.Current);
            if (current != null)
            {
                this.selectedTheme = current.Item1;
                this.selectedAccent = current.Item2;
            }
            else
            {
                this.selectedTheme = ThemeManager.GetAppTheme("BaseLight");
                this.selectedAccent = ThemeManager.GetAccent("Blue");
            }
        }

        public IEnumerable<AppTheme> AppThemes { get; private set; }

        public AppTheme SelectedTheme
        {
            get
            {
                return this.selectedTheme;
            }

            set
            {
                this.SetProperty(ref this.selectedTheme, value);
                ThemeManager.ChangeAppStyle(Application.Current, this.selectedAccent, this.selectedTheme);
            }
        }

        public IEnumerable<Accent> Accents { get; private set; }

        public Accent SelectedAccent
        {
            get
            {
                return this.selectedAccent;
            }

            set
            {
                this.SetProperty(ref this.selectedAccent, value);
                ThemeManager.ChangeAppStyle(Application.Current, this.selectedAccent, this.selectedTheme);
            }
        }
    }
}