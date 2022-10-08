namespace FastPair.Web.Models;

public record Connection<T>(T ConnectionId, HashSet<ConnectionCode> Codes) where T : notnull;