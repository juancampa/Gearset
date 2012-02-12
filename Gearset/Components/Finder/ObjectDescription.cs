using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Gearset
{
    public delegate FinderResult SearchFunction(String queryString);

    public class ObjectDescription
    {
        public String Name { get { return name == null ? Object.ToString() : name; } }
        private String name;
        public Object Object { get; set; }
        public String Description { get; set; }

        /// <summary>
        /// Creates an ObjectDescription. The name field will be taken out
        /// of the object's ToString() method.
        /// </summary>
        /// <param name="o">The matching object.</param>
        /// <param name="description">A string describing the object.</param>
        public ObjectDescription(Object o, String description)
        {
            this.Object = o;
            this.Description = description;
        }

        /// <summary>
        /// Creates an ObjectDescription.
        /// </summary>
        /// <param name="o">The matching object.</param>
        /// <param name="description">A string describing the object.</param>
        /// <param name="name">The name to use instead of the object's ToString. Pass null to use the Object's ToString.</param>
        public ObjectDescription(Object o, String description, String name)
        {
            this.name = name;
            this.Object = o;
            this.Description = description;
        }

        public override string ToString()
        {
            return Object.ToString() + " (" + Description + ")";
        }
    }

    public class FinderResult : List<ObjectDescription>
    {
    
    }
}
