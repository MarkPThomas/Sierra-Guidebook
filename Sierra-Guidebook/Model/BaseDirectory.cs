using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra_Guidebook
{
    abstract class BaseDirectory : TreeNode
    {
        public BaseDirectory(string name) : base(name.Replace('"', '\''))
        {
            AddChild(new TreeNode("_Research"));
            AddChild(new TreeNode("_Development"));
            AddChild(new TreeNode("_Final"));
        }
    }
}
