using System;
using System.Collections.Generic;
using System.Text;
using iWalletDemo.Core.Util;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace iWalletDemo.Core.Models
{
    public class NotificationModel : MvxNotifyPropertyChanged
    {
        public string Message { get; set; }

        // Commands
        public IMvxCommand RemoveNotificationCommand { get; set; }

        public NotificationModel(string message)
        {
            Message = message;

            RemoveNotificationCommand = new MvxCommand(RemoveNotification);
        }

        public void RemoveNotification()
        {
            WpfUtil.SignalNotificationRemoval?.Invoke(this);
        }
    }
}
