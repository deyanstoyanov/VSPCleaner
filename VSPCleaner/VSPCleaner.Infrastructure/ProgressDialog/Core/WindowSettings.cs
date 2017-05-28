namespace VSPCleaner.Infrastructure.ProgressDialog.Core
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public class WindowSettings
    {
        #region public bool HideCloseButton (attached)

        public static readonly DependencyProperty HideCloseButtonProperty =
            DependencyProperty.RegisterAttached(
                "HideCloseButton", 
                typeof(bool), 
                typeof(WindowSettings), 
                new FrameworkPropertyMetadata(false, OnHideCloseButtonPropertyChanged));

        public static bool GetHideCloseButton(FrameworkElement element)
            => (bool)element.GetValue(HideCloseButtonProperty);

        public static void SetHideCloseButton(FrameworkElement element, bool hideCloseButton)
            => element.SetValue(HideCloseButtonProperty, hideCloseButton);

        private static void OnHideCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
            {
                return;
            }

            var hideCloseButton = (bool)e.NewValue;
            if (hideCloseButton && !GetIsCloseButtonHidden(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded += OnWindowLoaded;
                }
                else
                {
                    HideCloseButton(window);
                }

                SetIsCloseButtonHidden(window, true);
            }
            else if (!hideCloseButton && GetIsCloseButtonHidden(window))
            {
                if (!window.IsLoaded)
                {
                    window.Loaded -= OnWindowLoaded;
                }
                else
                {
                    ShowCloseButton(window);
                }

                SetIsCloseButtonHidden(window, false);
            }
        }

        private static readonly RoutedEventHandler OnWindowLoaded = (s, e) =>
            {
                if (!(s is Window))
                {
                    return;
                }

                var window = s as Window;
                HideCloseButton(window);
                window.Loaded -= OnWindowLoaded;
            };

        #endregion

        #region public bool IsCloseButtonHidden (readonly attached)

        private static readonly DependencyPropertyKey IsHiddenCloseButtonKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsCloseButtonHidden", 
                typeof(bool), 
                typeof(WindowSettings), 
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsCloseButtonHiddenProperty =
            IsHiddenCloseButtonKey.DependencyProperty;

        public static bool GetIsCloseButtonHidden(FrameworkElement element) => (bool)element.GetValue(IsCloseButtonHiddenProperty);

        private static void SetIsCloseButtonHidden(FrameworkElement element, bool isCloseButtonHidden) => element.SetValue(IsHiddenCloseButtonKey, isCloseButtonHidden);

        #endregion

        #region Helper Methods

        private static void HideCloseButton(Window w)
        {
            var hWnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hWnd, GwlStyle, GetWindowLong(hWnd, GwlStyle) & ~WsSysmenu);
        }

        private static void ShowCloseButton(Window w)
        {
            var hWnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hWnd, GwlStyle, GetWindowLong(hWnd, GwlStyle) | WsSysmenu);
        }

        #endregion

        #region Win32 Native Methods And Constants

        private const int GwlStyle = -16;

        private const int WsSysmenu = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion
    }
}