using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Homework;

public class Admin : User
{
    public Admin()
    {
        this.Role = "ADMIN";
    }

    public static void PrintAllDraft(TransactionCollection<Transaction> collection,
        List<Dictionary<string, string>> statusesDict)
    {
        foreach (var status in statusesDict)
        {
            if (status["Status"] == "Draft")
            {
                collection.printObjectById(Int32.Parse(status["ID"]));
            }
        }
        Console.WriteLine("Type 'y' if you want to see rejected");
        string input = Console.ReadLine();
        if (input == "y")
        {
            foreach (var status in statusesDict)
            {
                if (status["Status"] == "Rejected")
                {
                    Console.WriteLine("-------------------------------");
                    collection.printObjectById(Int32.Parse(status["ID"]));
                    Console.WriteLine(status["Message"]);
                    Console.WriteLine("--------------------------");
                }
            }
        }
    }
}

public class Staff : User
{
    private int salary;
    private DateOnly first_day_in_company;

    public static void PrintAllApproved(TransactionCollection<Transaction> collection, 
        List<Dictionary<string, string>> statusesDict)
    {
        foreach (var status in statusesDict)
        {
            if (status["Status"] == "Approved")
            {
                collection.printObjectById(Int32.Parse(status["ID"]));
            }
        }
    }
    public static void PrintAllAddedByMe(TransactionCollection<Transaction> collection, 
        List<Dictionary<string, string>> statusesDict, string Email, bool setStatus = false)
    {
        if (setStatus)
        {
            Console.WriteLine("Write status you want to see: ");
            var statusCode = Console.ReadLine();
            foreach (var status in statusesDict)
            {
                if (status["Email"] == Email && status["Status"] == statusCode)
                {
                    collection.printObjectById(Int32.Parse(status["ID"]));
                    Console.WriteLine("Status: " + status["Status"]);
                    if (status["Status"] == "Rejected")
                    {
                        Console.WriteLine("Message: " + status["Message"]);
                    }
                }
            }
        }
        else
        {
            foreach (var status in statusesDict)
            {
                if (status["Email"] == Email)
                {
                    collection.printObjectById(Int32.Parse(status["ID"]));
                }
            }
        }
    }
    public Staff()
    {
        this.Role = "STAFF";
    }

    public int Salary
    {
        get { return salary; }
        set
        {
            if (value >= 1)
            {
                salary = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Salary","Salary must be > 0");
            }
        }
    }

    public DateOnly FirstDayInCompany
    {
        get { return first_day_in_company; }
        set
        {
            if (value <= DateOnly.FromDateTime(DateTime.Today))
            {
                first_day_in_company = value;
            }

            else
            {
                throw new ArgumentOutOfRangeException("FirstDayInCompany","FirstDayInCompany must be before or equal today");
            }
            
        }
    }
}

public abstract class User
{
    private string first_name;
    private string last_name;
    private string email;
    private string role;
    private string password;
    
    public static string GetStringSha256Hash(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }

    public void setHashedPassword(string hashedPsw)
    {
        this.password = hashedPsw;
    }
    
    public string FirstName
    {
        get { return first_name; }
        set
        {
            if (value.Length >= 2 && Regex.IsMatch(value, @"^[a-zA-Z]+$"))
            {
                first_name = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("FirstName","First name must be 2 letters long and" +
                                                                  " contain letters only");
            }
        }
    }
    
    public string LastName
    {
        get { return last_name; }
        set
        {
            if (value.Length >= 2 && Regex.IsMatch(value, @"^[a-zA-Z]+$"))
            {
                last_name = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("LastName","Last name must be 2 letters long and" +
                                                                  " contain letters only");
            }
        }
    }

    public string Email
    {
        get { return email; }
        set
        {
            if (value.Length >= 3 && new EmailAddressAttribute().IsValid(value))
            {
                email = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Email","Email is not valid");
            }
        }
    }

    public string Role
    {
        get { return role; }
        set
        {
            if (value == "ADMIN" || value == "STAFF")
            {
                role = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Role","Role must be STAFF or ADMIN");
            }
        }
    }

    public string Password
    {
        get { return password; }
        set
        {
            if (value.Length >= 8 && Regex.IsMatch(value, "[A-Z]") && Regex.IsMatch(value, "[a-z]"))
            {
                password = GetStringSha256Hash(value);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Password","Password is not valid");
            }
        }
    }
    
}