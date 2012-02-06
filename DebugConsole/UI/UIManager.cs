using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.UI
{
    public static class UIManager
    {
        public static List<LayoutBox> Boxes { get; private set; }
        private static MouseRouter mouseRouter;

        static UIManager()
        {
            Boxes = new List<LayoutBox>();
            mouseRouter = new MouseRouter();
        }

        public static void Update(GameTime gameTime)
        {
            mouseRouter.Update();
        }
    }
}
