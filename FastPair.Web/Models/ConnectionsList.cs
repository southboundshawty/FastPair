namespace FastPair.Web.Models;

public class ConnectionsList<TConnectionId> where TConnectionId : IEquatable<TConnectionId>
{
    private readonly HashSet<Connection<TConnectionId>> _connections = new ();

    public bool Contains(TConnectionId connectionId)
    {
        lock (_connections)
        {
            return _connections.Any(x => x.ConnectionId.Equals(connectionId));
        }
    }

    public Connection<TConnectionId>? GetConnection(TConnectionId connectionId)
    {
        lock (_connections)
        {
            return _connections.FirstOrDefault(x => x.ConnectionId.Equals(connectionId));
        }
    }
    
    public Connection<TConnectionId>? GetConnectionByCode(string code)
    {
        lock (_connections)
        {
            return _connections.FirstOrDefault(x => x.Codes.Any(c => c.Code == code));
        }
    }
    
    public void Remove(TConnectionId connectionId)
    {
        lock (_connections)
        {
            _connections.RemoveWhere(x => x.ConnectionId.Equals(connectionId));
        }
    }

    public void Add(Connection<TConnectionId> connection)
    {
        lock (_connections)
        {
            _connections.Add(connection);
        }
    }

    public void ClearExpiredSessions(Predicate<ConnectionCode> match)
    {
        lock (_connections)
        {
            foreach (var connection in _connections)
            {
                connection.Codes.RemoveWhere(match);
            }

            //_connections.RemoveWhere(x => !x.Codes.Any());
        }
    }

    public HashSet<Connection<TConnectionId>> GetConnections()
    {
        lock (_connections)
        {
            return _connections;
        }
    }
}