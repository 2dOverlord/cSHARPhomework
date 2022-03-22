using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public void Sort()
        {
            this.container.Sort(delegate(T firstItem, T secondItem)
            {
                List<bool> compareList = new List<bool>();
                
                foreach (var property in firstItem.GetType().GetProperties())
                {
                    object firstItemPropertyValue = property.GetValue(firstItem, null);
                    object secondItemPropertyValue = property.GetValue(secondItem, null);
                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    //compareList.Add(firstItemPropertyValue.CompareTo();
                }
            });
        }
    }
}
