using MySafe.AdminCodeGenerator;

namespace MySafe.SafeHelper;

// In-memory DB
public class SafeCache
{
    private readonly Dictionary<int, ISafe> _cache = new();
    private readonly IAdminCodeGenerator _adminCodeGenerator;

    public SafeCache(IAdminCodeGenerator adminCodeGenerator)
    {
        _adminCodeGenerator = adminCodeGenerator;
        // add temporary safes to the safeCache
        AddSafe(1, new Safe(_adminCodeGenerator));
        AddSafe(2, new Safe(_adminCodeGenerator));
        AddSafe(3, new Safe(_adminCodeGenerator));
        AddSafe(4, new Safe(_adminCodeGenerator));
        AddSafe(5, new Safe(_adminCodeGenerator));
        AddSafe(6, new Safe(_adminCodeGenerator));
        AddSafe(7, new Safe(_adminCodeGenerator));
        AddSafe(8, new Safe(_adminCodeGenerator));
        AddSafe(9, new Safe(_adminCodeGenerator));
        AddSafe(10, new Safe(_adminCodeGenerator));
        AddSafe(9999, new Safe(_adminCodeGenerator));
    }

    public void AddSafe(int safeId, ISafe newSafe)
    {
        if (!_cache.TryAdd(safeId, newSafe))
        {
            throw new ArgumentException("non-unique safe id");
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
    
    public ISafe FactoryResetSafe(int safeId)
    {
        if (!_cache.ContainsKey(safeId))
        {
            throw new KeyNotFoundException("safe does not exist");
        }

        _cache[safeId] = new Safe(_adminCodeGenerator);
        
        return _cache[safeId];
    }
}