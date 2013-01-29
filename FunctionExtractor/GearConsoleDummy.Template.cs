using System;
using Microsoft.Xna.Framework;

namespace Gearset
{
    /// <summary>
    /// This is a dummy class, it does not have any Gearset functionality. It's a dummy
    /// version of Gearset for Windows Phone and Xbox. Use the FunctionExtractor to autogenerate it.
    /// </summary>
    public class GearConsole
    {
        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public bool VisibleOverlays { get; set; }
        public Matrix Transform2D { get; set; }
        public float BenderNeedlePosition { get { return 0; } }
        public bool Initialized { get; private set; }

        public GearConsole(Game game) { }
        public void Initialize() { Initialized = true; }
        public void Update(GameTime gameTime) { }
        public void Draw(GameTime gameTime) { }
        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection) { }

        #region Wrappers for Gearset methods
// FUNCTION WRAPPERS PLACEHOLDER
        #endregion
    }
}

