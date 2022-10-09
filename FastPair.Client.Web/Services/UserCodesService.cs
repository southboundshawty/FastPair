using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastPair.Client.Web.Services;

public class UserCodesService
{
    public UserCodesService(IConfiguration configuration)
    {
        _configuration = configuration;

        var host = Settings.BackendHost;
            //_configuration.GetValue<string>("BackendHost");
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{host}/clients", options => 
            {
                options.Transports = HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<string>("SendAuthRequest", AuthHandler);
    }

    private readonly IConfiguration _configuration;

    private void AuthHandler(string obj)
    {
        OnAuthorized?.Invoke(true);
    }

    private readonly HubConnection _hubConnection;

    public event Action<bool> OnAuthorized;

    public async Task SendCode(string code)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            await _hubConnection.StartAsync();
        }
        
        await _hubConnection.InvokeAsync("SendClientCode", code);
    }
    
    public (string Url, string Code) GenerateUrl()
    {
        var code = new Random().Next(10000, 99999).ToString();

        var host = Settings.AdminHost;
            //_configuration.GetValue<string>("AdminHost");

        var url = $"{host}/UserCodes?code={code}";

        return (url, code);
    }
}