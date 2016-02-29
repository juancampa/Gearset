using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Point = System.Drawing.Point;

namespace Gearset
{
    /// <summary>
    /// Represents a shim for GameWindow behaviour in projects that do not support retrieving the main game window handle.
    /// </summary>
    /// <remarks>
    /// MonoGame's Windows OpenGL implementation uses OpenTK to create the game window and this does not expose the 
    /// window handle required to retrieve a System.Windows.Form.
    /// </remarks>
    public partial class GameWindowMonitor : Form
    {
        readonly Game _game;

        readonly Timer _timer = new Timer();

        public GameWindowMonitor(Game game)
        {
            InitializeComponent();

            _game = game;
            _game.Activated += (sender, e) =>
            {
                Size = new Size(1,1);
                SetPosition();
            };

            _game.Deactivated += (sender, e) =>
            {
                SetPosition();
            };

            _game.Exiting += (sender, e) =>
            {
                _timer.Stop();
                _timer.Dispose();
            };

            _timer.Interval = 500;
            _timer.Start();
            _timer.Tick += (sender, e) => { SetPosition(); };
        }

        void SetPosition()
        {
#if FNA
            int x;
            int y;
            SDL2.SDL.SDL_GetWindowPosition(_game.Window.Handle, out x, out y);
            Location = new Point(x, y - 24);
#else
            Location = new Point(_game.Window.Position.X, _game.Window.Position.Y);
#endif
        }
    }
}
