using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable once CheckNamespace

    public class NetFieldBuffer<T>
    {
        public int Count => _queue.Count;

        private readonly ConcurrentQueue<T> _queue;

        private readonly int _size;

        public NetFieldBuffer(NetFieldBuffer<T> queue)
        {
            var collection = queue as T[] ?? queue.Dequeue().ToArray();
            _queue = new ConcurrentQueue<T>(collection);
            _size = collection.Count();
        }

        public NetFieldBuffer(int size)
        {
            _queue = new ConcurrentQueue<T>();
            this._size = size;
        }

        public T Read()
        {
            var read = _queue.TryDequeue(out var result);
            return !read ? default : result;
        }

        public void Consume(NetFieldBuffer<T> buffer)
        {
            while (buffer.Any()) {
                Add(buffer.Read());
            }
        }

        public IEnumerable<T> Dequeue()
        {
            var dequeue = new List<T>();
            while (_queue.TryDequeue(out var entry)) {
                dequeue.Add(entry);
            }
            return dequeue;
        }

        public void Add(T entry)
        {
            while (_queue.Count >= _size && _size > 0) {
                _queue.TryDequeue(out _);
            }

            _queue.Enqueue(entry);
        }

        public T Peek()
        {
            return _queue.TryPeek(out var result) ? result : default;
        }

        public T Last()
        {
            return _queue.LastOrDefault();
        }

        public bool Any()
        {
            return _queue.Any();
        }

        public void Clear()
        {
            while (_queue.TryDequeue(out _)){}
        }
    }
