using System;
using Gearset.UserInterface;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Finder
{
    /// <summary>
    /// Lets the user search for objects in the game.
    /// </summary>
    public class Finder : Gear
    {
        float _searchDelay;

        public FinderConfig Config { get; private set; }

        readonly IUserInterface _userInterface;

        public Finder(IUserInterface userInterface)
            : base(GearsetSettings.Instance.FinderConfig)
        {
            Config = GearsetSettings.Instance.FinderConfig;

            _userInterface = userInterface;
            _userInterface.CreateFinder(this);

            Config.SearchTextChanged += (sender, args) => _searchDelay = .25f; 
            _searchDelay = .25f;
        }

        protected override void OnVisibleChanged()
        {
            _userInterface.FinderVisible = Visible;
        }

        public override void Update(GameTime gameTime)
        {
            if (_searchDelay > 0)
            {
                var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _searchDelay -= dt;
                if (_searchDelay <= 0 && Config.SearchText != null)
                {
                    _searchDelay = 0;
                    if (Config.SearchFunction != null)
                        _userInterface.FinderSearch(Config.SearchFunction(Config.SearchText));
                    else
                        _userInterface.FinderSearch(DefaultSearchFunction(Config.SearchText));
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// The default search function. It will search through the GameComponentCollection of the Game.
        /// </summary>
        static FinderResult DefaultSearchFunction(String queryString)
        {
            var result = new FinderResult();
            var searchParams = queryString.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Split the query
            if (searchParams.Length == 0)
            {
                searchParams = new [] { String.Empty };
            }
            else
            {
                // Ignore case.
                for (var i = 0; i < searchParams.Length; i++)
                    searchParams[i] = searchParams[i].ToUpper();
            }

            foreach (var component in GearsetResources.Game.Components)
            {
                var matches = true;
                var type = component.GetType().ToString();

                if (component is GearsetComponentBase)
                    continue;

                // Check if it matches all search params.
                foreach (var t in searchParams)
                {
                    if (component.ToString().ToUpper().Contains(t) || type.ToUpper().Contains(t)) 
                        continue;

                    matches = false;
                    break;
                }

                if (matches)
                    result.Add(new ObjectDescription(component, type));
            }
            return result;
        }
    }
}
