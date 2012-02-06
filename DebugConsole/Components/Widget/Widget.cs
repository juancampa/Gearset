using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Controls;
using Gearset.Components.InspectorWPF;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Gearset.Components
{
    internal class ActionItem
    {
        internal Action Action;
        internal String Name;
        public override string ToString()
        {
            return Name;
        }
    }

    public class Widget : Gear
    {
        private ObservableCollection<ActionItem> buttonActions;
        internal WidgetWindow Window { get; private set; }
        private int initialPositionSetDelay = 3;

        public Widget()
            : base(new GearConfig())
        {
            Window = new WidgetWindow();
            Window.DataContext = GearsetSettings.Instance;
            GearsetResources.GameWindow.Move += new EventHandler(GameWindow_Move);
            GearsetResources.GameWindow.GotFocus += new EventHandler(GameWindow_GotFocus);
            GearsetResources.GameWindow.VisibleChanged += new EventHandler(GameWindow_VisibleChanged);

            // Data bind to action buttons.
            buttonActions = new ObservableCollection<ActionItem>();
            Window.buttonList.ItemsSource = buttonActions;
        }

        public override void Update(GameTime gameTime)
        {
            if (initialPositionSetDelay > 0)
            {
                initialPositionSetDelay--;
                GameWindow_Move(this, null);
            }
            base.Update(gameTime);
        }

        void GameWindow_VisibleChanged(object sender, EventArgs e)
        {
            Window.Visibility = GearsetResources.GameWindow.Visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        void GameWindow_GotFocus(object sender, EventArgs e)
        {
            Window.Topmost = true;
            Window.Topmost = false;
        }

        void GameWindow_Move(object sender, EventArgs e)
        {
            System.Windows.Forms.Form form = GearsetResources.GameWindow;
            Window.Top = form.Top - Window.Height;
            Window.Left = GearsetResources.GameWindow.Left + 20;
        }

        public void AddAction(string name, Action action)
        {
            // Search for an action with that name.
            for (int i = 0; i < buttonActions.Count; i++)
            {
                if (buttonActions[i].Name == name)
                {
                    buttonActions[i].Action = action;
                    return;
                }
            }
            // New action
            if (name != null && action != null)
                buttonActions.Add(new ActionItem() { Name = name, Action = action });
        }
    }
}
