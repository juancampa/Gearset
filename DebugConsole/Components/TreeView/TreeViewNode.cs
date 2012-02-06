using System;
using System.Collections.Generic;
using System.Text;

namespace Gearset.Components
{
    public class TreeViewNode
    {
        public List<TreeViewNode> Nodes;
        public String Name;
        public String FilterName;
        public Object Value;
        public bool Open;

        public TreeViewNode(String name)
        {
            this.Name = name;
            this.FilterName = name.ToLower();
            this.Nodes = new List<TreeViewNode>();
            this.Open = false;
        }

        public void Toggle()
        {
            Open = !Open;
        }
    }
}
