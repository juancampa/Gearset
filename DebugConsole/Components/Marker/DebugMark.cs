using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset.Components
{
    class DebugMark
    {
        public bool ScreenSpace;
        public Vector2 ScreenSpacePosition;
        public Color color;
        public int remainingTime = -1;
        public VertexPositionColor[] mark;
        public Texture2D label;
        private MarkType type;

        public MarkType Type
        {
            get { return type; }
        }

        #region Constructors
        public DebugMark(Vector3 position, Color color, Texture2D label, bool screenSpace)
        {
            Initialize(position, color, -1, label, MarkType.Cross, screenSpace);
        }

        public DebugMark(Vector3 position, Color color, Texture2D label, MarkType type, bool screenSpace)
        {
            Initialize(position, color, -1, label, type, screenSpace);
        }

        private void Initialize(Vector3 position, Color color, int time, Texture2D label, MarkType type, bool screenSpace)
        {
            this.color = color;
            this.remainingTime = time;
            this.label = label;
            this.type = type;
            this.ScreenSpace = screenSpace;

            if (screenSpace) ScreenSpacePosition = new Vector2(position.X, position.Y);
            else
            {
                if (type == MarkType.Cross)
                {
                    mark = new VertexPositionColor[6];
                }
                else
                {
                    mark = new VertexPositionColor[1];
                }

                for (int i = 0; i < mark.Length; i++)
                    mark[i].Color = color;

                MoveTo(position);
            }
        }
        #endregion

        #region MoveTo
        public void MoveTo(Vector3 position)
        {
            if (ScreenSpace)
            {
                ScreenSpacePosition = new Vector2(position.X, position.Y);
            }
            else
            {
                for (int i = 0; i < mark.Length; i++)
                    mark[i].Position = position;

                if (this.type == MarkType.Cross)
                {
                    mark[0].Position += new Vector3(+0.1f, 0, 0);
                    mark[1].Position += new Vector3(-0.1f, 0, 0);
                    mark[2].Position += new Vector3(0, -0.01f, 0);
                    mark[3].Position += new Vector3(0, +0.01f, 0);
                    mark[4].Position += new Vector3(0, 0, -0.1f);
                    mark[5].Position += new Vector3(0, 0, +0.1f);
                }
            }
        }
        #endregion
    }
}

