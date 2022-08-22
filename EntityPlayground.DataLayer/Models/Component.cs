using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Models
{
    public class Component
    {
        public Guid ComponentId { get; set; }
        public string Type { get; set; }
        public ICollection<EntityComponent> EntityLinks { get; set; }
    }

    public class ComponentTypeConfiguration : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder
                .HasKey(x => x.ComponentId);

            //builder
            //    .Property(c => c.ComponentId)
            //    .ValueGeneratedNever();
        }
    }
}
