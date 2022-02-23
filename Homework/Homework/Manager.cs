using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Newtonsoft.Json;

namespace Homework
{
    
    public class Manager
    {
        public TransactionCollection<Transaction> collection;
        private string fileName = "";
        public Manager()
        {
            this.collection = new TransactionCollection<Transaction>();
        }
        public void run()
        {
            
            while (true)
            {
                var menu = String.Join("\n", getMenu().ToArray());
                Console.WriteLine(menu);
                string command = Console.ReadLine();
                this.handleCommand(command);    
            }
        }

        public void handleCommand(string command)
        {
            switch (command)
            {
                case "add":
                    this.addObjectFromConsole();
                    break;
                case "edit":
                    this.editObjectById();
                    break;
                case "remove":
                    this.removeObjectById();
                    break;
                case "enter filename":
                    this.startWorkingWithFile();
                    break;
                case "search":
                    this.searchObjectsByInput();
                    break;
                case "sort":
                    this.sortObjects();
                    break;
                case "print all":
                    this.printAllObjects();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("There is no such command in the list, try again");
                    break;
            }
        }

        public void CreateObjects(List<Dictionary<string, string>> items, string fileName)
        {
            List<Dictionary<string, string>> ObjectsErrorsList = new List<Dictionary<string, string>>();
            List<Transaction> ValidObjects = new List<Transaction>();
            foreach (var item in items)
            {
                Transaction transaction = new Transaction();
                Dictionary<string, string> errorsDict = transaction.SetValuesFromDict(item);

                if (errorsDict.Count != 0)
                {
                    ObjectsErrorsList.Add(errorsDict);
                }
                else
                {
                    ValidObjects.Add(transaction);
                }
            }

            if (ObjectsErrorsList.Count != 0)
            {
                foreach (var err in ObjectsErrorsList)
                {
                    foreach (var item in err)
                    {
                        Console.WriteLine($"Errors: {item.Key} {item.Value}");
                    }
                }
            }
            else
            {
                this.fileName = fileName;
                foreach (var obj in ValidObjects)
                {
                    this.collection.AddObject(obj);
                }
                this.RewriteFile();
            }
        }

        public void RewriteFile()
        {
            List<Dictionary<string, string>> objects = new List<Dictionary<string, string>>();
            foreach (var obj in this.collection.container)
            {
                
                objects.Add(obj.GetValuesInDict());
            }
            string jsonString = JsonConvert.SerializeObject( objects );
            using (StreamWriter writer = new StreamWriter(this.fileName, false)){ 
                writer.Write(jsonString);
            }
        }
        public void startWorkingWithFile()
        {
            if (this.fileName == "")
            {
                Console.Write("Enter the filename: ");
                string fileName = Console.ReadLine();
                Console.WriteLine(fileName);
                try
                {
                    var path = "D:\\cSHARPhomework\\Homework\\Homework\\" + fileName;
                    Console.WriteLine(path);
                    List<Dictionary<string, string>> items;
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    }
                    this.CreateObjects(items, path);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("There is no such file");
                }
            }
            else
            {
                Console.WriteLine("You already have a filename");
            }
        }
        public void sortObjects()
        {
            Console.WriteLine("Sorting............");
            this.collection.container.Sort();
            Console.WriteLine("\nAfter sort by part number:");
            this.collection.container.ForEach(Console.WriteLine);
        }
        public void searchObjectsByInput()
        {
            Console.WriteLine("Input your value: ");
            string value = Console.ReadLine();
            List<Transaction> results = new List<Transaction>();
            foreach (var obj in this.collection.container)
            {
                if (obj.ToString().Contains(value))
                {
                    results.Add(obj);
                }
            }

            if (results.Count != 0)
            {
                foreach (var result in results)
                {
                    Console.WriteLine(result.ToString());
                }
            }
            else
            {
                Console.WriteLine("No matches found");
            }
        }
        public void removeObjectById()
        {
            Console.Write("Enter the object id: ");
            string objectID = Console.ReadLine();

            try
            {
                int objectIdInt = Int32.Parse(objectID);
                var obj = this.collection.container.Find(x => x.ID == objectIdInt);
                if (obj == null)
                {
                    throw new Exception();
                }

                this.collection.container.Remove(obj);
                if (this.fileName != "")
                {
                    this.RewriteFile();
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Id must be a number");
            }
            catch (Exception)
            {
                // Console.WriteLine("there is no element with such id in the list");
                Console.WriteLine("smth gone wrong");
            }
        }

        public void editObjectById()
        {
            Console.Write("Enter the object id: ");
            string objectID = Console.ReadLine();

            try
            {
                int objectIdInt = Int32.Parse(objectID);
                var obj = this.collection.container.Find(x => x.ID == objectIdInt);
                if (obj == null)
                {
                    throw new Exception();
                }

                int indexOf = this.collection.container.IndexOf(obj);
                Console.Write("Type the field you want to edit: ");
                string fieldName = Console.ReadLine();
                
                PropertyInfo[] properties = obj.GetProperties();
                if (Array.Find(properties.ToArray(), x => x.Name == fieldName) == null)
                {
                    throw new Exception();
                }
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if (property.Name == fieldName)
                    {
                        Console.WriteLine("Enter value: ");
                        var fieldValue = Console.ReadLine();
                        var propType = property.PropertyType;
                        var converter = TypeDescriptor.GetConverter(propType);
                        var convertedObject = converter.ConvertFromString(fieldValue); 
                        property.SetValue(obj, convertedObject);
                        this.collection.container[indexOf] = obj;
                    }
                }
                this.RewriteFile();
                
            }
            catch (FormatException)
            {
                Console.WriteLine("Id must be a number");
            }
            catch (Exception)
            {
                // Console.WriteLine("there is no element with such id in the list");
                Console.WriteLine("smth gone wrong");
            }
        }
        public void addObjectFromConsole()
        {
            Transaction transaction = new Transaction();
            Console.WriteLine("Enter this fields: ");
            
            Dictionary<string, string> errs = transaction.Read();
            var transactionID = transaction.ID;
            var sameIdExist = this.collection.container.Exists(x => x.ID == transactionID);

            if (sameIdExist)
            {
                errs.Add("ID", "Same id already exists in the list");
            }

            if (errs.Count != 0)
            {
                Console.WriteLine("There are such errors in input: ");
                foreach (var item in errs)
                {
                    Console.WriteLine(item.Value);
                }
            }
            else
            {
                this.collection.AddObject(transaction);
                Console.WriteLine("object has been added correctly");
                if (this.fileName != "")
                {
                    this.RewriteFile();
                }
            }
            
        }

        public void printAllObjects()
        {
            this.collection.PrintAll();
        }
        public List<string> getMenu()
        {
            List<string> menu = new List<string>();
            menu.Add("type 'add' to add a new item from console");
            menu.Add("type 'edit' to edit object by id");
            menu.Add("type 'remove' to remove object by id");
            menu.Add("type 'enter filename' to make all functions using your file");
            menu.Add("type 'search' to search object by input");
            menu.Add("type 'sort' to sort objects by all fields");
            menu.Add("type 'print all' to print all objects in collection");
            menu.Add("type 'exit' to end program");
            return menu;
        }
    }

}