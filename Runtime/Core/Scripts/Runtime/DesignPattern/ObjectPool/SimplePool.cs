using System.Collections.Generic;

namespace NIX.Core.DesignPatterns
{
    public sealed class SimplePool<T> where T : class, new()
    {   
        private readonly Stack<T> _stack = new();
        public T Get() => _stack.Count > 0 ? _stack.Pop() : new T();

        public List<T> GetList(int count)
        {
            if (count <= 0) return null;
            List<T> result = new();
            for (int i = 0; i < count; i++)
            {
                result.Add(Get());
            }
            return result;
        }
        
        public void Release(T item) => _stack.Push(item);

        public void Fill(int toCount)
        {
            if (_stack.Count >= toCount) return;
            int createCount = toCount - _stack.Count;
            for (var i = 0; i < createCount; ++i)
            {
                this.Release(new T());
            }
        }

        public void ReleasePool()
        {
            _stack.Clear();
        }
    }
}