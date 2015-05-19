#if WPF
    using System;
    using System.ComponentModel;

    namespace Gearset.UserInterface.Wpf.About
    {
        public class AboutViewModel : INotifyPropertyChanged
        {
            public string CopyrightNotice { get; set; }
            public string ProductNameAndVersion { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged(String propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }


            public AboutViewModel(String productNameAndVersion, string copyrightNotice)
            {
                ProductNameAndVersion = productNameAndVersion;
                CopyrightNotice = copyrightNotice;
            }
        }
    }
#endif