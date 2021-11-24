using System;
using System.Collections.Generic;
using System.Text;
using iWalletDemo.Core.External;
using iWalletDemo.Core.Models;
using MvvmCross;

namespace iWalletDemo.Core.Util
{
    public static class WpfUtil
    {
        public static Action<NotificationModel> SignalNotificationRemoval;

        public static IExternalTimer DebugRecommendationNotificationTimer = Mvx.IoCProvider.Resolve<IExternalTimer>();
    }
}
