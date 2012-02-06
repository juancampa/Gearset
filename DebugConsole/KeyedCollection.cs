using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset
{
    public class KeyedCollection<T>
    {
        /// <summary>
        /// Dictionary that maps a key (string) to
        /// an index in the IList.
        /// </summary>
        private Dictionary<String, int> keyIndexTable;

        /// <summary>
        /// The list that holds the values.
        /// </summary>
        private IList<T> list;

        /// <summary>
        /// Returns a reference to the back-end list.
        /// </summary>
        public IList<T> List
        {
            get { return list; }
        }

        public int Count { get { return List.Count; } }

        /// <summary>
        /// Constructs a KeyCollection and uses the specified
        /// list to hold the values. The list must be empty and
        /// if it doesn't resize dynamically then it must have
        /// enough space to hold everything inserted.
        /// </summary>
        public KeyedCollection(IList<T> list)
        {
            if (list.Count > 0)
                throw new ArgumentException("Parameter is not valid, the list must be empty", "list");
            this.list = list;
            keyIndexTable = new Dictionary<string, int>();
        }

        /// <summary>
        /// Constructs a KeyedCollection with a List of T
        /// to holds the values.
        /// </summary>
        public KeyedCollection()
        {
            this.list = new List<T>();
            keyIndexTable = new Dictionary<string, int>();
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public T this[String index]
        {
            get
            {
                if (keyIndexTable.ContainsKey(index))
                    return list[keyIndexTable[index]];
                else
                    throw new KeyNotFoundException();
            }
        }

        public bool ContainsKey(String key) { return keyIndexTable.ContainsKey(key); }
        public int IndexOf(T value) { return List.IndexOf(value); }
        public void Insert(int index, T value) { throw new NotSupportedException("Use Set() instead"); }
        public void RemoveAt(int index) { List.RemoveAt(index); }
        public void RemoveAt(String key)
        {
            if (ContainsKey(key))
                List.RemoveAt(keyIndexTable[key]);
        }

        /// <summary>
        /// Adds a new value to the keyed collection. If the key
        /// already exist, the value will be changed.
        /// </summary>
        public int Set(String key, T value)
        {
            int index;
            if (ContainsKey(key))
            {
                index = keyIndexTable[key];
                list[index] = value;
            }
            else
            {
                index = list.Count;
                list.Add(value);
                keyIndexTable.Add(key, index);
            }
            return index;
        }

        /// <summary>
        /// Adds a new value to the keyed collection
        /// </summary>
        public void Set(int index, T value)
        {
            if (index >= list.Count)
            {
                throw new IndexOutOfRangeException("The index must be withing the list range.");
            }
            list[index] = value;
        }

        /// <summary>
        /// Adds a new value and returns its index.
        /// </summary>
        /// <param name="value"></param>
        public int Add(T value)
        {
            list.Add(value);
            return list.Count - 1;
        }

       
    }
}
