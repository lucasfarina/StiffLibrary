using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StiffLibrary
{
    public class Pile<T>
    {
        public Pile()
        {
            items = new List<T>();
        }

        private List<T> items;

        public void Push(T item)
        {
            items.Add(item);
        }

        public T Pop()
        {
            T answer = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return answer;
        }

        public int PileSize()
        {
            return items.Count;
        }

        public T GetTop()
        {
            return items[items.Count - 1];
        }
    }
}
