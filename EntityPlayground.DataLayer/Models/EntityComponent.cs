using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Models
{
    public class EntityComponent
    {
        public Guid GameEntityId { get; set; }
        public GameEntity GameEntity { get; set; }

        public Guid ComponentId { get; set; }
        public Component Component { get; set; }
    }

    public class EntityComponentTypeConfiguration : IEntityTypeConfiguration<EntityComponent>
    {
        public void Configure(EntityTypeBuilder<EntityComponent> builder)
        {
            builder
                .HasKey(ec => new {ec.GameEntityId, ec.ComponentId});

            builder
                .HasOne(ec => ec.GameEntity)
                .WithMany(e => e.ComponentLinks)
                .HasForeignKey(ec => ec.GameEntityId);

            builder
                .HasOne(ec => ec.Component)
                .WithMany(c => c.EntityLinks)
                .HasForeignKey(ec => ec.ComponentId);
        }
    }
}
