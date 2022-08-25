using EntityPlayground.DataLayer.Derived;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Models
{
    public enum  ConnectionType
    {
        Base,
        Inventory,
        Recipe
    }
    public abstract class Connection
    {
        public virtual ConnectionType Type { get; set; }   
        public virtual Guid SourceId { get; set; }
        public virtual GameEntity Source { get; set; }

        public virtual Guid TargetId { get; set; }
        public virtual GameEntity Target { get; set; }

        public virtual int Quantity { get; set; } = 1;
    }

    public class ConnectionTypeConfiguration : IEntityTypeConfiguration<Connection>
    {
        public void Configure(EntityTypeBuilder<Connection> builder)
        {
            builder
                .HasDiscriminator(c => c.Type)
                .HasValue<InventoryConnection>(ConnectionType.Inventory)
                .HasValue<BaseConnection>(ConnectionType.Base)
                .HasValue<RecipeConnection>(ConnectionType.Recipe);

            builder.
                HasKey(c => new { c.SourceId, c.TargetId });
        }
    }
}
