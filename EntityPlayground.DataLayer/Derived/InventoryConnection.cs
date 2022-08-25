using EntityPlayground.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Derived
{
    public class BaseConnection : Connection
    { }

    public class InventoryConnection : Connection
    {
        public virtual byte Slot { get; set; }
    }

    public class RecipeConnection: Connection
    {
        public virtual Guid ComponentTemplateEntityId { get; set; }
    }
    public class TestConnection: Connection
    {
        public virtual int MyProperty { get; set; }
        
    }
}

