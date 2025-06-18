using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;

namespace Tournament.API.Extensions
{

    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDataAsync(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Tournament.Data.Data.TournamentAPIContext>();

            await db.Database.MigrateAsync();

            try
            {
                await Tournament.Data.Data.SeedData.InitAsync(db);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}


