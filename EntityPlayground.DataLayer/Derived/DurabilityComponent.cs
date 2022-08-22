using EntityPlayground.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Derived
{
    public class DurabilityComponent: Component
    {
        public int Durability { get; set; }
    }

    public class DescriptionComponent: Component
    {
        public string Description { get; set; }
    }
}
