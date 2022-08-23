using EntityPlayground.DataLayer.Derived;
using EntityPlayground.DataLayer.GameContext;
using EntityPlayground.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;



namespace EntityPlayground.DataTests
{
    public class GameContextTests
    {
        private  Guid g1 = new("6086b495-5df3-4048-949a-647c7e62ee61");
        private Guid g2 = new("5e3eb754-5bd7-4ee3-a26b-7c7aa9dac0fa");
        private Guid g3 = new("5d808377-a6a6-4131-9a95-a5f145ce07b3");

        [Fact]
        public void RelationshipSetupTests()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<GameContext>();

            using var ctx = new GameContext(options);
            ctx.Database.EnsureCreated();

            var entity1 = new GameEntity { GameEntityId = g1 };
            var entity2 = new GameEntity { GameEntityId = g2 };
            var entity3 = new GameEntity { GameEntityId = g3 };

            var connection1 = new InventoryConnection { Source = entity1, Target = entity2, Slot = 2 };
            var connection2 = new BaseConnection { Source = entity1, Target = entity3 };

            var component1 = new DurabilityComponent {  Durability=55 };
            var component2 = new DescriptionComponent { Description = "Some text" };
            var clink1 = new EntityComponent { GameEntity = entity1, Component = component1 };
            var clink2 = new EntityComponent { GameEntity = entity1, Component = component2 };

            ctx.AddRange(entity1, entity2, entity3, connection1, connection2, component1, component2, clink1, clink2);
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
            c1e1.Slot = 12;
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
            var e1c2 = ctx.GameEntities.AsNoTracking().Include(e => e.Outbound).Single(e => e.GameEntityId == g1);
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

            var dur = ctx.Set<Component>()
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