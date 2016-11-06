using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra_Guidebook
{
    class ParentRegion :  BaseRegionDirectory
    {
        public ParentRegion(string name) : base(name)
        {
           
        }

        public bool ContainsChildRegion(string name)
        {
            ChildRegion childRegion = new ChildRegion(name);
            return ContainsChild(childRegion);
        }

        public ChildRegion GetChildRegion(string name)
        {
            ChildRegion childRegion = new ChildRegion(name);
            return GetChild(childRegion) as ChildRegion;
        }
    }
}
