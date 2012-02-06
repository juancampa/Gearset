using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Holds the information used to draw a Vector3 overlaid
    /// on the game.
    /// </summary>
    public class MatrixVisualHelper : VisualHelper
    {
        // Indices of the Vectors.
        private int VectorX;
        private int VectorY;
        private int VectorZ;

        private static readonly float axisScale = 0.02f;


        public MatrixVisualHelper(Matrix matrix)
        {
            VectorX = GearsetResources.Console.HelperDrawer.ShowVector3(matrix.Translation, matrix.Right * axisScale, Color.Red);
            VectorY = GearsetResources.Console.HelperDrawer.ShowVector3(matrix.Translation, matrix.Up * axisScale, Color.Green);
            VectorZ = GearsetResources.Console.HelperDrawer.ShowVector3(matrix.Translation, matrix.Forward * axisScale, Color.Blue);
        }

        /// <summary>
        /// Call this method to update the visual representation.
        /// </summary>
        public void Update(Matrix matrix)
        {
            UpdateLines(matrix);
        }

        /// <summary>
        /// Update the lines direcly modifying the value
        /// stored in the DebugLines.
        /// </summary>
        private void UpdateLines(Matrix matrix)
        {
            GearsetResources.Console.HelperDrawer.ShowVector3(VectorX, matrix.Translation, matrix.Translation + matrix.Right * axisScale, Color.Red);
            GearsetResources.Console.HelperDrawer.ShowVector3(VectorY, matrix.Translation, matrix.Translation + matrix.Up * axisScale, Color.Green);
            GearsetResources.Console.HelperDrawer.ShowVector3(VectorZ, matrix.Translation, matrix.Translation + matrix.Forward * axisScale, Color.Blue);
        }

        /// <summary>
        /// Remove all lines used.
        /// </summary>
        public override void CleanUp()
        {
            GearsetResources.Console.HelperDrawer.DeleteVector3(VectorX);
            GearsetResources.Console.HelperDrawer.DeleteVector3(VectorY);
            GearsetResources.Console.HelperDrawer.DeleteVector3(VectorZ);
        }

    }
}

