using Microsoft.Extensions.Caching.Memory;

namespace BookingEngine.Helpers
{
    public class MyMemoryCache
    {
        private const string KEY = "user_cache";
        public MemoryCache Cache { get; set; }
        public MyMemoryCache()
        {

            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

       
    }
}
