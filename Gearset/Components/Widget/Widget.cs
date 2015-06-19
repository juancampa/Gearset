using System;
using Gearset.UserInterface;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Widget
{
    public class Widget : Gear
    {
        readonly IUserInterface _userInterface;
    
        private int _initialPositionSetDelay = 3;

        public Widget(IUserInterface userInterface)
            : base(new GearConfig())
        {
            _userInterface = userInterface;
            _userInterface.CreateWidget();

            //TODO: can this move to UserInterface
            if (GearsetResources.GameWindow != null)
            {
                GearsetResources.GameWindow.Move += GameWindowMove;
                GearsetResources.GameWindow.VisibleChanged += (sender, args) => _userInterface.WidgetVisible = GearsetResources.GameWindow.Visible;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_initialPositionSetDelay > 0)
            {
                _initialPositionSetDelay--;
                GameWindowMove(this, null);
            }
            base.Update(gameTime);
        }

        void GameWindowMove(object sender, EventArgs e)
        {
            //TODO: can this move to UserInterface
			if (GearsetResources.GameWindow != null) 
			{
				_userInterface.MoveTo (GearsetResources.GameWindow.Left, GearsetResources.GameWindow.Top);
			}
        }

        public void AddAction(string name, Action action)
        {
            _userInterface.AddAction(name, action);
        }
    }
}
