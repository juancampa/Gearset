using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ReleaseTest
{
    public class TesterLabels : Tester
    {
        float elapsed;

        public Vector3 TestVector3;

        public TesterLabels(Game game)
            : base(game)
        {
            TestVector3 = new Vector3(0, 4, 5);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsed += dt;

            Vector3 position3 = new Vector3((float)Math.Sin(elapsed), (float)Math.Cos(elapsed), 0);

            Debug.GS.ShowLabel("Label3D_NoText", position3);

            position3 += Vector3.UnitX * 2;
            Debug.GS.ShowLabel("Label3DT", position3, "Label3D_WithText");

            position3 += Vector3.UnitX * 2;
            Debug.GS.ShowLabel("Label3DTC", position3, "Label3D_WithTextColor", Color.Orange);


            Vector2 position2 = new Vector2(100 + (float)Math.Sin(elapsed) * 80, 100 + (float)Math.Cos(elapsed) * 80);

            Debug.GS.ShowLabel("Label2D_NoText", position2);

            position2 += Vector2.UnitX * 200;
            Debug.GS.ShowLabel("Label2DT", position2, "Label2D_WithText");

            position2 += Vector2.UnitX * 200;
            Debug.GS.ShowLabel("Label2DTC", position2, "Label2D_WithTextColor", Color.Red);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
