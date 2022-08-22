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
        public ConnectionType Type { get; set; }   
        public Guid SourceId { get; set; }
        public GameEntity Source { get; set; }

        public Guid TargetId { get; set; }
        public GameEntity Target { get; set; }

        public int Quantity { get; set; } = 1;
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
