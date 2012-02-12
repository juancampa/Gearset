using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Gearset.Components;
using System.ComponentModel;

namespace Gearset.Components
{
    /// <summary>
    /// A Gear's methods will be called by the GearConsole
    /// pretty much like a DrawableGameComponent gets called
    /// by XNA.
    /// </summary>
    public abstract class Gear
    {
        protected GearConfig gearConfig;

        public List<Gear> Children { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gear"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get { return gearConfig.Enabled; } }
            //set { gearConfig.Enabled = value; OnEnabledChanged(); } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gear"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible { get { return gearConfig.Visible; } }
            //set { gearConfig.Visible = value; OnVisibleChanged(); } }

        /// <summary>
        /// Gets the game.
        /// </summary>
        public Game Game { get { return GearsetResources.Game; } }

        public Gear(GearConfig config)
        {
            gearConfig = config;

            config.EnabledChanged += new EventHandler<BooleanChangedEventArgs>(config_EnabledChanged);
            config.VisibleChanged += new EventHandler<BooleanChangedEventArgs>(config_VisibleChanged);

            Children = new List<Gear>();
        }

        void config_VisibleChanged(object sender, BooleanChangedEventArgs e)
        {
            OnVisibleChanged();
        }

        void config_EnabledChanged(object sender, BooleanChangedEventArgs e)
        {
            OnEnabledChanged();
        }

        /// <summary>
        /// Called every frame so that the component can get updated.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) { }
        /// <summary>
        /// Called several times every frame, one for each render pass.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime) { }
        /// <summary>
        /// Called for every component when the game resolution changes.
        /// So that it can adjust things like drawing positions.
        /// </summary>
        public virtual void OnResolutionChanged() { }
        
        /// <summary>
        /// Override to implement functionality when the value
        /// of Enabled changes.
        /// </summary>
        protected virtual void OnEnabledChanged() { }

        /// <summary>
        /// Override to implement functionality when the value
        /// of Visible changes.
        /// </summary>
        protected virtual void OnVisibleChanged() { }

    }
}

