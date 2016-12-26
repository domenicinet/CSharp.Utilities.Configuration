using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Xml;

namespace Domenici.Utilities.Configuration
{
    /// <summary>
    /// This class provices access to global application settings.
    /// </summary>
    /// <remarks>
    /// Your .NET configuration file must have the following key in the appSettings section:
    /// <![CDATA[
    ///         <appSettings>
    ///             <!-- This is the full path of the configuration file: e.g. "D:\webroot\myapp\default.domenici.settings" -->
    ///             <add key="Domenici.Net:Configuration:DefaultAppSettingsPath" value="YOUR_CONFIGURATION_FILE_HERE" />
    ///         </appSettings>
    /// ]]>
    /// And the following tags in the configuration section, in order to ensure that clients
    /// won't be able to download the configuration file:
    /// <![CDATA[
    ///         <security>
    ///             <requestFiltering>            
    ///                 <hiddenSegments>
    ///                     <!-- This is the name of the configuration file -->
    ///                     <add segment="domenici.settings" />
    ///                 </hiddenSegments>
    ///             </requestFiltering>
    ///         </security>
    /// ]]>
    /// </remarks>
    public static class SettingsManager
    {
        public const string SectionSeparator = "/";

        private const string AppSettingsPathKeyName = "Domenici.Net:Configuration:AppSettingsPath";
        private const int DefaultRefreshTimeoutIntervalInMinutes = 360; // 6 hours

        private static Timer timeoutTimer = null;

        #region Memory usage estimate
        private static long memorySize = 0;

        /// <summary>
        /// Returns an estimate of the memory required to store all the configuration data.
        /// </summary>
        public static long EstimatedTotalMemoryUsage
        {
            get
            {
                if (memorySize == 0)
                {
                    Debug.WriteLine("Start estimating memory footprint...");
                    long ticks = DateTime.Now.Ticks;

                    EstimateMemoryUsage();

                    decimal elapsedTimeInSeconds = (DateTime.Now.Ticks - ticks) / 10000000;

                    string message = string.Format("Memory footprint estimate done in {0} seconds.", elapsedTimeInSeconds);
                    Debug.WriteLine(message);

                    if (elapsedTimeInSeconds > 0.1M)
                    {
                        Debug.WriteLine(string.Format("Memory footprint estimate executed unexpectedly slowly: {0} seconds.", elapsedTimeInSeconds));
                    }
                }

                return memorySize * 2; // We count 2 bytes per character.
            }
        }

        private static void EstimateMemoryUsage()
        {
            foreach (SettingProperties item in settingsList)
            {
                memorySize += item.Key.Length;
                memorySize += item.Value.Length;
                memorySize += item.Type.Length;
            }
        } 
        #endregion

        private static List<SettingProperties> settingsList;

        static SettingsManager()
        {
            LoadSettings();
        }

        /// <summary>
        /// Load all sections and settings in memory.
        /// </summary>
        private static void LoadSettings()
        { 
            string message = "Start loading application settings data...";
            Debug.WriteLine(message);
            long ticks = DateTime.Now.Ticks;

            XmlDocument doc = null;
            
            #region Validation: Throw an exception is settings files cannot be found. 
            //get the default path
            string defaultPath = ConfigurationManager.AppSettings[AppSettingsPathKeyName];
            
            if (String.IsNullOrEmpty(defaultPath))
                throw new SettingsException($"Missing key '{AppSettingsPathKeyName}' in the appSettings of the default configuration file.", null);

            if (!Path.IsPathRooted(defaultPath))
            {
                //if relative path, get full path
                defaultPath = Path.GetFullPath(defaultPath);
            }

            if (!System.IO.Directory.Exists(defaultPath))
            {
                throw new SettingsException($"Missing configuration folder. The folder '{defaultPath}' in key '{AppSettingsPathKeyName}' in the appSettings of the configuration file does not exist.", null);
            }
            #endregion

            try
            {
                settingsList = new List<SettingProperties>();
                SettingsLoader loader = new SettingsLoader(settingsList, SectionSeparator);

                // Load all settings files  
                foreach (string file in Directory.GetFiles(defaultPath, "*.appsettings"))
                {
                    // Load settings file
                    Console.WriteLine($"Loading application settings data on file: {file}");
                    loader.LoadSettings(file);
                    Console.WriteLine($"Done loading application settings data on file: {file}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw new SettingsException("Unexpected error while loading application settings. Please see the inner exception for more details.", e);
            }

            #region Refresh timer
            try
            {
                // Read the timeout information, if any
                int intervalInMinutes = DefaultRefreshTimeoutIntervalInMinutes;

                try
                {
                    XmlNode timeoutNode = doc.DocumentElement.Attributes["timeout"];

                    if (null != timeoutNode)
                    {
                        intervalInMinutes = int.Parse(timeoutNode.Value);
                    }
                    else
                    {
                        message = string.Format("The timeout node is missing. Using default value: {0} minutes.", DefaultRefreshTimeoutIntervalInMinutes);
                        Debug.WriteLine(message);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Cannot parse the timeout node. Ensure that the value is an integer (details follow): {e.ToString()}");
                }

                if (0 == intervalInMinutes)
                {
                    intervalInMinutes = DefaultRefreshTimeoutIntervalInMinutes;

                    message = string.Format("The timeout cannot be set to 0. Using default value: {0} minutes.", DefaultRefreshTimeoutIntervalInMinutes);
                    Debug.WriteLine(message);
                }

                timeoutTimer = new Timer(intervalInMinutes * 1000 * 60);
                timeoutTimer.Elapsed += timeoutTimer_Elapsed;
                timeoutTimer.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw new SettingsException("Unexpected error while setting up the refresh timer. Please see the inner exception for more details.", e);
            }

            decimal elapsedTimeInSeconds = (DateTime.Now.Ticks - ticks) / 10000000;

            message = string.Format("Application settings data loaded in {0} seconds.", elapsedTimeInSeconds);
            Debug.WriteLine(message);

            if (elapsedTimeInSeconds > 0.1M)
            {
                Debug.WriteLine(string.Format("Application settings data loaded unexpectedly slowly: {0} seconds.", elapsedTimeInSeconds));
            } 
            #endregion
        }

        static void timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeoutTimer.Stop();
            timeoutTimer.Dispose();

            LoadSettings();
        }

        /// <summary>
        /// Retrieve a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return (string)GetValue(key, typeof(string));
        }

        /// <summary>
        /// Retrieve a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetValue(string key, Type t)
        {
            try
            {
                var data = settingsList.Where(x => x.Key.Contains(key)).FirstOrDefault();
                return Convert.ChangeType(data.Value, t);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw new SettingsException("Unexpected error while retrieving application settings. Please see the inner exception for more details.", e);
            }
        }
    }
}