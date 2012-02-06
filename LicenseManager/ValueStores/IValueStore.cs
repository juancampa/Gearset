using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Component
{
    internal abstract class IValueStore
    {
        protected String Key { get; private set; }

        public IValueStore(String key)
        {
            this.Key = key;
        }

        internal abstract bool TryWriteValue(long value);

        /// <summary>
        /// Returns true if the value was successfully read.
        /// </summary>
        internal abstract bool TryReadValue(out long value);

        /// <summary>
        /// Returns true if it looks that a value has been written before
        /// </summary>
        /// <returns></returns>
        internal abstract bool HasBeenWrittenBefore();
    }
}
