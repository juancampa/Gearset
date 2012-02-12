using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Gearset.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KeyboardComponent : Gear
    {
        private KeyboardState prevState;
        private KeyboardState state;

        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        public KeyboardState State { get { return state; } }

        public KeyboardComponent()
            : base(new GearConfig())
        {
        }

        public override void Update(GameTime gameTime)
        {
            prevState = state;
            state = Keyboard.GetState();
        }


        internal bool IsKeyJustDown(Keys key)
        {
            return (state.IsKeyDown(key) && prevState.IsKeyUp(key));
        }

        internal bool IsKeyDown(Keys key)
        {
            return (state.IsKeyDown(key));
        }
    }
}