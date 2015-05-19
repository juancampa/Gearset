using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Gearset.Components.Persistor
{
    /// <summary>
    /// Handles persistance of object members. Can store and load data
    /// from/to an object as when it is being initialized. With this
    /// component variables can be edited, stored and reloaded.
    /// This class is not threadsafe as it uses the same StringBuilder
    /// </summary>
    internal class Persistor
    {
        /// <summary>
        /// The data stored by the dictionary it maps the an id
        /// with a dictionary of stored values for that id.
        /// </summary>
        
        private ValueCollection data;

        /// <summary>
        /// One instance for all calls to String.Split. Avoids
        /// garbage generation.
        /// </summary>
        private static char[] commaDelimiter = new char[] { ',' };

        public string CurrentSettingsName { get; set; }

        /// <summary>
        /// Constructor, will load everything.
        /// </summary>
        public Persistor()
        {
            Load("default");
        }

        /// <summary>
        /// Loads a pre-saved Persistor data set.
        /// <see>Save</see>
        /// </summary>
        private void Load(String SettingsName)
        {
            CurrentSettingsName = SettingsName;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                FileStream file = new FileStream(SettingsName + ".PersistorData", FileMode.Open);
                data = (ValueCollection)formatter.Deserialize(file);
                file.Close();
            }
            catch
            {
                GearsetResources.Console.Log("Gearset", "No persistor data file found, creating a fresh data set.");
                data = new ValueCollection();
            }
        }

        /// <summary>
        /// Saves the current state of the configuration with a settings name.
        /// This allows to create several settings, useful for example to debug
        /// different components of your game.
        /// </summary>
        /// <param name="SettingsName"></param>
        public void Save(String SettingsName)
        {
#if WINDOWS || LINUX || MONOMAC
            CurrentSettingsName = SettingsName;
            //try
            //{
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = new FileStream(SettingsName + ".PersistorData", FileMode.Create);
            //XmlSerializer serializer = new XmlSerializer(configuration.GetType());
            //serializer.Serialize(file, configuration);
            formatter.Serialize(file, data);
            file.Close();
            //}
            //catch (Exception e)
            //{
            //    GearsetResources.Console.Log("Gearset", "The DebugConsole configuration could not be saved: " + e.Message);
            //}
#endif
        }

        /// <summary>
        /// Initializes fields and properties of the object o, with
        /// data of the specified category and of the specified id.
        /// </summary>
        /// <param name="o">Object to get its fields initialized.</param>
        /// <param name="type">The category this object belongs, all objects
        /// of the same category will get the same values.</param>
        /// <param name="id">The id of the object which identifies it inside
        /// its category, it </param>
        public void InitializeObject(IPersistent o)
        {
            Type t = o.GetType();
            String[] ids = o.Ids.Split(commaDelimiter, StringSplitOptions.RemoveEmptyEntries);

            // Hey, lock here, there might be other threads
            // trying to initialize their values also.
            lock (data)
            {
                foreach (String id in ids)
                {
                    if (!data.ContainsKey(id))   // Do we have something for this id?
                        continue;

                    // Iterate over what we have for this id
                    foreach (var pair in data[id])  
                    {
                        // Does o has a member we can write to?
                        MemberInfo[] members = t.GetMember(pair.Key);
                        foreach (var info in members)
                        {
                            if (info is FieldInfo)
                            {
                                TrySetValue(o, (FieldInfo)info, data[id][info.Name]);
                            }
                            else if (info is FieldInfo)
                            {
                                TrySetValue(o, (PropertyInfo)info, data[id][info.Name]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of the field defined by info on the object o
        /// to the value value. If the fields aren't of the same type
        /// it will be silently ignored.
        /// </summary>
        private void TrySetValue(IPersistent o, FieldInfo info, Object value)
        {
            if (value.GetType() == info.FieldType)
                info.SetValue(o, value);
        }

        /// <summary>
        /// Sets the value of the property defined by info on the object o
        /// to the value value. If the fields aren't of the same type
        /// it will be silently ignored.
        /// </summary>
        private void TrySetValue(IPersistent o, PropertyInfo info, Object value)
        {
            if (value.GetType() == info.PropertyType)
                info.SetValue(o, value, null);
        }

        /// <summary>
        /// Call this method to save a value that will be later initialized
        /// to the object o.
        /// </summary>
        /// <param name="id">The id to which the value will be saved.</param>
        /// <param name="info">The field or property info to be saved.</param>
        /// <param name="value">The value to save for the specified field.</param>
        public void SaveValue<T>(String id, String memberName, T value)
        {
            lock (data)
            {
                Dictionary<string, object> idData;
                if (data.ContainsKey(id))
                    idData = data[id];
                else
                {
                    idData = new Dictionary<string, object>();
                    data.Add(id, idData);
                }

                idData[memberName] = value;
            }
        }

    }
}
