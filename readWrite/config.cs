using static System.Math;

namespace TeleprompterConsole
{
    internal class TelePrompterConfig
    {
        public bool Done => done;
        
        private bool done;
        
        public void SetDone()
        {
            done = true;
        }
        
        private object lockhandle = new object();
        public int DelayInMilliSeconds { get; private set; } = 200;
        
        public void UpdateDelay(int increment) // negative to speedup
        {
            var newDelay = Min(DelayInMilliSeconds + increment, 1000);
            newDelay = Max(newDelay, 20);
            lock (lockhandle)
            {
                DelayInMilliSeconds = newDelay;
            }
        }
    }
}