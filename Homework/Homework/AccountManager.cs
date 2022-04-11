using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Homework;

public class AccountManager
{
    private TransactionCollection<Transaction> _collection;
    private User _user;
    private List<User> _user_collection;
    private string usersCollectionFileName;
    private string statusesFileName;
    private List<Dictionary<string, string>> statusesDict;

    public AccountManager(string fileName = "users.txt", string statusesFileName = "statuses.txt")
    {
        this.usersCollectionFileName = fileName;
        this.usersCollectionFileName = @"..\..\..\" + this.usersCollectionFileName;
        this.statusesFileName = @"..\..\..\" + statusesFileName;
        this._user_collection = new List<User>();
        this._collection = new TransactionCollection<Transaction>();
        this.statusesDict = new List<Dictionary<string, string>>();
        this._collection.startWorkingWithFile();
        
        this.CollectDataFromFile();
        this.CollectStatusDataFromFile();
    }

    public void CollectStatusDataFromFile()
    {
        var path = this.statusesFileName;
        List<Dictionary<string, string>> items;
        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
        }

        this.statusesDict = items;
    }

    public void RewriteStatusesFile()
    {
        string jsonString = JsonConvert.SerializeObject( this.statusesDict, Formatting.Indented);
        // Set a variable to the Documents path.
        var path = this.statusesFileName;
        using (StreamWriter writer = new StreamWriter(path, false)){ 
            writer.Write(jsonString);
        }
    }
    public void run()
    {
        while (true)
        {
            var menu = String.Join("\n", getAccManagerMenu().ToArray());
            Console.WriteLine(menu);
            string command = Console.ReadLine();
            if (this._user == null)
            {
                this.handleStartCommand(command);
            }
            else if(this._user.Role == "STAFF")
            {
                this.handleStaffCommand(command);    
            }
            else if(this._user.Role == "ADMIN")
            {
                this.handleAdminCommand(command);   
            }
        }
    }

    public void handleAdminCommand(string command)
    {
        switch (command)
        {
            case "logout":
                this.Loggout();
                break;
            case "print all draft":
                this.PrintAllDraft();
                break;
            case "change status":
                this.ChangeStatus();
                break;
            case "exit":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("There is no such command in the list, try again");
                break;
        }
    }

    public void ChangeStatus()
    {
        Console.WriteLine("Input item ID: ");
        string id = Console.ReadLine();
        if (this._user != null && this._user.Role == "ADMIN")
        {
            Dictionary<string, string> SD = null;
            bool found = false;
            foreach (var statusDict in this.statusesDict)
            {
                if (id == statusDict["ID"] && statusDict["Status"] == "Draft")
                {
                    found = true;
                    SD = statusDict;
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("There is no obj with such id or this object is not valid for you");
            } else if (SD != null)
            {
                var index = this.statusesDict.FindIndex(x => x == SD);
                Console.WriteLine("Enter new status(Rejected or Approved)");
                string status = Console.ReadLine();
                if (status == "Approved")
                {
                    SD["Status"] = "Approved";
                    this.statusesDict[index] = SD;
                    this.RewriteStatusesFile();
                }
                else if (status == "Rejected")
                {
                    Console.WriteLine("Enter the message");
                    string message = Console.ReadLine();
                    SD["Status"] = "Rejected";
                    SD["Message"] = message;
                    this.statusesDict[index] = SD;
                    this.RewriteStatusesFile();
                }
                else
                {
                    Console.WriteLine("You entered impossible statuses");
                }
            }
        }
    }

    public void PrintAllDraft()
    {
        if (this._user is Admin)
        {
            Admin.PrintAllDraft(this._collection, this.statusesDict);
        }
    }
    public void handleStaffCommand(string command)
    {
        switch (command)
        {
            case "logout":
                this.Loggout();
                break;
            case "run arr":
                this.RunArrayCommand();
                break;
            case "added by me":
                this.PrintAllAddedByUser();
                break;
            case "added but status":
                this.PrintAllAddedByUserButStatus();
                break;
            case "print all approved":
                this.PrintAllApproved();
                break;
            case "exit":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("There is no such command in the list, try again");
                break;
        }
    }

    public void PrintAllApproved()
    {
        if (this._user is Staff)
        {
            Staff.PrintAllApproved(this._collection, this.statusesDict);
        }
    }

    public void PrintAllAddedByUser()
    {
        if (this._user is Staff)
        {
            Staff.PrintAllAddedByMe(this._collection, this.statusesDict, this._user.Email);
        }
    }

    public void PrintAllAddedByUserButStatus()
    {
        if (this._user is Staff )
        {
            Staff.PrintAllAddedByMe(this._collection, this.statusesDict, this._user.Email, true);
        }
    }

    public void RunArrayCommand()
    {
        if (this._user != null && this._user.Role == "STAFF")
        {
            var menu = String.Join("\n", this._collection.getMenu().ToArray());
            Console.WriteLine(menu);
            string command = Console.ReadLine();
            if (command == "add")
            {
                try
                {
                    var obj = this._collection.addObjectFromConsoleGeneric();
                    var id = obj.ID;
                    Dictionary<string, string> statusDict = new Dictionary<string, string>();
                    statusDict.Add("Email", this._user.Email);
                    statusDict.Add("Status", "Draft");
                    statusDict.Add("ID", id.ToString());
                    this.statusesDict.Add(statusDict);
                    this.RewriteStatusesFile();
                } catch(Exception){}
            }
            else if (command == "edit")
            {
                Console.WriteLine("Enter id of the object you want to edit: ");
                var id = Console.ReadLine();
                Dictionary<string, string> SD = null;
                bool found = false;
                foreach (var statusDict in this.statusesDict)
                {
                    if (id == statusDict["ID"] && this._user.Email == statusDict["Email"])
                    {
                        found = true;
                        this._collection.editObjectById(Int32.Parse(id));
                        SD = statusDict;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("There is no obj with such id or this object is not valid for you");
                } else if (SD != null)
                {
                    var index = this.statusesDict.FindIndex(x => x == SD);
                    SD["Status"] = "Draft";
                    SD.Remove("Message");
                    this.statusesDict[index] = SD;
                    this.RewriteStatusesFile();
                }
            }
            else if (command == "remove")
            {
                Console.WriteLine("Enter id of the object you want to remove: ");
                var id = Console.ReadLine();
                Dictionary<string, string> value = null;
                bool found = false;
                foreach (var statusDict in this.statusesDict)
                {
                    if (id == statusDict["ID"] && this._user.Email == statusDict["Email"] &&
                        statusDict["Status"] == "Draft")
                    {
                        found = true;
                        value = statusDict;
                        this._collection.removeObjectById(Int32.Parse(id));
                        break;
                    }

                }

                if (!found)
                {
                    Console.WriteLine("There is no obj with such id or this object is not valid for you");
                }
                else if (value != null)
                {
                    this.statusesDict.Remove(value);
                    this.RewriteStatusesFile();
                }
            }
            else
            {
                Console.WriteLine("There is no such command in the list");
            }
        }
    }

    public void Loggout()
    {
        this._user = null;
    }
    public void handleStartCommand(string command)
    {
        switch (command)
        {
            case "register":
                this.RegisterNewAccount();
                break;
            case "login":
                this.LoginIntoAccount();
                break;
            case "exit":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("There is no such command in the list, try again");
                break;
        }
    }

    public void LoginIntoAccount()
    {
        Console.WriteLine("Enter email: ");
        string email = Console.ReadLine();
        Console.WriteLine("Enter password");
        string passw = Console.ReadLine();
        string hashedPassw = User.GetStringSha256Hash(passw);

        bool LoggedIn = false;
        foreach (var obj in this._user_collection)
        {
            if (obj.Email == email && obj.Password == hashedPassw)
            {
                this._user = obj;
                LoggedIn = true;
                break;
            }
        }

        if (LoggedIn)
        {
            Console.WriteLine("Youve succesfully logged in");
        }
        else
        {
            Console.WriteLine("Email or Password doesnt match");
        }
    }
    public void RegisterNewAccount()
    {
        var obj = new Staff();
        try
        {
            List<string> errList = new List<string>();
                
            PropertyInfo[] properties = obj.GetType().GetProperties();
            Console.WriteLine("Enter this fields: ");
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.Name != "Role")
                {
                    Console.WriteLine("Enter value of " + property.Name.ToString() + ": ");
                    string fieldValue = Console.ReadLine();
                    try
                    {
                        var propType = property.PropertyType;
                        var converter = TypeDescriptor.GetConverter(propType);
                        var convertedObject = converter.ConvertFromString(fieldValue);
                        property.SetValue(obj, convertedObject);
                        if (property.Name.ToString().ToLower() == "email")
                        {
                            var sameIdExist =
                                this._user_collection.Exists(x => property.GetValue(x).ToString() == fieldValue);

                            if (sameIdExist)
                            {
                                errList.Add("Same email already exists in the list");
                            }
                        }

                    }
                    catch (NotSupportedException notSupportedErr)
                    {
                        try
                        {
                            var propType = property.PropertyType;
                            if (propType.ToString() == "System.DateOnly")
                            {
                                var value = System.DateOnly.Parse(fieldValue);
                                property.SetValue(obj, value);
                            }
                            else
                            {
                                errList.Add(notSupportedErr.Message);
                            }
                        }
                        catch (Exception insideExc)
                        {
                            errList.Add(insideExc.InnerException.Message);
                        }
                    }
                    catch (Exception LocalErr)
                    {
                        errList.Add(LocalErr.InnerException.Message);
                    }
                }
            }
            if (errList.Count == 0)
            {
                this.AddObject(obj);
                this.RewriteFile();
                Console.WriteLine("You have succesfully created new account");
                this._user = obj;
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
            
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
    }

    public void CollectAccsFromData(List<Dictionary<string, string>> items)
    {
        List<User> ValidObjects = new List<User>();

        for (int i = 0; i < items.Count(); i++)
        {
            Dictionary<string, string> item = items[i];
            Dictionary<string, string> lowercase_dict = new Dictionary<string, string>();
            foreach (var kv in item)
            {
                lowercase_dict[kv.Key.ToString().ToLower()] = kv.Value;
            }

            item = lowercase_dict;
            var role = item["role"];
            if (role == "ADMIN")
            {
                Admin obj = new Admin();
                obj.FirstName = item["firstname"];
                obj.LastName = item["lastname"];
                obj.Email = item["email"];
                obj.setHashedPassword(item["password"]);
                ValidObjects.Add(obj);
            } 
            else if (role == "STAFF")
            {
                Staff obj = new Staff();
                DateOnly FirstDayInCompany = System.DateOnly.Parse(item["firstdayincompany"]);
                obj.FirstDayInCompany = FirstDayInCompany;
                int Salary = Int32.Parse(item["salary"]);
                obj.Salary = Salary;
                obj.FirstName = item["firstname"];
                obj.LastName = item["lastname"];
                obj.Email = item["email"];
                obj.setHashedPassword(item["password"]);
                ValidObjects.Add(obj);
            }
        }
        foreach (var obj in ValidObjects)
        {
            this.AddObject(obj);
        }
        this.RewriteFile();
    }
    
    public void RewriteFile()
    {
        List<Dictionary<string, string>> objects = new List<Dictionary<string, string>>();
        foreach (var obj in this._user_collection)
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
        var path = this.usersCollectionFileName;
        using (StreamWriter writer = new StreamWriter(path, false)){ 
            writer.Write(jsonString);
        }
    }

    public void AddObject(User obj)
    {
        this._user_collection.Add(obj);
    }

    public void CollectDataFromFile()
    {
        var path = this.usersCollectionFileName;
        List<Dictionary<string, string>> items;
        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
        }
        this.CollectAccsFromData(items);
    }
    public List<string> getAccManagerMenu()
    {
        if (this._user == null)
        {
            List<string> menu = getNonLoggedInUserMenu();
            return menu;
        }
        else if(this._user.Role == "STAFF")
        {
            List<string> menu = getLoggedInUserStaffMenu();
            return menu;
        }
        else
        {
            List<string> menu = getLoggedInUserAdminMenu();
            return menu;  
        }
    }

    public List<string> getLoggedInUserAdminMenu()
    {
        List<string> menu = new List<string>();
        menu.Add("type 'logout' to log out from your account");
        menu.Add("type 'print all draft' to print all drafted or rejected(optionally)");
        menu.Add("type 'change status' to change status of drafted items");
        menu.Add("type 'exit' to end program");
        return menu;
    }
    public List<string> getLoggedInUserStaffMenu()
    {
        List<string> menu = new List<string>();
        menu.Add("type 'logout' to log out from your account");
        menu.Add("type 'added by me' to print all added objects by you");
        menu.Add("type 'added but status' to print all added objects by you with custom status");
        menu.Add("type 'print all approved' to print all approved objects by all users");
        menu.Add("type 'run arr' to run any of array commands(add, edit or delete object)");
        menu.Add("type 'exit' to end program");
        return menu;
    }

    public List<string> getNonLoggedInUserMenu()
    {
        List<string> menu = new List<string>();
        menu.Add("type 'register' to register new account");
        menu.Add("type 'login' to login into your account");
        menu.Add("type 'exit' to end program");
        return menu;
    }
}