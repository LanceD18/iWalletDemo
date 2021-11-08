using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Wpf.Core;
using Serilog;
using Serilog.Extensions.Logging;

namespace iWalletDemo.WPF
{
    public class Setup : MvxWpfSetup<Core.App>
    {
        //IoCProvider initialization
        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
        {
            //base.InitializeFirstChance(iocProvider);
            //? Not needed until we need to send references from .WPF back to .Core
        }

        protected override ILoggerProvider CreateLogProvider()
        {
            return new SerilogLoggerProvider();
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            // serilog configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            return new SerilogLoggerFactory();
        }
    }
}
