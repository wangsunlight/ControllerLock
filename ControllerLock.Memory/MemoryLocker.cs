using ControllerLock.Abstractions;

namespace ControllerLock.Memory
{
    public class MemoryLocker : ILocker
    {
        private readonly LockProvider<string> lockProvider;

        public MemoryLocker(LockProvider<string> lockProvider)
        {
            this.lockProvider = lockProvider;
        }
        public bool Lock(string key, string value, TimeSpan timeout, bool isdelay)
        {
            Console.WriteLine($"上锁key:{key}value{value}");
            if (isdelay)
            {
                lockProvider.Wait(key);
                
                return true;
            }

            return lockProvider.Wait(key, timeout.Seconds);
        }

        public void UnLock(string key, string value, bool isdelay)
        {
            Console.WriteLine($"解锁key:{key}value{value}");
            lockProvider.Release(key);
        }
    }
}
