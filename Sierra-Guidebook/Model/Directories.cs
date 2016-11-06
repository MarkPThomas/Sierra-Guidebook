using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra_Guidebook
{
    class Directories
    {
        internal const string RESEARCH = "_Research";
        internal const string DEVELOPMENT = "_Development";
        internal const string FINAL = "_Final";
        internal const string PHOTOGENIC = "_Photogenic";

        internal List<string> Standard { get; set; } = new List<string>();
        internal List<string> ParentAreaPaths { get; set; } = new List<string>();
        internal List<string> ChildAreaPaths { get; set; } = new List<string>();
        internal List<string> FormationPaths { get; set; } = new List<string>();
        internal List<string> RoutePaths { get; set; } = new List<string>();

        public Directories()
        {
            Standard.Add(RESEARCH);
            Standard.Add(DEVELOPMENT);
            Standard.Add(FINAL);
            Standard.Add(PHOTOGENIC);
        }
    }
}
