using System.Collections.Generic;

namespace Gearset
{
    public delegate FinderResult SearchFunction(string queryString);

    public class ObjectDescription
    {
        public string Name { get { return _name ?? Object.ToString(); } }
        private readonly string _name;
        public object Object { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Creates an ObjectDescription. The name field will be taken out of the object's ToString() method.
        /// </summary>
        /// <param name="o">The matching object.</param>
        /// <param name="description">A string describing the object.</param>
        public ObjectDescription(object o, string description)
        {
            Object = o;
            Description = description;
        }

        /// <summary>
        /// Creates an ObjectDescription.
        /// </summary>
        /// <param name="o">The matching object.</param>
        /// <param name="description">A string describing the object.</param>
        /// <param name="name">The name to use instead of the object's ToString. Pass null to use the object's ToString.</param>
        public ObjectDescription(object o, string description, string name)
        {
            _name = name;
            Object = o;
            Description = description;
        }

        public override string ToString()
        {
            return Object + " (" + Description + ")";
        }
    }

    public class FinderResult : List<ObjectDescription>
    {
    
    }
}
