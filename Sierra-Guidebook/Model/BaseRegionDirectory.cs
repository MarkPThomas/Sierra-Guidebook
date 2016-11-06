using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MPT.Trees;

namespace Sierra_Guidebook
{
    abstract class BaseRegionDirectory : BaseDirectory
    {
        public BaseRegionDirectory(string name) : base(name)
        {
            AddChild(new TreeNode("_Photogenic"));
        }
    }
}
