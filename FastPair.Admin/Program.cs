using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

var hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7257/clients", options => 
    {
        options.Transports = HttpTransportType.LongPolling;
    })
    .WithAutomaticReconnect()
    .Build();

await hubConnection.StartAsync();

hubConnection.On<string>("SendAdminCode", Console.WriteLine);

Console.WriteLine("Введите код:");
var code = Console.ReadLine();

await hubConnection.InvokeAsync("SendAdminCode", code);

Console.ReadKey();

await hubConnection.StopAsync();

