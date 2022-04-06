using System;
using System.Collections.Generic;
using System.Reflection;

namespace Homework
{
    class Program
    {
        static void Main(string[] args)
        {
            TransactionCollection<Transaction> collection = new TransactionCollection<Transaction>();
            collection.run();
        }
    }
}