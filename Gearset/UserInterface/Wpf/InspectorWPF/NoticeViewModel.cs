using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Gearset.Components.InspectorWPF
{
    public class NoticeViewModel : INotifyPropertyChanged
    {
        public String NoticeText { get { return noticeText; } set { noticeText = value; OnPropertyChanged("NoticeText"); } }
        private String noticeText;

        public String NoticeHyperlinkText { get { return noticeHyperlinkText; } set { noticeHyperlinkText = value; OnPropertyChanged("NoticeHyperlinkText"); } }
        private String noticeHyperlinkText;

        public String NoticeHyperlinkUrl { get { return noticeHyperlinkUrl; } set { noticeHyperlinkUrl = value; OnPropertyChanged("NoticeHyperlinkUrl"); } }
        private String noticeHyperlinkUrl;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
