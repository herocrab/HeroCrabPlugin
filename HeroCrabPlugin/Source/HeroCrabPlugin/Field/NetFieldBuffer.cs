// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network field buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetFieldBuffer<T>
    {
        /// <summary>
        /// Count of entries in the buffer.
        /// </summary>
        public int Count => _queue.Count;

        private readonly ConcurrentQueue<T> _queue;

        private readonly int _size;

        /// <summary>
        /// Create a new buffer while emptying an existing buffer.
        /// </summary>
        /// <param name="queue">NetFieldBuffer</param>
        public NetFieldBuffer(NetFieldBuffer<T> queue)
        {
            var collection = queue as T[] ?? queue.Dequeue().ToArray();
            _queue = new ConcurrentQueue<T>(collection);
            _size = collection.Length;
        }

        /// <summary>
        /// Create a new buffer given the buffer size.
        /// </summary>
        /// <param name="size">Int32</param>
        public NetFieldBuffer(int size)
        {
            _queue = new ConcurrentQueue<T>();
            this._size = size;
        }

        /// <summary>
        /// Read an entry from the buffer.
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            var read = _queue.TryDequeue(out var result);
            return !read ? default : result;
        }

        /// <summary>
        /// Consume the contents of a buffer by adding them to this buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public void Consume(NetFieldBuffer<T> buffer)
        {
            while (buffer.Any()) {
                Add(buffer.Read());
            }
        }

        /// <summary>
        /// Dequeue all items in the buffer and return an enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Dequeue()
        {
            var dequeue = new List<T>();
            while (_queue.TryDequeue(out var entry)) {
                dequeue.Add(entry);
            }
            return dequeue;
        }

        /// <summary>
        /// Add an item to this buffer.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(T entry)
        {
            while (_queue.Count >= _size && _size > 0) {
                _queue.TryDequeue(out _);
            }

            _queue.Enqueue(entry);
        }

        /// <summary>
        /// Peek an item from this buffer.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _queue.TryPeek(out var result) ? result : default;
        }

        /// <summary>
        /// Retrieve the last content in the buffer without dequeuing it.
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            return _queue.LastOrDefault();
        }

        /// <summary>
        /// Check if there are any entries in the buffer.
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return _queue.Any();
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void Clear()
        {
            while (_queue.TryDequeue(out _)){}
        }
    }
}
