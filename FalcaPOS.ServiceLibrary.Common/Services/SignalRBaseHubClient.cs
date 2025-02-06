using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Logger;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public abstract class SignalRBaseHubClient
    {
        public string ConnectionURL { get; set; }

        protected HubConnection _hubConnection;

        private readonly Logger _logger;

        public SignalRBaseHubClient(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public HubConnectionState State
        {
            get
            {
                return _hubConnection.State;
            }
        }
        protected void Init()
        {
            try
            {
                _logger.LogInformation("Init Hub........");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(ConnectionURL, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(AppConstants.ACCESS_TOKEN);
                    })
                    .WithAutomaticReconnect(new TimeSpan[] {
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(60),
                    })
                    .Build();

                _hubConnection.Reconnecting += HubConnection_Reconnecting;
                _hubConnection.Reconnected += HubConnection_Reconnected;
                _hubConnection.Closed += HubConnection_Closed;
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in Hub Init", _ex);
            }
        }

        private Task HubConnection_Closed(Exception arg)
        {

            _logger?.LogError("Hub connection was closed", arg);

            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            _logger.LogInformation($"Hub is reconnected with connection Id {arg}");

            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            _logger.LogWarning("Hub is reconneting.....");

            return Task.CompletedTask;
        }

        public abstract Task StartHubAsync(string connectionHubURL);
    }
}
