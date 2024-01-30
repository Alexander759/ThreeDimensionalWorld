using ThreeDimensionalWorldWeb.Configuration;

namespace NewProducts.SeedIdentity
{
    public class SeedDataService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SeedDataService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var identitySeeder = scope.ServiceProvider.GetRequiredService<AppRolesAndUsersSeeder>();
                await identitySeeder.SeedDefaultRolesAndUsersIfEmpty();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
