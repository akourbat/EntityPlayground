using EntityPlayground.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityPlayground.DataLayer.GameContext
{
    public class GameContext: DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        { }

        public DbSet<GameEntity> GameEntities { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<EntityComponent> ComponentLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameEntityTypeConfiguration).Assembly);

            var connectionTypes = typeof(Connection).Assembly.GetTypes()
                .Where(t => t != typeof(Connection) && typeof(Connection).IsAssignableFrom(t));
            foreach (var type in connectionTypes)
            {
                modelBuilder.Entity(type).HasBaseType(type.BaseType);
            }

            var componentTypes = typeof(Component).Assembly.GetTypes()
                .Where(t => t != typeof(Component) && typeof(Component).IsAssignableFrom(t));
            foreach (var type in componentTypes)
            {
                modelBuilder.Entity(type).HasBaseType(type.BaseType);
            }

        }

    }
}
 