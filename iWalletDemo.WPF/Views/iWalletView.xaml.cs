using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
    }
}
