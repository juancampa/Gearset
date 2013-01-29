using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms.Integration;
using System.Windows;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Lets the user search for a object in its game.
    /// </summary>
    class Finder : Gear
    {
        private float searchDelay;
        private bool locationJustChanged;
        /// <summary>
        /// WPF window instance.
        /// </summary>
        internal FinderWindow Window { get; private set; }

        public FinderConfig Config { get; private set; }

        public Finder()
            : base(GearsetSettings.Instance.FinderConfig)
        {
            Config = GearsetSettings.Instance.FinderConfig;

            Window = new FinderWindow();
            ElementHost.EnableModelessKeyboardInterop(Window);
            Window.SizeChanged += new System.Windows.SizeChangedEventHandler(Window_SizeChanged);
            Window.IsVisibleChanged += new DependencyPropertyChangedEventHandler(Window_IsVisibleChanged);
            Window.LocationChanged += new EventHandler(Window_LocationChanged);
            Window.DataContext = this;
            Window.Top = Config.Top;
            Window.Left = Config.Left;
            Window.Width = Config.Width;
            Window.Height = Config.Height;

            WindowHelper.EnsureOnScreen(Window);

            if (Config.Visible)
                Window.Show();

            Config.SearchTextChanged += new EventHandler(Config_SearchTextChanged);
            searchDelay = .25f;
        }

        void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Config.Visible = Window.IsVisible;
        }

        protected override void OnVisibleChanged()
        {
            if (Window != null)
            {
                Window.Visibility = Visible ? Visibility.Visible : Visibility.Hidden;
                Window.WasHiddenByGameMinimize = false;
            }
        }

        void Window_LocationChanged(object sender, EventArgs e)
        {
            locationJustChanged = true;
        }

        void Window_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            locationJustChanged = true;
        }

        void Config_SearchTextChanged(object sender, EventArgs e)
        {
            searchDelay = .25f;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (searchDelay > 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                searchDelay -= dt;
                if (searchDelay <= 0 && Config.SearchText != null)
                {
                    searchDelay = 0;
                    if (Config.SearchFunction != null)
                        Window.ResultsListBox.ItemsSource = Config.SearchFunction(Config.SearchText);
                    else
                        Window.ResultsListBox.ItemsSource = DefaultSearchFunction(Config.SearchText);

                    if (Window.ResultsListBox.Items.Count > 0)
                        Window.ResultsListBox.SelectedIndex = 0;
                }
            }

            if (locationJustChanged)
            {
                locationJustChanged = false;
                Config.Top = Window.Top;
                Config.Left = Window.Left;
                Config.Width = Window.Width;
                Config.Height = Window.Height;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// The default search function. It will search through the GameComponentCollection
        /// of the Game.
        /// </summary>
        private FinderResult DefaultSearchFunction(String queryString)
        {
            FinderResult result = new FinderResult();
            String[] searchParams = queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Split the query
            if (searchParams.Length == 0)
            {
                searchParams = new String[] { String.Empty };
            }
            else
            {
                // Ignore case.
                for (int i = 0; i < searchParams.Length; i++)
                    searchParams[i] = searchParams[i].ToUpper();
            }

            foreach (IGameComponent component in GearsetResources.Game.Components)
            {
                bool matches = true;
                String type = component.GetType().ToString();

                if (component is GearsetComponentBase)
                    continue;

                // Check if it matches all search params.
                for (int i = 0; i < searchParams.Length; i++)
                {
                    if (!(component.ToString().ToUpper().Contains(searchParams[i]) || type.ToUpper().Contains(searchParams[i])))
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches)
                    result.Add(new ObjectDescription(component, type));
            }
            return result;
        }
    }
}
