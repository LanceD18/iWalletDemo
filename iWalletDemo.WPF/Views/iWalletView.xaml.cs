using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iWalletDemo.Core.Util;
using iWalletDemo.Core.ViewModels;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;

namespace iWalletDemo.WPF.Views
{
    /// <summary>
    /// Interaction logic for iWalletView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(iWalletViewModel))]
    public partial class iWalletView : MvxWpfView
    {
        public iWalletView()
        {
            InitializeComponent();
        }

        private void ToggleButton_OnChecked_ToggleTimer(object sender, RoutedEventArgs e)
        {
            //? This will check the information from the previous state, so we must perform the reverse action
            if (!WpfUtil.DebugRecommendationNotificationTimer.IsEnabled)
            {
                Debug.WriteLine("start");
                WpfUtil.DebugRecommendationNotificationTimer.Start();
            }
            else
            {
                Debug.WriteLine("stop");
                WpfUtil.DebugRecommendationNotificationTimer.Stop();
            }
        }
    }
}
