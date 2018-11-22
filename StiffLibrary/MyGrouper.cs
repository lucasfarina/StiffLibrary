using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StiffLibrary
{
    public class MyGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key { get; set; }
        public MyGrouping(TKey key)
        {
            Key = key;
        }
    }
    public class MyGrouper<TKey, TElement> : List<MyGrouping<TKey, TElement>>
    {
        public new void Add(MyGrouping<TKey, TElement> item)
        {
            MyGrouping<TKey, TElement> find;
            if ((find = this.FirstOrDefault(x => x.Key.Equals(item.Key))) != null)
            {
                foreach (TElement subItem in item)
                {
                    if (!find.Contains(subItem))
                    {
                        find.Add(subItem);
                    }
                }
            }
            else
            {
                base.Add(item);
            }
        }

        public void RemoveByKey(TKey key)
        {
            MyGrouping<TKey, TElement> item;
            if ((item = this.FirstOrDefault(x => x.Key.Equals(key))) != null)
            {
                this.Remove(item);
            }
        }
    }
}
