using System.ComponentModel;
using System.Diagnostics;
using EntityPlayground.DataLayer.Derived;
using EntityPlayground.DataLayer.GameContext;
using EntityPlayground.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;

namespace EntityPlayground.DataTests
{
    public class GameContextTests
    {
        [Fact]
        public void RelationshipSetupTests()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<GameContext>(builder => builder.UseChangeTrackingProxies());

            using var ctx = new GameContext(options);
            ctx.Database.EnsureCreated();

            var entity1 = ctx.CreateProxy<GameEntity>();
            var entity2 = ctx.CreateProxy<GameEntity>();
            var entity3 = ctx.CreateProxy<GameEntity>();
            
            ctx.AddRange(entity1, entity2, entity3);
            ctx.SaveChanges();

            var g1 = entity1.GameEntityId;
            var g2 = entity2.GameEntityId;
            var g3 = entity3.GameEntityId;

            var connection1 = ctx.CreateProxy<InventoryConnection>(c => { c.Source = entity1; c.Target = entity2; c.Slot = 2; });
            var connection2 = ctx.CreateProxy<BaseConnection>(c => { c.Source = entity1; c.Target = entity3; });

            var component1 = ctx.CreateProxy<DurabilityComponent>(c => c.Durability = 55);
            var component2 = ctx.CreateProxy<DescriptionComponent>(c => c.Description = "Some text");
            var clink1 = ctx.CreateProxy<EntityComponent>(ec => {ec.GameEntity = entity1; ec.Component = component1;});
            var clink2 = ctx.CreateProxy<EntityComponent>(ec => {ec.GameEntity = entity1; ec.Component = component2;});

            ctx.AddRange(connection1, connection2, component1, component2, clink1, clink2);
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();

            var inventoryConnectionsCount = ctx.GameEntities
                .Include(e => e.Outbound.Where(c => c is InventoryConnection))
                .Single(e => e.GameEntityId == g1)
                .Outbound.Count();
                
            ctx.ChangeTracker.Clear();

            var sa = ctx.GameEntities.Find(g1);
            ctx.Entry(sa).Collection(e => e.Outbound).Load();
            var totalConnectionsCount = sa.Outbound.Count();

            ctx.ChangeTracker.Clear();

            var entityCount = ctx.GameEntities.Count();

            var directConnectionsCount = ctx.Connections.Count(c => c.SourceId == g1);

            var directInventoryConCount = ctx.Set<Connection>().OfType<InventoryConnection>().Count(c => c.SourceId == g1);

            ctx.ChangeTracker.Clear();

            var invCon = ctx.Set<Connection>().AsNoTracking().OfType<InventoryConnection>().Single(c => c.SourceId == g1);
            ctx.Attach(invCon);
            invCon.Slot = 5;
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
            
            var slot = ctx.Set<Connection>().OfType<InventoryConnection>().Single(c => c.SourceId == g1).Slot;

            Assert.Equal(ConnectionType.Inventory, ctx.Set<Connection>().OfType<InventoryConnection>().Single(c => c.SourceId == g1).Type);

            ctx.ChangeTracker.Clear();

            var e1c1 = ctx.GameEntities.AsNoTracking().Include(e => e.Outbound).Single(e => e.GameEntityId == g1);
            var c1e1 = e1c1.Outbound.OfType<InventoryConnection>().FirstOrDefault();
            ctx.Attach(c1e1);
            string output;
            ((INotifyPropertyChanged)c1e1).PropertyChanged += ((o, args) => output =args.PropertyName);
            c1e1.Slot = 12;
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();

            var e1c2 = ctx.GameEntities.AsNoTrackingWithIdentityResolution().Include(e => e.Outbound).Single(e => e.GameEntityId == g1);
            Assert.Equal(12, e1c2.Outbound.OfType<InventoryConnection>().FirstOrDefault().Slot);

            ctx.ChangeTracker.Clear();

            var targetEntityId = ctx.Set<Connection>().OfType<InventoryConnection>().Where(c => c.SourceId == g1)
                .Select (c => c.Target.GameEntityId).Single();

            Assert.Equal(3, entityCount);
            Assert.Equal(2, directConnectionsCount);
            Assert.Equal(1, directInventoryConCount);
            Assert.Equal(5, slot);
            Assert.Equal(g2, targetEntityId);
            Assert.Equal(1, inventoryConnectionsCount);
            Assert.Equal(2, totalConnectionsCount);

            ctx.ChangeTracker.Clear();

            var description = ctx.GameEntities
                .Include(e => e.Components)
                .Single(e => e.GameEntityId == g1)
                .Components.OfType<DescriptionComponent>()
                .First();
            
            ctx.ChangeTracker.Clear();

            var dur = ctx.Set<EntityPlayground.DataLayer.Models.Component>()
                .OfType<DurabilityComponent>()
                .Include(c => c.Entities)
                .First(c => c.Entities.Contains(entity1)).Durability;

            Assert.Equal("Some text", description.Description);
            Assert.Equal(55, dur);
            Assert.Equal(ComponentType.Description, description.Type);

            ctx.ChangeTracker.Clear();

            Assert.Equal(2, ctx.Connections.Count());
            Assert.Equal(3, ctx.GameEntities.Count());
            var deleted = ctx.GameEntities.Single(e => e.GameEntityId == g2);
            ctx.Remove(deleted);
            ctx.SaveChanges();

            Assert.Equal(1, ctx.Connections.Count());
            Assert.Equal(2, ctx.GameEntities.Count());

        }
    }
}