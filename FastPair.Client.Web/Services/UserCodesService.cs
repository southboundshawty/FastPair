using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace FastPair.Client.Web.Services;

public class UserCodesService
{
    public UserCodesService(IConfiguration configuration)
    {
        const string host = "http://192.168.224.226:5259";
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{host}/clients", options => 
            {
                options.Transports = HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<string>("SendAuthRequest", AuthHandler);
    }

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
    
    public static (string Url, string Code) GenerateUrl()
    {
        var code = new Random().Next(10000, 99999).ToString();

        var url = $"http://192.168.224.226:5257/UserCodes?code={code}";

        return (url, code);
    }
}