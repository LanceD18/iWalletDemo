using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AdonisUI.Controls;
using iWalletDemo.Core.Models;
using iWalletDemo.Core.Util;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Microsoft.VisualBasic;
using static System.String;

namespace iWalletDemo.Core.ViewModels
{
    /// <summary>
    /// An enum holding the different sorting types seen in the app
    /// </summary>
    public enum SortType
    {
        Name,
        Date
    }

    public class iWalletViewModel : MvxViewModel
    {
        #region View Variables

        #region Wallet Items
        // Wallet Items
        // TODO This likely no longer needs to be an Observable Collection now that VisibleWalletItems exists, remove later
        /// The full list of all existing wallet items
        private MvxObservableCollection<WalletItemModel> _walletItems = new MvxObservableCollection<WalletItemModel>();

        public MvxObservableCollection<WalletItemModel> WalletItems
        {
            get => _walletItems;
            set => SetProperty(ref _walletItems, value);
        }

        // Visible Wallet Items
        /// For use with searching & sorting
        /// This list determines what we see under the list of wallet items in the UI
        private MvxObservableCollection<WalletItemModel> _visibleWalletItems = new MvxObservableCollection<WalletItemModel>();

        public MvxObservableCollection<WalletItemModel> VisibleWalletItems
        {
            get => _visibleWalletItems;
            set => SetProperty(ref _visibleWalletItems, value);
        }

        // Active Wallet Item Name
        private string _activeWalletItemName = "";

        public string ActiveWalletItemName
        {
            get => _activeWalletItemName;
            set
            {
                SetProperty(ref _activeWalletItemName, value);
                RaisePropertyChanged(() => CanAddWalletItem);
            }
        }

        // Holds the object for the wallet item currently selected in the UI
        private WalletItemModel _selectedWalletItem;

        public WalletItemModel SelectedWalletItem
        {
            get { return _selectedWalletItem; }
            set
            {
                SetProperty(ref _selectedWalletItem, value);
                RaisePropertyChanged(() => CanRemoveWalletItem);
            }
        }

        // Search Filter
        private string _searchFilter = "";

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                SetProperty(ref _searchFilter, value);
                UpdateVisibleWalletItems();
            }
        }
        #endregion

        #region Notifications
        private MvxObservableCollection<NotificationModel> _notifications = new MvxObservableCollection<NotificationModel>();

        public MvxObservableCollection<NotificationModel> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);

        }

        public bool DisableNotifications { get; set; }

        public string NotificationTitle => Notifications.Count == 0 ? "Notifications" : "Notifications " + "[" + Notifications.Count + "]";
        #endregion

        #region Feedback

        private string _feedbackTitle;
        private string _feedbackDescription;
        private bool _submitToPublic;

        public string FeedbackTitle
        {
            get => _feedbackTitle;
            set
            {
                SetProperty(ref _feedbackTitle, value);
                RaisePropertyChanged(() => CanSubmitFeedback);
            }
        }

        public string FeedbackDescription
        {
            get => _feedbackDescription;
            set
            {
                SetProperty(ref _feedbackDescription, value);
                RaisePropertyChanged(() => CanSubmitFeedback);
            }
        }

        public bool SubmitToPublic
        {
            get => _submitToPublic;
            set => SetProperty(ref _submitToPublic, value);
        }

        #endregion

        #region Login
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                RaisePropertyChanged(() => CanSignIn);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                RaisePropertyChanged(() => CanSignIn);
            }
        }
        #endregion

        #endregion

        // These Enablers allow us to dynamically enable and disable controls depending on the given conditions
        #region Enablers
        public bool CanAddWalletItem => ActiveWalletItemName.Length > 0;

        public bool CanRemoveWalletItem => SelectedWalletItem != null;

        public bool CanSearchWalletItem => WalletItems.Count > 0;

        public bool CanSortWalletItems => WalletItems.Count > 0;

        public bool CanSubmitFeedback => FeedbackTitle != null && FeedbackDescription != null && FeedbackTitle != Empty && FeedbackDescription != Empty;

        public bool SignedIn { get; set; } = false;

        public bool CanSignIn => Username != null && Password != null && Username != Empty && Password != Empty && !SignedIn;
        #endregion

        // Commands are bound to controls within the .xaml to execute code on use of said controls
        #region Command Properties
        public IMvxCommand AddWalletItemCommand { get; set; }

        public IMvxCommand RemoveWalletItemCommand { get; set; }

        public IMvxCommand ToggleSortByNameCommand { get; set; }

        public IMvxCommand ToggleSortByDateCommand { get; set; }

        public IMvxCommand SubmitFeedbackCommand { get; set; }

        public IMvxCommand SignInCommand { get; set; }

        public IMvxCommand CustomerSupportCommand { get; set; } = new MvxCommand(() => {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://replit.com/@LanceD18/Team-5-CustomerSupport?v=1",
                UseShellExecute = true
            });
        });

        public IMvxCommand RecommendationEngineCommand { get; set; } = new MvxCommand(() => {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://colab.research.google.com/drive/1jpkkSIZ5F5-judmcN7uPaE7xtB6DOGMK#scrollTo=5ccecfe4",
                UseShellExecute = true
            });
        });

        public IMvxCommand VirtualWalletCommand { get; set; } = new MvxCommand(() =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://replit.com/@horuychiv7/Team-5-items-code",
                UseShellExecute = true
            });
        });
        #endregion

        #region Sort
        // true = normal, false = reverse
        // SortByDate = true is the default, this is the default starting output of WalletItems 'sort' functions
        public bool SortByName { get; set; } = false;

        public bool SortByDate { get; set; } = true;

        public SortType ActiveSortType { get; set; } = SortType.Date;
        #endregion
        
        public iWalletViewModel()
        {
            // Linking Commands
            AddWalletItemCommand = new MvxCommand(AddWalletItem);
            RemoveWalletItemCommand = new MvxCommand(RemoveWalletItem);
            ToggleSortByNameCommand = new MvxCommand(ToggleSortByName);
            ToggleSortByDateCommand = new MvxCommand(ToggleSortByDate);
            SubmitFeedbackCommand = new MvxCommand(SubmitFeedback);
            SignInCommand = new MvxCommand(() =>
            {
                SignedIn = true;
                RaisePropertyChanged(() => SignedIn);
                RaisePropertyChanged(() => CanSignIn);
            });

            WpfUtil.SignalNotificationRemoval = (notification) => Notifications.Remove(notification);
            WpfUtil.DebugRecommendationNotificationTimer.Interval = TimeSpan.FromSeconds(5);
            WpfUtil.DebugRecommendationNotificationTimer.Tick += (sender, e) => AddNotification("We have a new recommendation for you");

            // Updated the count given to the title on change
            Notifications.CollectionChanged += (sender, args) => RaisePropertyChanged(() => NotificationTitle);
        }

        #region Command Methods
        public void AddWalletItem()
        {
            WalletItems.Add(new WalletItemModel(ActiveWalletItemName));

            AddNotification("New Virtual Wallet Item added [" + ActiveWalletItemName + "]");

            RaisePropertyChanged(() => CanSearchWalletItem);
            RaisePropertyChanged(() => CanSortWalletItems);

            ActiveWalletItemName = "";

            UpdateVisibleWalletItems();
        }

        /// <summary>
        /// Removes the Selected Wallet Item and updates relevant properties
        /// </summary>
        public void RemoveWalletItem()
        {
            WalletItems.Remove(SelectedWalletItem);

            RaisePropertyChanged(() => CanSearchWalletItem);
            RaisePropertyChanged(() => CanSortWalletItems);

            UpdateVisibleWalletItems();
        }

        // 
        /// <summary>
        /// Flips the sorting property of SortByName, while also setting every other sort type to false so that
        /// they'll be given their default state the next time they are toggled
        /// </summary>
        public void ToggleSortByName()
        {
            ActiveSortType = SortType.Name;
            SortByName = !SortByName;
            SortByDate = false;

            UpdateVisibleWalletItems();
        }

        /// <summary>
        /// Flips the sorting property of SortByDate, while also setting every other sort type to false so that
        /// they'll be given their default state the next time they are toggled
        /// </summary>
        public void ToggleSortByDate()
        {
            ActiveSortType = SortType.Date;
            SortByName = false;
            SortByDate = !SortByDate;

            UpdateVisibleWalletItems();
        }

        /// <summary>
        /// Submits feedback to the developers (and a public forum if the option is selected)
        /// Currently just a demo version, the information doesn't actually go anywhere but performs the actions that would impact the app
        /// </summary>
        public void SubmitFeedback()
        {
            string notificationString = "Feedback [" + FeedbackTitle + "] sent to developers";

            if (SubmitToPublic) notificationString += " and public forums";

            AddNotification(notificationString);

            FeedbackTitle = "";
            FeedbackDescription = "";
            SubmitToPublic = false;
        }
        #endregion

        /// <summary>
        /// Updates the Visible Wallet Item list to match given sort settings & filters
        /// </summary>
        public void UpdateVisibleWalletItems()
        {
            IEnumerable<WalletItemModel> sortedItems = null;
            switch (ActiveSortType)
            {
                case SortType.Name:
                    sortedItems = SortByName
                        ? (from f in WalletItems orderby f.Name select f) // ascending
                        : (from f in WalletItems orderby f.Name descending select f);
                    break;

                case SortType.Date: // only applies to the false version of this, which is reversed
                    sortedItems = SortByDate
                        ? WalletItems
                        : WalletItems.Reverse();
                    break;
            }

            if (SearchFilter == "" && ActiveSortType == SortType.Date && SortByDate) // no need to sort anything in this case, so no need to traverse through the array
            {
                VisibleWalletItems = WalletItems;
            }
            else
            {
                VisibleWalletItems = new MvxObservableCollection<WalletItemModel>();

                foreach (WalletItemModel walletItem in sortedItems)
                {
                    if (walletItem.Name.Contains(SearchFilter))
                    {
                        VisibleWalletItems.Add(walletItem);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new notification if notifications are enabled
        /// </summary>
        /// <param name="message"></param>
        public void AddNotification(string message)
        {
            if (!DisableNotifications)
            {
                Notifications.Add(new NotificationModel(message));
                RaisePropertyChanged(() => Notifications);
            }
        }
    }
}
