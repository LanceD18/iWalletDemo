using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MvvmCross;
using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Views;

namespace iWalletDemo.WPF
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : MvxApplication
    {
        // called before OnStartup
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<Setup>();
        }
    }
}
