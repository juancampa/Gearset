using System;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    public class StreamItem : BindableBase
    {
        public String Name { get; set; }

        Boolean _enabled = true;
        public Boolean Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        public Brush Color { get; set; }
    }
}
