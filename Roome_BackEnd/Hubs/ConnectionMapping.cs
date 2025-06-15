using System.Collections.Concurrent;

namespace Roome_BackEnd.Hubs
{
    public static class ConnectionMapping
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

        public static void Add(string userId, string connectionId)
        {
            _connections.AddOrUpdate(userId,
                new HashSet<string> { connectionId },
                (key, existingSet) =>
                {
                    existingSet.Add(connectionId);
                    return existingSet;
                });
        }

        public static void Remove(string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
        }

        public static IEnumerable<string> GetConnections(string userId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                return connections;
            }
            return Enumerable.Empty<string>();
        }
    }
}
