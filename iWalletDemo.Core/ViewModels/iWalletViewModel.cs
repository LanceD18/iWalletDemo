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

        public bool DebugRecommendationNotification { get; set; }

        public string NotificationTitle => Notifications.Count == 0 ? "Notifications" :  "Notifications " + "[" + Notifications.Count + "]";
        #endregion

        #endregion

        // These Enablers allow us to dynamically enable and disable controls depending on the given conditions
        #region Enablers
        public bool CanAddWalletItem => ActiveWalletItemName.Length > 0;

        public bool CanRemoveWalletItem => SelectedWalletItem != null;

        public bool CanSearchWalletItem => WalletItems.Count > 0;

        public bool CanSortWalletItems => WalletItems.Count > 0;
        #endregion

        // Commands are bound to controls within the .xaml to execute code on use of said controls
        #region Command Properties
        public IMvxCommand AddWalletItemCommand { get; set; }

        public IMvxCommand RemoveWalletItemCommand { get; set; }

        public IMvxCommand ToggleSortByNameCommand { get; set; }

        public IMvxCommand ToggleSortByDateCommand { get; set; }
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

            WpfUtil.SignalNotificationRemoval = (notification) => Notifications.Remove(notification);
            WpfUtil.DebugRecommendationNotificationTimer.Interval = TimeSpan.FromSeconds(5);
            WpfUtil.DebugRecommendationNotificationTimer.Tick += (sender, e) =>
            {
                if (!DisableNotifications)
                {
                    Notifications.Add(new NotificationModel("We have a new recommendation for you"));
                    RaisePropertyChanged(() => Notifications);
                }
            };

            // Updated the count given to the title on change
            Notifications.CollectionChanged += (sender, args) => RaisePropertyChanged(() => NotificationTitle);
        }

        #region Command Methods
        public void AddWalletItem()
        {
            WalletItems.Add(new WalletItemModel(ActiveWalletItemName));
            Debug.WriteLine(WalletItems.Count);

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
    }
}
