using System;
using System.Windows;
using System.Windows.Threading;
using iWalletDemo.Core.External;

namespace iWalletDemo.WPF.External
{
    public class ExternalTimer : IExternalTimer
    {
        private readonly DispatcherTimer internalTimer;

        public ExternalTimer()
        {
            internalTimer = new DispatcherTimer(DispatcherPriority.Background, Application.Current.Dispatcher);
        }

        public bool IsEnabled
        {
            get => internalTimer.IsEnabled;
            set => internalTimer.IsEnabled = value;
        }

        public TimeSpan Interval
        {
            get => internalTimer.Interval;
            set => internalTimer.Interval = value;
        }

        public void Start() => internalTimer.Start();

        public void Stop() => internalTimer.Stop();

        public event EventHandler Tick
        {
            add => internalTimer.Tick += value;
            remove => internalTimer.Tick -= value;
        }
    }
}