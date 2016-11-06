using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra_Guidebook
{
    class ChildRegion : BaseRegionDirectory
    {
        public ChildRegion(string name) : base(name)
        {

        }

        public bool ContainsFormation(string name)
        {
            Formation formation = new Formation(name);
            return ContainsChild(formation);
        }

        public Formation GetFormation(string name)
        {
            Formation formation = new Formation(name);
            return GetChild(formation) as Formation;
        }
    }
}
