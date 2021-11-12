using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;

namespace iWalletDemo.Core.Models
{
    public class WalletItemModel
    {
        public string Name { get; set; } // custom name set by the user for identification purposes

        public string HolderName { get; set; }

        public string ExpirationDate { get; set; }

        public string Organization { get; set; }

        public string ID { get; set; }

        public WalletItemModel(string name)
        {
            Name = name;
        }
    }
}
