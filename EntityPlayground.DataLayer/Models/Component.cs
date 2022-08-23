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
    public enum ComponentType
    {
        Base,
        Durability,
        Description
    }
    public abstract class Component
    {
        public Guid ComponentId { get; set; }
        public ComponentType Type { get; set; }

        public ICollection<GameEntity> Entities { get; set; }
        public List<EntityComponent> EntityLinks { get; set; }
    }

    public class ComponentTypeConfiguration : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder
                .HasKey(x => x.ComponentId);

            builder
                .HasDiscriminator(c => c.Type)
                .HasValue<BaseComponent>(ComponentType.Base)
                .HasValue<DurabilityComponent>(ComponentType.Durability)
                .HasValue<DescriptionComponent>(ComponentType.Description);

            //builder
            //    .Property(c => c.ComponentId)
            //    .ValueGeneratedNever();
        }
    }
}
