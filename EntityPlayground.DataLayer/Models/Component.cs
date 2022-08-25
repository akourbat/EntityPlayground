using EntityPlayground.DataLayer.Derived;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public virtual Guid ComponentId { get; set; }
        public virtual ComponentType Type { get; set; }

        public virtual ICollection<GameEntity> Entities { get; set; } = new ObservableCollection<GameEntity>();
        public virtual IList<EntityComponent> EntityLinks { get; set; } = new ObservableCollection<EntityComponent>();
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
