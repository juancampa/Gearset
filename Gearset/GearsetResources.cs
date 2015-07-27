using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Gearset.Components;

using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

#if MONOMAC
namespace Microsoft.Xna.Framework.Content
{
	public class ResourceContentManager : ContentManager
	{
		private ResourceManager resource;

		public ResourceContentManager(IServiceProvider servicesProvider, ResourceManager resource)
			: base(servicesProvider)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}
			this.resource = resource;
		}

		protected override System.IO.Stream OpenStream(string assetName)
		{
			object obj = this.resource.GetObject(assetName);
			if (obj == null)
			{
				throw new ContentLoadException("Resource not found");
			}
			if (!(obj is byte[]))
			{
				throw new ContentLoadException("Resource is not in binary format");
			}
			return new MemoryStream(obj as byte[]);
		}
	}
}
#endif

namespace Gearset
{

    internal static class GearsetResources
    {
        internal static SpriteBatch SpriteBatch;
        internal static BasicEffect Effect;
        internal static BasicEffect Effect2D;

        internal static RenderPass CurrentRenderPass;

        internal static SpriteFont Font;
        internal static SpriteFont FontTiny;
        internal static SpriteFont FontAlert;
		internal static ResourceContentManager Content;
        internal static GraphicsDevice Device { get { return Game.GraphicsDevice; } }
        internal static GearConsole Console;

        internal static float GlobalAlpha;
        

        // Here's the data that is provided by the game.
        #region Game specific data
        public static Game Game { get; internal set; }
        internal static Matrix World;
        internal static Matrix View;
        internal static Matrix Projection;
        internal static Matrix Transform2D;
        #endregion

        internal static MouseComponent Mouse;
        internal static KeyboardComponent Keyboard;
        #if WINDOWS || LINUX || MONOMAC
            internal static System.Windows.Forms.Form GameWindow;
        #endif
    }

    /// <summary>
    /// Every DebugComponent will be called (Draw())
    /// for each of these modes, so they can draw
    /// when they need to.
    /// </summary>
    internal enum RenderPass
    {
        /// <summary>
        /// DebugComponents should draw only using
        /// spriteBatch here (without Begin() or End())
        /// </summary>
        SpriteBatchPass,

        /// <summary>
        /// DebugComponents should draw using the 
        /// common effect, and without changing any 
        /// parameters from it.
        /// </summary>
        BasicEffectPass,

        /// <summary>
        /// DebugComponents can draw whatever they want
        /// in this pass, they can change the params
        /// of the spritebatch and the basiceffect or even
        /// use their own effects.
        /// </summary>
        CustomPass,

        /// <summary>
        /// Similar to BasicEffectPass but with matrices
        /// work with screen space coordinates.
        /// </summary>
        ScreenSpacePass,

        /// <summary>
        /// Similar to ScreenSpacePass but with matrices
        /// passed by the game, so it's a transformed
        /// screen space.
        /// </summary>
        GameSpacePass,
    }
}
