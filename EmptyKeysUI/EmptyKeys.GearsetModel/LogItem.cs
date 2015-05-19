using System;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    public class LogItem : BindableBase
    {
        public Brush Color { get; set; }
        public int UpdateNumber { get; set; }
        public StreamItem Stream { get; set; }
        public String Content { get; set; }
    }
}
