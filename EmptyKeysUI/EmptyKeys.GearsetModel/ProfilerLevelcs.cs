using System;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    public class ProfilerLevel : BindableBase
    {
        public int LevelId { get; private set; }
        public String Name { get; set; }

        Boolean _enabled = true;
        public Boolean Enabled 
        { 
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        public ProfilerLevel(int levelId)
        {
            LevelId = levelId;
        }
    }
}
