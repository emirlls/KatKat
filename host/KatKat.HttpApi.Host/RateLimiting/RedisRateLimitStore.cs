using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace KatKat.RateLimiting;

/// <summary>
/// Redis-backed implementation of <see cref="IRateLimitStore"/>. Uses an INCR+EXPIRE Lua script
/// so the increment and the window expiry are set atomically - avoiding a race where a key could
/// be left without a TTL (and so would never reset) under concurrent requests.
/// </summary>
public class RedisRateLimitStore : IRateLimitStore
{
    private const string LuaScript = @"
        local current = redis.call('INCR', KEYS[1])
        if tonumber(current) == 1 then
            redis.call('EXPIRE', KEYS[1], ARGV[1])
        end
        return current";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisRateLimitStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<bool> TryAcquireAsync(string key, int permitLimit, TimeSpan window)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var result = (long)await db.ScriptEvaluateAsync(LuaScript, new RedisKey[] { key }, new RedisValue[] { (int)window.TotalSeconds });
        return result <= permitLimit;
    }
}
