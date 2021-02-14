using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLScriptsGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> types = new Dictionary<string, string>
            {
                {"System.Int32", "INT"},
                {"System.Double", "DECIMAL"},
                { "System.String", "NVARCHAR(MAX)"},
                { "System.Char", "NVARCHAR(MAX)"},
                {"System.Boolean", "BOOLEAN"}
            };

            string path = @"C:\Users\ww\source\repos\SQLclass\SQLclass\bin\Debug\netcoreapp3.1\SQLclass.dll";
            
            while (true)
            {
                Console.WriteLine(@"Путь по умолчанию: C:\Users\ww\source\repos\SQLclass\SQLclass\bin\Debug\netcoreapp3.1\SQLclass.dll");
                Console.WriteLine("Введите полный путь до сборки:");
                string input = Console.ReadLine();
                if (input.Length < 4)
                    break;

                string fileType = input.Substring(input.Length - 4);
                if (fileType == ".exe" || fileType == ".dll")
                {
                    path = input;
                    break;
                }

                Console.ReadLine();
                Console.Clear();
            }

            Assembly myLibrary;
            try
            {
                myLibrary = Assembly.LoadFile(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return;
            };

            int countOfObjectMethods = 4;
            
            foreach (Type type in myLibrary.GetTypes())
            {
                string sqlScript = "\nCREATE TABLE ";

                int methodCount = type.GetMethods().Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")).Count();
                
                if (methodCount != countOfObjectMethods)
                {
                    continue;
                }

                sqlScript += type.Name + " \n(\n";
                if (type.IsClass)
                {
                    int numberProperty = new int();
                    foreach (var member in type.GetMembers())
                        if (member is PropertyInfo)
                            numberProperty++;

                    int counter = new int();
                    foreach (var member in type.GetMembers())
                    {                      
                        if (member is PropertyInfo)
                        {
                            counter++;
                            var propertyInfo = member as PropertyInfo;
                            string sqlType = string.Empty;
                            types.TryGetValue(propertyInfo.PropertyType.FullName, out sqlType);
                            sqlScript += $"\t{propertyInfo.Name} {sqlType} NOT NULL";
                            if (propertyInfo.Name.ToLower() == "id")
                                sqlScript += " IDENTITY PRIMARY KEY";
                            if (counter < numberProperty)
                                sqlScript += ",";
                            sqlScript += "\n";
                        }

                    }
                    sqlScript += ");";

                    Console.WriteLine(sqlScript);
                }
            }
        }    
    }
}
