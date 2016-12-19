using Domenici.Utilities.Configuration;
using System;

namespace DomeniciSettingsManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating Configuration code.");

            try
            {
                string targetLibrary = string.Format("{0}.domenici.settings.dll", args[2]);

                Console.WriteLine("Source path: " + args[0]);
                Console.WriteLine("Target path: " + args[1] + "\\" + targetLibrary);
                Console.WriteLine("Output type: " + args[3]);

                StrongTyper st = new StrongTyper(args[0], args[1], args[2], args[3] == "/library" ? StrongTyper.OutputTypes.CompiledLibrary : StrongTyper.OutputTypes.SourceCode);
                st.CreateStronglyTypedClass();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating Configuration code: " + e.ToString());   
            }

            Console.WriteLine("Done generating Configuration code.");
        }
    }
}
