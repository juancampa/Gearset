using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Gearset.Component
{
    /// <summary>
    /// Used to read/write data to the Windows registry.
    /// </summary>
    internal class RegistryEditor
    {
        /// <summary>
        /// BaseKey is usually HKLM, HKCU, etc
        /// </summary>
        internal RegistryKey BaseKey { get; private set; }

        /// <summary>
        /// Subkey is where you want to store your data
        /// </summary>
        internal String KeyPath { get; private set; }

        /// <summary>
        /// Creates a new RegistryEditor, baseKey is usually HKLM, HKCU, etc.
        /// subkey is where you want to store your data, list of '/' separated keys.
        /// </summary>
        internal RegistryEditor(RegistryKey baseKey, String keyPath)
        {
            BaseKey = baseKey;
            KeyPath = keyPath;
        }

        /// <summary>
        /// Returns the BaseKey/SubKey/keyName value or null if the key or path does not exists.
        /// </summary>
        internal string Read(string KeyName)
        {
            RegistryKey rk = BaseKey;
            RegistryKey subkey = GetReadKey(KeyPath);

            if (subkey == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)subkey.GetValue(KeyName);
                }
                catch
                {
                    return null;
                }
            }
        }

        private RegistryKey GetReadKey(String path)
        {
            RegistryKey key = BaseKey;
            String[] keys = path.Split('/');
            for (int i = 0; i < keys.Length; i++)
            {
                key = key.OpenSubKey(keys[i]);
                if (key == null)
                    return null;
            }

            return key;
        }

        private RegistryKey GetWriteKey(String path)
        {
            RegistryKey key = BaseKey;
            String[] keys = path.Split('/');
            for (int i = 0; i < keys.Length; i++)
            {
                key = key.CreateSubKey(keys[i]);
            }

            return key;
        }

        /// <summary>
        /// Writes to BaseKey/SubKey/keyName the given value value. Returns true on success.
        /// </summary>
        internal bool Write(string keyName, object Value)
        {
            try
            {
                RegistryKey rk = BaseKey;
                RegistryKey sk1 = GetWriteKey(KeyPath);

                // Save the value
                sk1.SetValue(keyName, Value);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
