using EntityPlayground.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Derived
{
    public class BaseComponent : Component
    { }

    public class DurabilityComponent: Component
    {
        public virtual int Durability { get; set; }
    }

    public class DescriptionComponent: Component
    {
        public virtual string Description { get; set; }
    }
}
