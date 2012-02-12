using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace Gearset.Components.Persistor
{
    [Serializable]
    internal class ValueCollection : Dictionary<string, Dictionary<string, Object>>
    {
        public ValueCollection()
        {
        }

        public ValueCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
