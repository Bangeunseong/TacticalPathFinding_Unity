using System.Collections.Generic;
using System.Linq;

namespace _Tools.DataStructures
{
    public class PriorityQueue<TElement, TPriority> {
        readonly SortedList<TPriority, Queue<TElement>> priorities = new ();

        public int Count { get; set; }

        public void Enqueue(TElement element, TPriority priority) {
            if (!priorities.ContainsKey(priority)) {
                priorities[priority] = new Queue<TElement>();
            }

            priorities[priority].Enqueue(element);
            Count++;
        }

        public TElement Dequeue() {
            if (priorities.Count == 0) throw new System.InvalidOperationException("The queue is empty");

            var firstPair = priorities.First();
            var element = firstPair.Value.Dequeue();
            if (firstPair.Value.Count == 0) priorities.Remove(firstPair.Key);

            Count--;
            return element;
        }
    }
}