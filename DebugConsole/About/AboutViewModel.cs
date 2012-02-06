using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Gearset.About
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        public string CopyrightNotice { get; set; }
        public string ProductNameAndVersion { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public AboutViewModel(String productNameAndVersion, string copyrightNotice)
        {
            this.ProductNameAndVersion = productNameAndVersion;
            this.CopyrightNotice = copyrightNotice;
        }
    }
}
