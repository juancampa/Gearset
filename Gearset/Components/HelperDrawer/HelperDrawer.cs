using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Draw geometry in 3D space
    /// </summary>
    public class HelperDrawer : Gear
    {
        #region Constructor
        public HelperDrawer()
            : base(new GearConfig())
        {
        }
        #endregion

        #region Vector3 Stuff (including variables)
        private KeyedCollection<Vector3VisualHelper> vector3Collection = new KeyedCollection<Vector3VisualHelper>();

        /// <summary>
        /// Show a new Vector3.
        /// </summary>
        public void ShowVector3(String key, Vector3 baseVector, Vector3 vector, Color color)
        {
            if (vector3Collection.ContainsKey(key))
                vector3Collection[key].Update(baseVector, vector, color);
            else
                vector3Collection.Set(key, new Vector3VisualHelper(baseVector, vector, color));
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal int ShowVector3(Vector3 baseVector, Vector3 vector, Color color)
        {
            return vector3Collection.Add(new Vector3VisualHelper(baseVector, vector, color));
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal void ShowVector3(int index, Vector3 baseVector, Vector3 vector, Color color)
        {
            vector3Collection[index].Update(baseVector, vector, color);
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal void DeleteVector3(int index)
        {
            vector3Collection.RemoveAt(index);
        }

        /// <summary>
        /// Deletes a vector3.
        /// </summary>
        /// <param name="key"></param>
        internal void DeleteVector3(String key)
        {
            vector3Collection.RemoveAt(key);
        }
        #endregion

        #region Transform Stuff (including variables)
        private KeyedCollection<MatrixVisualHelper> MatrixCollection = new KeyedCollection<MatrixVisualHelper>();

        /// <summary>
        /// Show a new Matrix.
        /// </summary>
        public void ShowTransform(String key, Matrix matrix)
        {
            if (MatrixCollection.ContainsKey(key))
                MatrixCollection[key].Update(matrix);
            else
                MatrixCollection.Set(key, new MatrixVisualHelper(matrix));
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal int ShowTransform(Matrix matrix)
        {
            return MatrixCollection.Add(new MatrixVisualHelper(matrix));
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal void ShowTransform(int index, Matrix matrix)
        {
            MatrixCollection[index].Update(matrix);
        }

        /// <summary>
        /// Used internally handled with indices for performance considerations
        /// </summary>
        internal void DeleteTransform(int index)
        {
            MatrixCollection.RemoveAt(index);
        }

        /// <summary>
        /// Deletes a Matrix.
        /// </summary>
        /// <param name="key"></param>
        internal void DeleteTransform(String key)
        {
            MatrixCollection.RemoveAt(key);
        }
        #endregion
    }
}
