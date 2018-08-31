namespace LinFu.AOP.Interfaces
{
    internal class Counter<T>
    {
        private readonly object lockObject = new object();
        private int _count;

        public void Increment()
        {
            lock (lockObject)
            {
                _count++;
            }
        }

        public void Decrement()
        {
            lock (lockObject)
            {
                _count--;
            }
        }

        public int GetCount()
        {
            return _count;
        }
    }
}