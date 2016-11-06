using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sierra_Guidebook
{
    class Formation : BaseDirectory
    {
        public Formation(string name) : base(name)
        {

        }

        public bool ContainsRoute(string name)
        {
            Route route = new Route(name);
            return ContainsChild(route);
        }

        public Route GetRoute(string name)
        {
            Route route = new Route(name);
            return GetChild(route) as Route;
        }
    }
}
