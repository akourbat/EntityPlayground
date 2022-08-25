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
    public class GameEntity
    {
        public virtual Guid GameEntityId { get; set; }
        public virtual ICollection<Connection> Inbound { get; set; } = new ObservableCollection<Connection>();
        public virtual ICollection<Connection> Outbound { get; set; } = new ObservableCollection<Connection>();

        public virtual ICollection<Component> Components { get; set; }= new ObservableCollection<Component>();
        public virtual IList<EntityComponent> ComponentLinks { get; set; } = new ObservableCollection<EntityComponent>();
    }

    public class GameEntityTypeConfiguration : IEntityTypeConfiguration<GameEntity>
    {
        public void Configure(EntityTypeBuilder<GameEntity> builder)
        {
            builder
                .HasKey(x => x.GameEntityId);

            builder
                .Property(e => e.GameEntityId)
                .ValueGeneratedNever();

            builder
                .HasMany<Connection>(e => e.Outbound)
                .WithOne(c => c.Source)
                .HasForeignKey(c => c.SourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany<Connection>(e => e.Inbound)
                .WithOne(c => c.Target)
                .HasForeignKey(c => c.TargetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Join table config
            builder
                .HasMany(e => e.Components)
                .WithMany(c => c.Entities)
                .UsingEntity<EntityComponent>(
                    ec => ec.HasOne(ec => ec.Component)
                            .WithMany(c => c.EntityLinks)
                            .HasForeignKey(ec => ec.ComponentId),
                    ec => ec.HasOne(ec => ec.GameEntity)
                            .WithMany(e => e.ComponentLinks)
                            .HasForeignKey(ec => ec.GameEntityId),
                    ec => ec.HasKey(ec => new { ec.GameEntityId, ec.ComponentId })
                 );
        }
    }
}
