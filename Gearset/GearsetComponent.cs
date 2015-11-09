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
    public class GearsetComponent : GearsetComponentBase
    {
        public GearConsole Console { get; }

        public GearsetComponent(Game game, bool createUI)
            : base(game)
        {
            UpdateOrder = int.MaxValue - 1;
            Console = new GearConsole(Game, createUI);
        }

        public override void Initialize()
        {
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
