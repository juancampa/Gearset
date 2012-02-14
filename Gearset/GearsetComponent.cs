using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Gearset
{
    /// <summary>
    /// Provides a simple way to add Gearset to your game. Simply
    /// add this component to your Game's Component collection and
    /// you're set. (Additionally you have to add the [STAThread]
    /// attribute to your Main(string[] args) method (usually in
    /// program.cs)
    /// </summary>
    class GearsetComponent : DrawableGameComponent
    {
        public GearConsole Console { get; private set; }

        public GearsetComponent(Game game)
            : base(game)
        {
            this.UpdateOrder = int.MaxValue - 1;
        }

        public override void Initialize()
        {
            Console = new GearConsole(this.Game);
            Console.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Console.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Console.Draw(gameTime);
        }
    }
}
