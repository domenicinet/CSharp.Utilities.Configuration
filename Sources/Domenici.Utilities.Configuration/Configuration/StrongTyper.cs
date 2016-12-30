using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Domenici.Utilities.Configuration
{    
    public class StrongTyper
    {
        #region OutputTypes
        public enum OutputTypes
        {
            SourceCode = 0,
            CompiledLibrary = 1
        }
        #endregion

        #region RunModes
        public enum RunModes
        {
            SingleSourceFile = 0,
            MultipleSourceFiles = 1
        } 
        #endregion

        #region Single source file mode variables
        private string settingsFileContents;
        #endregion

        #region Multiple source files mode variables
        private string appSettingsFolder;
        private string outputFolder;
        private string libraryName;
        #endregion

        private OutputTypes outputType;
        private readonly RunModes runMode;

        private List<SettingProperties> settingsList;

        #region Constructors
        /// <summary>
        /// This constructor prepares the class for the generation of a strongly typed 
        /// C# file obtained by analyzing the given setting file.
        /// </summary>
        /// <param name="settingsFileContents"></param>
        /// <param name="classNamespace"></param>
        /// <param name="className"></param>
        public StrongTyper(string settingsFileContents, string classNamespace, string className)
        {
            this.settingsFileContents = settingsFileContents;
            this.runMode = RunModes.SingleSourceFile;
            this.outputType = OutputTypes.SourceCode;
        }

        /// <summary>
        /// This constructor prepares the class for the generation of a strongly typed 
        /// library (or C# file) obtained by analyzing one or more setting files in a 
        /// given folder.
        /// </summary>
        /// <param name="appSettingsFolder"></param>
        /// <param name="outputFolder"></param>
        /// <param name="libraryName"></param>
        /// <param name="outputType"></param>
        public StrongTyper(string appSettingsFolder, string outputFolder, string libraryName, OutputTypes outputType)
        {

            if (!Directory.Exists(appSettingsFolder))
            {
                throw new DirectoryNotFoundException(string.Format("Cannot find directory: {0}", appSettingsFolder));
            }

            if (!Directory.Exists(outputFolder))
            {
                throw new DirectoryNotFoundException(string.Format("Cannot find directory: {0}", outputFolder));
            }

            this.appSettingsFolder = appSettingsFolder;
            this.outputFolder = outputFolder;
            this.libraryName = libraryName;
            this.runMode = RunModes.MultipleSourceFiles;
            this.outputType = outputType;
        } 
        #endregion

        public void CreateStronglyTypedClass()
        {
            if (this.runMode != RunModes.MultipleSourceFiles)
            {
                throw new InvalidOperationException("Function 'CreateStronglyTypedClass' is not available when run mode is set to 'SingleSourceFile'.");
            }

            Console.WriteLine("Loading settings...");
            LoadSettings();
            Console.WriteLine("Done loading settings.");

            string outputClass = CreateOutput();

            if (OutputTypes.CompiledLibrary == outputType)
            {
                #region Create a compiled .NET library
                Console.WriteLine("Generating library...");

                // Get application path
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri  = new UriBuilder(codeBase);
                string path     = Uri.UnescapeDataString(uri.Path);
                string appPath  = Path.GetDirectoryName(path);

                // Compile Library
                var csc = new CSharpCodeProvider(new Dictionary<string, string>() { 
                    { "CompilerVersion", "v4.0" } 
                });

                var parameters = new CompilerParameters(
                    new[] { 
                    appPath + @"\Domenici.Utilities.Configuration.dll", 
                    "mscorlib.dll", 
                    "System.Core.dll" 
                },
                    string.Format("{0}.domenici.settings.dll", this.libraryName),
                    false);

                parameters.CompilerOptions = "/optimize";

                parameters.GenerateExecutable    = false;
                parameters.GenerateInMemory      = false;
                parameters.TreatWarningsAsErrors = false;

                CompilerResults results = csc.CompileAssemblyFromSource(parameters, outputClass);

                if (results.Errors.Count > 0)
                {
                    StringBuilder errors = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        errors.Append(error.ToString() + "\r\n");
                    }

                    throw new Exception("Compilation errors: " + errors.ToString());
                }

                File.Copy(results.CompiledAssembly.Location, string.Format("{0}\\{1}.domenici.settings.dll", this.outputFolder, this.libraryName), true);
                
                Console.WriteLine("Done generating library.");
                #endregion
            }
            else
            {
                #region Create a C# class source file
                Console.WriteLine("Generating C# source code...");

                string filePath = string.Format("{0}\\{1}.cs", this.outputFolder, this.outputType);
                File.Delete(filePath);

                using (TextWriter tw = File.CreateText(filePath))
                {
                    tw.Write(outputClass);
                }

                Console.WriteLine("Done generating C# source code.");
                #endregion
            }
        }

        public string CreateStronglyTypedOutput()
        {
            if (this.runMode != RunModes.SingleSourceFile)
            {
                throw new InvalidOperationException("Function 'CreateStronglyTypedOutput' is not available when run mode is set to 'MultipleSourceFiles'.");
            }

            Console.WriteLine("Loading settings...");
            LoadSettings();
            Console.WriteLine("Done loading settings.");
            
            Console.WriteLine("Generating C# source code...");
            string outputClass = CreateOutput();
            Console.WriteLine("Done generating C# source code.");

            return outputClass;
        }        

        /// <summary>
        /// Generate a C# class that stong-types the settings.
        /// </summary>
        /// <returns></returns>
        private string CreateOutput()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Tabs(0) + "//------------------------------------------------------------------------------\r\n");
            sb.Append(Tabs(0) + "// <auto-generated>\r\n");
            sb.Append(Tabs(0) + "// This code was automatically generated by the Domenici.Utilities.Configuration tool.\r\n");
            sb.Append(Tabs(0) + "//\r\n");
            sb.Append(Tabs(0) + "// Changes to this file may cause incorrect behavior and will be lost if\r\n");
            sb.Append(Tabs(0) + "// the code is regenerated.\r\n");
            sb.Append(Tabs(0) + "// </auto-generated>\r\n");
            sb.Append(Tabs(0) + "//------------------------------------------------------------------------------\r\n");
            sb.Append(Tabs(0) + "\r\n");
            sb.Append(Tabs(0) + "namespace Domenici.Utilities.Configuration\r\n");
            sb.Append(Tabs(0) + "{\r\n");
            sb.Append(Tabs(1) + string.Format("public static class {0}\r\n", FormatName(this.libraryName + "Settings")));
            sb.Append(Tabs(1) + "{\r\n");

            foreach (SettingProperties setting in this.settingsList)
            {
                if (0 <= setting.Key.IndexOf(SettingsManager.SectionSeparator))
                {
                    string[] sections = setting.Key.Split(new string[] { SettingsManager.SectionSeparator }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < sections.Length; i++)
                    {
                        if (i < sections.Length - 1)
                        {
                            sb.Append(Tabs((i + 2)) + string.Format("public static partial class {0}\r\n", FormatName(sections[i])));
                            sb.Append(Tabs((i + 2)) + "{\r\n");
                        }
                    }

                    sb.Append(Tabs(sections.Length + 1) + string.Format("public static {0} {1} ", string.IsNullOrEmpty(setting.Type) ? "string" : setting.Type, FormatName(sections[sections.Length - 1])));
                    sb.Append("{ get { ");
                    sb.Append(string.Format("return ({0})SettingsManager.GetValue(\"{1}\", typeof({0})); ", setting.Type, setting.Key));
                    sb.Append("} }\r\n");

                    for (int i = sections.Length; i >= 0; i--)
                    {
                        if (i < sections.Length - 1)
                        {
                            sb.Append(Tabs(i + 2) + "}\r\n");
                        }
                    }
                }
                else
                {
                    sb.Append(Tabs(2) + string.Format("public static {0} {1} ", string.IsNullOrEmpty(setting.Type) ? "string" : setting.Type, FormatName(setting.Key)));
                    sb.Append("{ get { ");
                    sb.Append(string.Format("return ({0})SettingsManager.GetValue(\"{1}\", typeof({0}));", setting.Type, setting.Key));
                    sb.Append("} }\r\n");
                }
            }

            sb.Append(Tabs(1) + "}\r\n");
            sb.Append(Tabs(0) + "}\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// Load all sections and settings in memory.
        /// </summary>
        private void LoadSettings()
        { 
            long ticks = DateTime.Now.Ticks;

            try
            {
                settingsList = new List<SettingProperties>();
                SettingsLoader loader = new SettingsLoader(settingsList, SettingsManager.SectionSeparator, true, true);

                switch (this.runMode)
                {
                    case RunModes.SingleSourceFile:
                        {
                            // Load settings file
                            Console.WriteLine("Loading application settings data");
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(this.settingsFileContents);

                            loader.LoadSettings(doc, true);
                            Console.WriteLine("Done loading application settings data.");
                        }

                        break;

                    case RunModes.MultipleSourceFiles:
                        {
                            foreach (string file in Directory.GetFiles(this.appSettingsFolder, "*.appsettings"))
                            {
                                // Load settings file
                                Console.WriteLine($"Loading application settings data on file: {file}");
                                loader.LoadSettings(file, true);
                                Console.WriteLine($"Done loading application settings data on file: {file}");
                            }
                        }

                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new SettingsException("Unexpected error while loading application settings. Please see the inner exception for more details.", e);
            }
        }

        #region Helper methods
        private string Tabs(int count)
        {
            return string.Empty.PadLeft(count * 4);
        }

        private string FormatName(string name)
        {
            string result = name;
            if (Regex.IsMatch(name[0].ToString(), "[0-9]")) result = "_" + name;

            return Regex.Replace(result, "[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled);
        } 
        #endregion
    }
}