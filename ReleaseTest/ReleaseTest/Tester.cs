using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ReleaseTest
{
    public class Tester : DrawableGameComponent
    {
        public Tester(Game game)
            : base(game)
        {
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled == false)
                Debug.GS.ClearAll();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
