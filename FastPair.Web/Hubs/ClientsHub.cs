using FastPair.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace FastPair.Web.Hubs;

public class ClientsHub : Hub
{
    private const int MaxSessionExistTimeMinutes = 15;

    private static readonly ConnectionsList<string> Connections = new();
    
    public void SendAdminCode(string code)
    {
        lock (Connections)
        {
            var connection = Connections.GetConnectionByCode(code);

            if (connection == null)
            {
                Clients.Caller.SendAsync("SendAdminCode", "Incorrect code!");

                return;
            }

            Connections.Remove(connection.ConnectionId);

            Clients.Client(connection.ConnectionId).SendAsync("SendAuthRequest", connection.ConnectionId);

            Trace();
        }
    }

    public void SendClientCode(string? code)
    {
        lock (Connections)
        {
            if (Connections.Contains(Context.ConnectionId))
            {
                var connection = Connections.GetConnection(Context.ConnectionId);

                if (code != null)
                {
                    connection?.Codes.Add(new ConnectionCode(code, DateTime.Now));
                }
            }
        }

        Trace();
    }
    
    public override Task OnConnectedAsync()
    {
        lock (Connections)
        {
            Connections.Add(new Connection<string>(Context.ConnectionId, new HashSet<ConnectionCode>()));
        }

        Trace();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        lock (Connections)
        {
            if (Connections.Contains(Context.ConnectionId))
            {
                Connections.Remove(Context.ConnectionId);
            }
        }

        Trace();

        return base.OnDisconnectedAsync(exception);
    }

    private static void ClearTimeOutSessions()
    {
        lock (Connections)
        {
            Connections.ClearExpiredSessions(x => 
                    DateTime.Now.Subtract(x.TimeIn).TotalMinutes >= MaxSessionExistTimeMinutes);
        }
    }

    private void Trace()
    {
        lock (Connections)
        {
            Console.WriteLine("in trace:");
        
            foreach (var connection in Connections.GetConnections())
            {
                Console.WriteLine($"{connection.ConnectionId} - {string.Join(",", connection.Codes)}");
            }
        
            Console.WriteLine();
        }
    }

    protected override void Dispose(bool disposing)
    {
        ClearTimeOutSessions();

        Console.WriteLine("Очистили очередь");

        Trace();

        base.Dispose(disposing);
    }
}