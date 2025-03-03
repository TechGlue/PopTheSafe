using MySafe.AdminCodeGenerator;

namespace MySafe.SafeHelper;

// In-memory DB
public class SafeCache
{
    private readonly Dictionary<int, ISafe> _cache = new();

    public SafeCache(IAdminCodeGenerator adminCodeGenerator)
    {
        // add dummy data to safeCache
        AddSafe(1, new Safe(adminCodeGenerator));
        AddSafe(2, new Safe(adminCodeGenerator));
        AddSafe(3, new Safe(adminCodeGenerator));
        AddSafe(4, new Safe(adminCodeGenerator));
    }

    public void AddSafe(int safeId, ISafe newSafe)
    {
        if (!_cache.TryAdd(safeId, newSafe))
        {
            throw new ArgumentException("Non-unique safe id");
        }
    }

    public ISafe FetchSafe(int safeId)
    {
        if (!_cache.ContainsKey(safeId))
        {
            throw new KeyNotFoundException("safe does not exist");
        }

        return _cache[safeId];
    }
}