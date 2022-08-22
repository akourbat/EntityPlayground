using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.Models
{
    public class GameEntity
    {
        public Guid GameEntityId { get; set; }
        public ICollection<Connection> Inbound { get; set; }
        public ICollection<Connection> Outbound { get; set; }
        public ICollection<EntityComponent> ComponentLinks { get; set; }
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
        }
    }
}
