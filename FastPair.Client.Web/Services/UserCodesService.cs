using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastPair.Client.Web.Services;

public class UserCodesService
{
    public UserCodesService(IConfiguration configuration)
    {
        _configuration = configuration;

        var host = "http://192.168.253.226:5257";
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{host}/clients", options => 
            {
                options.Transports = HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<string>("SendAdminCode", Console.WriteLine);
    }
    
    private HubConnection _hubConnection;

    private readonly IConfiguration _configuration;

    public async Task SendCode(string code)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            await _hubConnection.StartAsync();
        }
        
        await _hubConnection.InvokeAsync("SendClientCode", code);
    }
}