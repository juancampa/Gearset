using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gearset.Components
{
    /// <summary>
    /// Holds the information used to draw a Vector3 overlaid
    /// on the game.
    /// </summary>
    public class Vector3VisualHelper : VisualHelper
    {
        private int bodyLineIndex;
        private int[] headLineIndices = new int[4];

        public Vector3VisualHelper(Vector3 baseVector, Vector3 vector, Color color)
        {
            // Create the lines with whatever value we want, the update them.
            bodyLineIndex = GearsetResources.Console.LineDrawer.ShowLine(baseVector, vector, color);
            for (int i = 0; i < 4 ; i++)
            {
            	headLineIndices[i] = GearsetResources.Console.LineDrawer.ShowLine(baseVector, vector, color);
            }
            UpdateLines(baseVector, vector, color);
        }

        /// <summary>
        /// Call this method to update the visual representation of the vector.
        /// </summary>
        public void Update(Vector3 baseVector, Vector3 vector, Color color)
        {
            UpdateLines(baseVector, vector, color);
        }

        /// <summary>
        /// Update the lines direcly modifying the value
        /// stored in the DebugLines.
        /// </summary>
        private void UpdateLines(Vector3 v1, Vector3 v2, Color color)
        {
            // Difference vector
            Vector3 diff = v1 - v2;
            float distance = Vector3.Distance(v1, v2);

            diff *= 1 / distance;
            // Craft a vector that is normal to diff
            Vector3 normal1 = Vector3.Cross(diff, new Vector3(diff.Y, diff.X, diff.Z));
            if (normal1 == Vector3.Zero)
                normal1 = Vector3.Cross(diff, diff + Vector3.UnitY);
            // Craft a vector that is normal to both diff and normal1
            Vector3 normal2 = Vector3.Cross(diff, normal1);

            normal1 *= distance * 0.02f;
            normal2 *= distance * 0.02f;
            diff *= distance * 0.04f;
            GearsetResources.Console.LineDrawer.ShowLine(bodyLineIndex, v1, v2, color);
            GearsetResources.Console.LineDrawer.ShowLine(headLineIndices[0], v2 + normal1 + diff, v2, color);
            GearsetResources.Console.LineDrawer.ShowLine(headLineIndices[1], v2 - normal1 + diff, v2, color);
            GearsetResources.Console.LineDrawer.ShowLine(headLineIndices[2], v2 + normal2 + diff, v2, color);
            GearsetResources.Console.LineDrawer.ShowLine(headLineIndices[3], v2 - normal2 + diff, v2, color);
        }

        /// <summary>
        /// Remove all lines used.
        /// </summary>
        public override void CleanUp()
        {
            GearsetResources.Console.LineDrawer.DeleteLine3(bodyLineIndex);
            GearsetResources.Console.LineDrawer.DeleteLine3(headLineIndices[0]);
            GearsetResources.Console.LineDrawer.DeleteLine3(headLineIndices[1]);
            GearsetResources.Console.LineDrawer.DeleteLine3(headLineIndices[2]);
            GearsetResources.Console.LineDrawer.DeleteLine3(headLineIndices[3]);
        }

    }
}

