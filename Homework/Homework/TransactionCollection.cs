using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Homework
{
    public class TransactionCollection<T>
    {
        public List<T> container;

        public TransactionCollection()
        {
            this.container = new List<T>();
        }

        public void AddObject(T obj)
        {
            this.container.Add(obj);
        }

        public void PrintAll()
        {
            foreach (var item in this.container)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
