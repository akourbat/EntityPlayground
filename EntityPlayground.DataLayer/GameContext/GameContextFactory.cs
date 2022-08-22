using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EntityPlayground.DataLayer.GameContext
{
    internal class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        private const string ConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=EntityPlayground;Trusted_Connection=True;MultipleActiveResultSets=true";
        public GameContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionBilder = new DbContextOptionsBuilder<GameContext>();
            var connString = config.GetConnectionString("DefaultConnection");

            optionBilder.UseSqlServer(connString, ctx => ctx.MigrationsAssembly("EntityPlayground.DataLayer"));

            return new GameContext(optionBilder.Options);
        }
    }
}
