using System;
using System.Collections.Generic;
using System.Globalization;


abstract class TimeCalculator
{
    public abstract double CalculateTotalHours(DateTime timeIn, DateTime timeOut);
}


class WorkHoursCalculator : TimeCalculator
{
    public override double CalculateTotalHours(DateTime timeIn, DateTime timeOut)
    {
        return (timeOut - timeIn).TotalHours;
    }
}


class Employee
{
    public string EmployeeNumber { get; private set; }
    public string Name { get; set; }
    public string Location { get; private set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
    public double TotalHours { get; set; }
    public string Note { get; set; }

    public Employee(string empNumber, string name, string location)
    {
        EmployeeNumber = empNumber;
        Name = name;
        Location = location;
    }
}

class Program
{

    static Dictionary<string, Employee> EmployeeRecords = new Dictionary<string, Employee>();

    static void Main(string[] args)
    {
        Console.WriteLine("Employee Number: ");
        string empNumber = Console.ReadLine();

        Console.WriteLine("Employee Name: ");
        string empName = Console.ReadLine();

        string location = "";
        while (true)
        {
            Console.WriteLine("Which office are you located? (Philippines / United States / India)");
            location = Console.ReadLine();
            if (location.Equals("Philippines", StringComparison.OrdinalIgnoreCase) ||
                location.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                location.Equals("India", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid location. Try again.");
            }
        }

        DateTime localTime = GetLocalTime(location);

  
        Employee emp = new Employee(empNumber, empName, location);
        emp.TimeIn = localTime;

        Console.WriteLine("\nYou clocked in at:");
        Console.WriteLine($" Date: {emp.TimeIn.ToString("MM/dd/yy")}");
        Console.WriteLine($" Time: {emp.TimeIn.ToString("hh:mm:ss tt")}");


        Console.WriteLine("\nEnter your clock out time (HH:mm, 24-hour format) or press Enter for now:");
        string inputTimeOut = Console.ReadLine();

        if (string.IsNullOrEmpty(inputTimeOut))
        {
          
            emp.TimeOut = emp.TimeIn.AddHours(9);
        }
        else
        {
            DateTime tempTimeOut;
            if (DateTime.TryParseExact(inputTimeOut, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempTimeOut))
            {
                emp.TimeOut = new DateTime(emp.TimeIn.Year, emp.TimeIn.Month, emp.TimeIn.Day, tempTimeOut.Hour, tempTimeOut.Minute, 0);
            }
            else
            {
                Console.WriteLine("Invalid input, defaulting 9 hours shift.");
                emp.TimeOut = emp.TimeIn.AddHours(9);
            }
        }

    
        WorkHoursCalculator calc = new WorkHoursCalculator();
        emp.TotalHours = calc.CalculateTotalHours(emp.TimeIn, emp.TimeOut);

    
        if (emp.TotalHours < 9)
        {
            double remaining = 9 - emp.TotalHours;
            emp.Note = $"Early Out. Hours left: {Math.Round(remaining, 2)} hours";
        }
        else if (emp.TotalHours > 9)
        {
            double extra = emp.TotalHours - 9;
            emp.Note = $"Overtime. Hours extended: {Math.Round(extra, 2)} hours";
        }
        else
        {
            emp.Note = "";
        }

        EmployeeRecords[emp.EmployeeNumber] = emp;

        // Display Employee Log
        Console.WriteLine("\nEmployee Log:");
        Console.WriteLine($"Name: {emp.Name}");
        Console.WriteLine($"Location: {emp.Location}");
        Console.WriteLine($"Time-in: {emp.TimeIn.ToString("MM/dd/yy hh:mm:ss tt")}");
        Console.WriteLine($"Time-out: {emp.TimeOut.ToString("MM/dd/yy hh:mm:ss tt")}");
        Console.WriteLine($"Total Hours: {Math.Round(emp.TotalHours, 2)}");
        if (!string.IsNullOrEmpty(emp.Note))
            Console.WriteLine($"Note: {emp.Note}");
    }

   
    static DateTime GetLocalTime(string location)
    {
        TimeZoneInfo tz;
        if (location.Equals("Philippines", StringComparison.OrdinalIgnoreCase))
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"); // Philippines shares with Singapore
        }
        else if (location.Equals("United States", StringComparison.OrdinalIgnoreCase))
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // US Eastern
        }
        else // India
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        }

        return TimeZoneInfo.ConvertTime(DateTime.Now, tz);
    }
}