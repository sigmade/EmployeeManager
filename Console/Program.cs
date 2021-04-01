using DomainLayer;
using System;

namespace Console
{
    // For test queries
    public class Program
    {
        static void Main(string[] args)
        {
            using EmployeesDBContext db = new();
            foreach (var p in db.People)
            {
                System.Console.WriteLine($"Id {p.Id}; Name: {p.FullName}; Age: {p.Age}");
            }
        }
    }
}
