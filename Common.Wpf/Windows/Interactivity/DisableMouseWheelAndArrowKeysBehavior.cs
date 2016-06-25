// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Workstation.Windows.Interactivity
{
    public class DisableMouseWheelAndArrowKeysBehavior : Behavior<UIElement>
    {
        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = true;
            }
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
            this.AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            this.AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
    }
}