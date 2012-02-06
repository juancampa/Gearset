using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Gearset.Component
{
    internal class AdsValueStore : IValueStore
    {
        private string filename;
        public AdsValueStore(String key)
            : base(key)
        {
            // Store on App Data the same date for redundancy.
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"The Complot\"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"The Complot\Gearset"));
            filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"The Complot\Gearset\default.conf");
        }

        internal override bool TryReadValue(out long value)
        {
            try
            {
                IntPtr fileIntPtr = NativeMethods.CreateFileW(filename + ":" + Key, NativeConstants.GENERIC_READ, NativeConstants.FILE_SHARE_WRITE, IntPtr.Zero, NativeConstants.OPEN_ALWAYS, 0, IntPtr.Zero);
                if (fileIntPtr.ToInt32() <= 0)
                {
                    value = 0;
                    return false;
                }

                SafeFileHandle streamHandle = new SafeFileHandle(fileIntPtr, true);
                using (BinaryReader reader = new BinaryReader(new FileStream(streamHandle, FileAccess.Read)))
                {
                    value = reader.ReadInt64();
                }
                streamHandle.Close();

                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        internal override bool TryWriteValue(long value)
        {
            try
            {
                IntPtr fileIntPtr = NativeMethods.CreateFileW(filename + ":" + Key, NativeConstants.GENERIC_WRITE, NativeConstants.FILE_SHARE_WRITE, IntPtr.Zero, NativeConstants.OPEN_ALWAYS, 0, IntPtr.Zero);
                if (fileIntPtr.ToInt32() <= 0)
                    return false;

                SafeFileHandle streamHandle = new SafeFileHandle(fileIntPtr, true);
                using (BinaryWriter writer = new BinaryWriter(new FileStream(streamHandle, FileAccess.Write)))
                {
                    writer.Write(value);
                }
                streamHandle.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal override bool HasBeenWrittenBefore()
        {
            return File.Exists(filename);
        }
    }
}
