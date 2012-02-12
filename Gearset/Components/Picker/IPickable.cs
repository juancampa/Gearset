using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    /// <summary>
    /// Implement to allow the console pick this object, Type T
    /// defines kind of Pickable Volume that will be intersected
    /// with the casted ray.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IPickable<T> : IPickable
    {
        /// <summary>
        /// The pickable Volume
        /// </summary>
        T PickableVolume { get; }

    }

    internal interface IPickable
    {
        /// <summary>
        /// This method will be called when the object has been
        /// picked with the mouse
        /// </summary>
        void Picked();
    }
}
