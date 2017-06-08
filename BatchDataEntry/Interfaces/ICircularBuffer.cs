using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Interfaces
{
    public interface ICircularBuffer<T>
    {
        int Count { get; }
        int Capacity { get; set; }
        T Enqueue(T item);
        T Dequeue();
        void Clear();
        T this[int index] { get; set; }
        int IndexOf(T item);
        void Insert(int index, T item);
        void RemoveAt(int index);
    }
}
