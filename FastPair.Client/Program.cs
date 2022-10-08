using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

var hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7257/clients", options =>
    {
        options.Transports = HttpTransportType.LongPolling;
    })
    .WithAutomaticReconnect()
    .Build();

hubConnection.On<string>("SendAuthRequest", (connectionId) =>
{
    Console.WriteLine($"Код сессии: {connectionId}");
});

hubConnection.On<string>("SendAdminCode",  async (response) =>
{
    await hubConnection.InvokeAsync("SendAuthRequest");
});

await hubConnection.StartAsync();

var code = new Random().Next(1000, 9999);

Console.WriteLine($"Ваш код: {code}");

await hubConnection.InvokeAsync<string>("SendClientCode", code.ToString());

Console.ReadKey();

await hubConnection.StopAsync();