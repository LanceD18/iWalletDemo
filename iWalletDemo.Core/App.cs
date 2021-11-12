using System;
using System.Collections.Generic;
using System.Text;
using iWalletDemo.Core.ViewModels;
using MvvmCross.ViewModels;

namespace iWalletDemo.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<iWalletViewModel>();
        }
    }
}
