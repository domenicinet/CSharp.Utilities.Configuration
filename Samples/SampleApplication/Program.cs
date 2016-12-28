using Domenici.Utilities.Configuration;
using SampleApplication.domenici.settings;
using SampleDependency;
using System;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataManager = new DataManager();
            Console.WriteLine(dataManager.GetStartMessage());
            Console.ReadKey();
            Console.WriteLine();

            // Read all settings from referenced library
            Console.WriteLine("Read all settings from referenced library");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"App Version: { SampleApplicationSettings.ApplicationVersion }");
            Console.WriteLine($"String: { SampleApplicationSettings.DataSamples.StringSetting }");
            Console.WriteLine($"Integer: { SampleApplicationSettings.DataSamples.IntegerSetting }");
            Console.WriteLine($"Decimal: { SampleApplicationSettings.DataSamples.DecimalSetting }");
            Console.WriteLine($"Boolean: { SampleApplicationSettings.DataSamples.BooleanSetting }");
            
            // Read all settings from generated class
            Console.WriteLine("Read all settings from generated class");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"String: { Sample.ItemSamples.StringValue }");
            Console.WriteLine($"Integer: { Sample.ItemSamples.IntegerValue }");
            Console.WriteLine($"Decimal: { Sample.ItemSamples.DecimalValue }");
            Console.WriteLine($"Boolean: { Sample.ItemSamples.BooleanValue }");

            // Done
            Console.WriteLine();
            Console.WriteLine(dataManager.GetEndMessage());
            Console.ReadKey();
        }
    }
}
