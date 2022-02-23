using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;

using System.Reflection;
using System.Reflection.Metadata;

namespace Homework
{
    public class Transaction:  IComparable<Transaction>
    {
        private int id;
        private string payer_name;
        private ulong card_number;
        private string month;
        private string year;
        private int cvc;
        private DateTime payment_date;
        private Decimal amount;

        public override String ToString()
        {
            return $"{this.ID},\n{this.PayerName},\n{this.CardNumber.ToString()}," +
                   $"\n{this.Month},\nyear = {this.Year},\n{this.CVC.ToString()},\n" +
                   $"{this.PaymentDate.Year.ToString()}-{this.PaymentDate.Month.ToString()}-" +
                   $"{this.PaymentDate.Day.ToString()},\n{this.Amount.ToString()}";
        }

        public int CompareTo(Transaction comparePart) =>
            comparePart == null ? 1 : ID.CompareTo(comparePart.ID);
        public Dictionary<string, string> Read()
        {
            Dictionary<string, string> valuesDict = new Dictionary<string, string>(); 
            PropertyInfo[] properties = this.GetProperties();
            
            for (int i = 0; i < properties.Length; i++)
            {
                string propertyValue;
                string propertyName = properties[i].Name;

                switch (propertyName)
                {
                    case "CardNumber":
                        propertyValue = InputField(propertyName, "16 digits number");
                        break;
                    case "Month":
                        propertyValue = InputField(propertyName, "2 digits like 01, 02, 03 etc....");
                        break;
                    case "CVC":
                        propertyValue = InputField(propertyName, "### or ####");
                        break;
                    case "PaymentDate":
                        propertyValue = InputField(propertyName, "year-month-day");
                        break;
                    case "Amount":
                        propertyValue = InputField(propertyName, "somenumber,##");
                        break;
                    default:
                        propertyValue = InputField(propertyName);
                        break;
                }
                
                
                valuesDict.Add(propertyName, propertyValue);
            }

            Dictionary<string, string> errorsDict = this.SetValuesFromDict(valuesDict);
            return errorsDict;
        }

        public Dictionary<string, string> GetValuesInDict()
        {
            PropertyInfo[] props = this.GetProperties();
            Dictionary<string, string> propsDict = new Dictionary<string, string>();

            foreach (var prop in props)
            {
                propsDict.Add(prop.Name, prop.GetValue(this).ToString());
            }

            return propsDict;
        }
        public Dictionary<string, string> SetValuesFromDict(Dictionary<string, string> valuesDict)
        {
            Dictionary<string, string> errorsDict = new Dictionary<string, string>();

            foreach (var item in valuesDict)
            {
                try
                {
                    var property = this.GetType().GetProperty(item.Key);
                    var propType = property.PropertyType;
                    var converter = TypeDescriptor.GetConverter(propType);
                    var convertedObject = converter.ConvertFromString(item.Value);
                    property.SetValue(this, convertedObject);
                }
                catch (TargetInvocationException error)
                {
                    errorsDict.Add(item.Key, error.InnerException.Message);
                }
                catch (NotSupportedException)
                {
                    errorsDict.Add(item.Key, $"{item.Key} is wrong type");
                }
                catch (ArgumentNullException)
                {
                    errorsDict.Add(item.Key, "There is no such property");
                }
                catch (ArgumentException)
                {
                    errorsDict.Add(item.Key, $"{item.Key} is wrong type or format");
                }
                catch (FormatException)
                { 
                    errorsDict.Add(item.Key, $"{item.Key} is wrong type or format");
                }
            }
            return errorsDict;
        }

        public PropertyInfo[] GetProperties()
        {
            return this.GetType().GetProperties();
        }

        public string InputField(string fieldName, string format = null)
        {
            string enter = $"Enter {fieldName}";
            if (format != null)
            {
                enter += $" in a {format} format";
            }
            Console.Write(enter + ": ");
            string var = Console.ReadLine();

            return var;
        }
        
        public int ID
        {
            get { return id; }
            set
            {
                if (value > 0)
                {
                    id = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ID","Id value must be more than zero");
                }
            }
        }

        public string PayerName
        {
            get { return payer_name;  }
            set
            {
                if (value.Length > 3 && value.Length < 15 && Char.IsUpper(value, 0))
                {
                    payer_name = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PayerName","payer name length must be more than 3 and less than 15, also it must start" +
                                                          "from capital letter");
                }
            }
        }

        public ulong CardNumber
        {
            get { return card_number;  }
            set
            {
                if ( value.ToString().Length == 16 )
                {
                    card_number = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("CardNumber","card number must be 16 digits number");
                }
            }
        }

        public string Month
        {
            get { return month;  }
            set
            {
                if (value.Length <= 2 && value.Length > 0 && Int32.Parse(value) > 0 && Int32.Parse(value) < 13)
                {
                    month = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Month","month must be a number from 01 to 12");
                }
            }
        }

        public string Year
        {
            get { return year;  }
            set
            {
                if (value.Length == 4 && Int32.Parse(value) >= 2022)
                {
                    year = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Year","Year must be more than 2022");
                }
            }
        }

        public int CVC
        {
            get { return cvc;  }
            set
            {
                if (value > 99 && value <= 9999)
                {
                    cvc = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("CVC","CVC must be in a format ### or ####");
                }
            }
        }

        public DateTime PaymentDate
        {
            get { return payment_date;  }
            set
            {
                if (value.Year <= 2022 && value.Year >= 2015 && (value.Year != 2022 || value.Month < DateTime.Today.Month 
                        || (value.Month == DateTime.Today.Month && value.Day <= DateTime.Today.Day)))
                {
                    payment_date = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("PaymentDate","Payment date must be in a format year-month-day");
                }
            }
        }

        public Decimal Amount
        {
            get { return amount;  }
            set
            {
                if (value > 0)
                {
                    amount = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Amount", "Amount must be more than zero");
                }
            }
        }
    }
}