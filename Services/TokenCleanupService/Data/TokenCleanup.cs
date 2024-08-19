using BL.Interfaces;
using Common.Configuration;
using Common.SearchParams;

namespace TokenCleanupService.Data
{
    public class TokenCleanup : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TokenCleanup> _logger;
        private readonly SharedConfiguration _sharedConfiguration;
        private ITokensBL? _tokensBL;

        public TokenCleanup(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<TokenCleanup> logger,
            SharedConfiguration sharedConfiguration
            )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _sharedConfiguration = sharedConfiguration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _tokensBL = scope.ServiceProvider.GetRequiredService<ITokensBL>();
                    try
                    {
                        var tokensForDelete = (await _tokensBL.GetAsync(new TokensSearchParams
                        {
                            IsActive = false
                        })).Objects;

                        foreach (var token in tokensForDelete)
                        {
                            await _tokensBL.DeleteAsync(token.Id);
                        }

                        _logger.LogInformation($"Deleted {tokensForDelete.Count} inactive tokens.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while cleaning up tokens.");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(_sharedConfiguration.CleanupIntervalHours!.Value), stoppingToken);
            }

            _logger.LogInformation("Token Cleanup Service is stopping.");
        }
    }
}
