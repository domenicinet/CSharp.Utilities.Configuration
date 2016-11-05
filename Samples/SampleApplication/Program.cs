using Domenici.Utilities.Configuration;
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

            // Read all settings
            string  applicationVersion = SampleApplicationSettings.ApplicationVersion;
            string  stringSetting      = SampleApplicationSettings.DataSamples.StringSetting;
            int     integerSetting     = SampleApplicationSettings.DataSamples.IntegerSetting;
            decimal decimalSetting     = SampleApplicationSettings.DataSamples.DecimalSetting;
            bool    booleanSetting     = SampleApplicationSettings.DataSamples.BooleanSetting;

            Console.WriteLine($"App Version: {applicationVersion}");
            Console.WriteLine($"String: {stringSetting}");
            Console.WriteLine($"Integer: {integerSetting}");
            Console.WriteLine($"Decimal: {decimalSetting}");
            Console.WriteLine($"Boolean: {booleanSetting}");

            // Done
            Console.WriteLine();
            Console.WriteLine(dataManager.GetEndMessage());
            Console.ReadKey();
        }
    }
}
