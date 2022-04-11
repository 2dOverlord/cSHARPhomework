using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Homework
{
    class Program
    {
        static void Main(string[] args)
        { 
             AccountManager accManager = new AccountManager();
             accManager.run();
             /*
            Console.WriteLine(User.GetStringSha256Hash("Ui9921Ddsad"));
            Console.WriteLine(User.GetStringSha256Hash("UserIRJIojr"));
            */
        }
    }
}