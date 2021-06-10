using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp7.Cats.Controllers;
using WebApp7.Cats.DALL;
using Xunit;
using Xunit.Abstractions;

namespace WebApp7.Cats.Tests
{
    public class CatsControllerTests
    {
        private readonly ITestOutputHelper outputs;

        public CatsControllerTests(ITestOutputHelper output)
        {
            this.outputs = output;
        }
        [Fact]
        public void CatsFilters()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<wg_forge_dbContext>()
            .UseInMemoryDatabase(databaseName: "wgForge")
            .Options;
            using (var context = new wg_forge_dbContext(options))
            {
                for (int i = 0; i < 30; i++)
                {
                    context.Cats.Add(new DALL.Cats { Name = "Name" + i, TailLength = 1 * i, WhiskersLength = 1 * i, Color = CatColor.Red });
                }

                context.SaveChanges();
            }
            using (var context = new wg_forge_dbContext(options))
            {
                CatsController controller = new CatsController(context);
                int offset = 0;
                List<DALL.Cats> movies = new List<DALL.Cats>();
                //List<DALL.Cats> movies = controller.SortAsync("tail_length", "asc", 0*10);
                foreach (var item in movies)
                {
                    outputs.WriteLine(item.ToString());
                }
                outputs.WriteLine("------------------");
                //movies = controller.SortAsync("tail_length", "asc", 1*10);
                foreach (var item in movies)
                {
                    outputs.WriteLine(item.ToString());
                }
                outputs.WriteLine("------------------");
                //movies = controller.SortAsync("tail_length", "asc", 2*10);
                foreach (var item in movies)
                {
                    outputs.WriteLine(item.ToString());
                }
            }
        }
    }
}
