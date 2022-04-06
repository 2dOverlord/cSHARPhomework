using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class TransactionCollection<T>
    {
        public List<T> container;
        private string fileName = "";

        public TransactionCollection()
        {
            this.container = new List<T>();
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
                    this.addObjectFromConsoleGeneric();
                    break;
                case "edit":
                    this.editObjectById();
                    break;
                case "remove":
                    this.removeObjectById();
                    break;
                case "search":
                    this.searchObjectsByInput();
                    break;
                case "enter filename":
                    this.startWorkingWithFile();
                    break;
                case "sort":
                    this.Sort();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "print all":
                    this.printAllObjects();
                    break;
                default:
                    Console.WriteLine("There is no such command in the list, try again");
                    break;
            }
        }

        public string ObjectValuesToString(T obj)
        {
            string returnString = "";
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                returnString += prop.GetValue(obj).ToString();
            }

            return returnString;
        }
        public void searchObjectsByInput()
        {
            Console.WriteLine("Input your value: ");
            string value = Console.ReadLine();
            List<T> results = new List<T>();
            foreach (var obj in this.container)
            {
                string objectString = ObjectValuesToString(obj);
                
                if (objectString.Contains(value))
                {
                    results.Add(obj);
                }
            }

            if (results.Count != 0)
            {
                foreach (var result in results)
                {
                    PropertyInfo[] properties = result.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        Console.WriteLine(prop.Name.ToString() + ": " + prop.GetValue(result));
                    }
                }
            }
            else
            {
                Console.WriteLine("No matches found");
            }
        }

        public int returnObjId(T obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.Name.ToString().ToLower() == "id")
                {
                    return Convert.ToInt32(prop.GetValue(obj));
                }
            }

            return -1;
        }
        
        public void removeObjectById()
        {
            Console.Write("Enter the object id: ");
            string objectID = Console.ReadLine();

            try
            {
                int objectIdInt = Int32.Parse(objectID);
                var obj = this.container.Find(x => returnObjId(x) == objectIdInt);
                if (obj == null)
                {
                    throw new Exception();
                }

                this.container.Remove(obj);
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
                Console.WriteLine("there is no element with such id in the list");
            }
        }
        public void editObjectById()
        {
            Console.Write("Enter the object id: ");
            string objectID = Console.ReadLine();

            try
            {
                int objectIdInt = Int32.Parse(objectID);
                
                var obj = this.container.Find(x => returnObjId(x) == objectIdInt);
                if (obj == null)
                {
                    throw new Exception();
                }

                int indexOf = this.container.IndexOf(obj);
                Console.Write("Type the field you want to edit: ");
                string fieldName = Console.ReadLine();

                PropertyInfo[] properties = obj.GetType().GetProperties();
                if (Array.Find(properties.ToArray(), x => x.Name.ToString() == fieldName) == null)
                {
                    throw new Exception();
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var propType = property.PropertyType;
                    if (property.Name.ToString() == fieldName && propType.ToString() != "System.DateOnly" && propType.ToString() != "System.TimeOnly")
                    {
                        Console.WriteLine("Enter value: ");
                        var fieldValue = Console.ReadLine();
                        var converter = TypeDescriptor.GetConverter(propType);
                        var convertedObject = converter.ConvertFromString(fieldValue);
                        property.SetValue(obj, convertedObject);
                        this.container[indexOf] = obj;
                    }
                    else if (property.Name.ToString() == fieldName && propType.ToString() == "System.DateOnly")
                    { 
                        Console.WriteLine("Enter value: ");
                        var fieldValue = Console.ReadLine();
                        var value = System.DateOnly.Parse(fieldValue);
                        property.SetValue(obj, value);
                        this.container[indexOf] = obj;
                    } else if(property.Name.ToString() == fieldName  && propType.ToString() == "System.TimeOnly")
                    { 
                        Console.WriteLine("Enter value: ");
                        var fieldValue = Console.ReadLine();
                        var value = System.TimeOnly.Parse(fieldValue);
                        property.SetValue(obj, value);
                        this.container[indexOf] = obj;
                    }
                }

                if (this.fileName != "")
                {
                    this.RewriteFile();
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("Id must be a number");
            }
            catch (TargetInvocationException err)
            {
                Console.WriteLine(err.InnerException.Message);
            }
            catch (Exception err)
            {
                Console.WriteLine("There are no such object or a field");
            }
        }
        public void RewriteFile()
        {
            List<Dictionary<string, string>> objects = new List<Dictionary<string, string>>();
            foreach (var obj in this.container)
            {
                Dictionary<string, string> objDict = new Dictionary<string, string>();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    objDict.Add(prop.Name.ToString(), prop.GetValue(obj).ToString());
                }
                objects.Add(objDict);
            }
            string jsonString = JsonConvert.SerializeObject( objects, Formatting.Indented);
            // Set a variable to the Documents path.
            var path = this.fileName;
            using (StreamWriter writer = new StreamWriter(path, false)){ 
                writer.Write(jsonString);
            }
        }

        public void CreateObjectsFromData(List<Dictionary<string, string>> items, string fileName)
        {
            List<Dictionary<string, string>> ObjectsErrorsList = new List<Dictionary<string, string>>();
            List<T> ValidObjects = new List<T>();
        
            for (int i = 0; i < items.Count(); i++)
            {
                Dictionary<string, string> item = items[i];
                Dictionary<string, string> lowercase_dict = new Dictionary<string, string>();
                foreach (var kv in item)
                {
                    lowercase_dict[kv.Key.ToString().ToLower()] = kv.Value;
                }

                item = lowercase_dict;
                T obj = (T) Activator.CreateInstance(typeof(T));
                PropertyInfo[] properties = obj.GetType().GetProperties();
                Dictionary<string, string> errDict = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                {
                    var property = properties[j];
                    try
                    {
                        var propType = property.PropertyType;
                        var converter = TypeDescriptor.GetConverter(propType);
                        var convertedObject = converter.ConvertFromString(item[property.Name.ToString().ToLower()]);
                        property.SetValue(obj, convertedObject);
                        if (property.Name.ToString().ToLower() == "id")
                        {
                            var sameIdExist = this.container.Exists(x =>
                                property.GetValue(x).ToString() == item[property.Name.ToString().ToLower()]);
                            sameIdExist = sameIdExist || ValidObjects.Exists(x =>
                                property.GetValue(x).ToString() == item[property.Name.ToString().ToLower()]);
                            if (sameIdExist)
                            {
                                errDict.Add("ID", "Same id already exists in the list");
                            }
                        }
                    }
                    catch (NotSupportedException notSupportedErr)
                    {
                        var propType = property.PropertyType;
                        if (propType.ToString() == "System.DateOnly")
                        {
                            var value = System.DateOnly.Parse(item[property.Name.ToString().ToLower()]);
                            property.SetValue(obj, value);
                        } else if(propType.ToString() == "System.TimeOnly")
                        {
                            var value = System.TimeOnly.Parse(item[property.Name.ToString().ToLower()]);
                            property.SetValue(obj, value);
                        }
                        else
                        {
                            errDict.Add(property.Name.ToString(), notSupportedErr.Message);
                        }
                    }
                    catch (Exception LocalErr)
                    {
                        errDict.Add(property.Name.ToString(), LocalErr.Message);
                    }
                }

                foreach (var prop in properties)
                {
                    if (prop.GetValue(obj) == null)
                    {
                        errDict.Add(prop.Name.ToString(), "Property value is null");
                    }
                }
                if (errDict.Count != 0)
                {
                    ObjectsErrorsList.Add(errDict);
                }
                else
                {
                    ValidObjects.Add(obj);
                }
            }
            
            if (ObjectsErrorsList.Count != 0)
            {
                foreach (var err in ObjectsErrorsList)
                {
                    Console.WriteLine("----------------------");
                    foreach (var item in err)
                    {
                        if (item.Key != "General")
                        {
                            Console.WriteLine($"Errors: {item.Key} {item.Value}");
                        }
                    }
                }
            }
            else
            {
                this.fileName = fileName;
                foreach (var obj in ValidObjects)
                {
                    this.AddObject(obj);
                }
                this.RewriteFile();
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
                    var path = @"..\..\..\" + fileName;
                    List<Dictionary<string, string>> items;
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    }
                    this.CreateObjectsFromData(items, path);
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

        public void addObjectFromConsoleGeneric()
        {
            try
            {
                T obj = (T) Activator.CreateInstance(typeof(T));
                
                List<string> errList = new List<string>();
                
                PropertyInfo[] properties = obj.GetType().GetProperties();
                
                Console.WriteLine("Enter this fields: ");
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    Console.WriteLine("Enter value of " + property.Name.ToString() + ": ");
                    var fieldValue = Console.ReadLine();
                    try
                    {
                        var propType = property.PropertyType;
                        var converter = TypeDescriptor.GetConverter(propType);
                        var convertedObject = converter.ConvertFromString(fieldValue);
                        property.SetValue(obj, convertedObject);
                        if (property.Name.ToString().ToLower() == "id")
                        {
                            var sameIdExist = this.container.Exists(x => property.GetValue(x).ToString() == fieldValue);

                            if (sameIdExist)
                            {
                                errList.Add("Same id already exists in the list");
                            }
                        }

                    }
                    catch (NotSupportedException notSupportedErr)
                    {
                        var propType = property.PropertyType;
                        if (propType.ToString() == "System.DateOnly")
                        {
                            var value = System.DateOnly.Parse(fieldValue);
                            property.SetValue(obj, value);
                        } else if(propType.ToString() == "System.TimeOnly")
                        {
                            var value = System.TimeOnly.Parse(fieldValue);
                            property.SetValue(obj, value);
                        }
                        else
                        {
                            errList.Add(notSupportedErr.Message);
                        }
                    }
                    catch (Exception LocalErr)
                    {
                        errList.Add(LocalErr.Message);
                    }
                }

                if (errList.Count == 0)
                {
                    this.container.Add(obj);
                    if (this.fileName != "")
                    {
                        this.RewriteFile();
                    }
                }
                else
                {
                    Console.WriteLine("Some errors occurs: ");
                    foreach (var errMsg in errList)
                    {
                        Console.WriteLine(errMsg);
                    }
                    Console.WriteLine("----------------------");
                }
            }
            
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }


        }

        public void AddObject(T obj)
        {
            this.container.Add(obj);
        }

        public void printAllObjects()
        {
            foreach (var item in this.container)
            {
                PropertyInfo[] properties = item.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    Console.WriteLine(prop.Name.ToString() + ": " + prop.GetValue(item));
                }
            }
        }

        public void Sort()
        {
            Console.WriteLine("Choose the field to sort by it: ");
            string fieldName = Console.ReadLine();
            Console.WriteLine("Sorting............");
            
            T Instance = (T) Activator.CreateInstance(typeof(T));
            
            if (Instance.GetType().GetProperty(fieldName) != null)
            {
                var result = this.container.OrderBy(r => r.GetType().GetProperty(fieldName).GetValue(r, null)).ToList();
                Console.WriteLine("\nAfter sort by part number:");
                foreach (var obj in result)
                {
                    Console.WriteLine(obj);
                }
            }
            else
            {
                Console.WriteLine("There is no such field in the list");
            }

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
