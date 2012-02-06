using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
#if WINDOWS_PHONE
    /// <summary>
    /// This is a placeholder class used in .NETCF because serializable
    /// is not available and we don't want to wrap all [Serializable]
    /// in #if WINDOWS.
    /// </summary>
    public class SerializableAttribute : Attribute
    {
    }

    public class NonSerializedAttribute: Attribute
    {
    }
#endif
}
