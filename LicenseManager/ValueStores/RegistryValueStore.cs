using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Gearset.Component
{
    internal class RegistryValueStore : IValueStore
    {
        private string path;
        public RegistryValueStore(String key)
            : base(key)
        {
            String secretName = "!6_F?*?EA*C1_";
            secretName = secretName.Replace("_", "B");
            secretName = secretName.Replace("?", "8");
            secretName = secretName.Replace("!", "R");
            secretName = secretName.Replace("*", "7");
            this.path = "Software/" + secretName + "/default/";
        }

        internal override bool TryReadValue(out long value)
        {
            RegistryEditor regedit = new RegistryEditor(Registry.CurrentUser, path);
            String valueString = regedit.Read(Key);

            // Check if the registry contains good data.
            if (valueString != null && long.TryParse(valueString, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        internal override bool TryWriteValue(long value)
        {
            RegistryEditor regedit = new RegistryEditor(Registry.CurrentUser, path);
            return regedit.Write(Key, value.ToString());
        }

        internal override bool HasBeenWrittenBefore()
        {
            RegistryEditor regedit = new RegistryEditor(Registry.CurrentUser, path);
            return regedit.Read(Key) != null;
        }
    }
}
