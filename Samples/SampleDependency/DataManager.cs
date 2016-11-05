using System;
using Domenici.Utilities.Configuration;

namespace SampleDependency
{
    public class DataManager
    {
        public string GetStartMessage()
        {
            return string.Format(SampleDependencySettings.SampleDependency.WelcomeMessage, SampleDependencySettings.SampleDependency.ApplicationName);
        }

        public string GetEndMessage()
        {
            return SampleDependencySettings.SampleDependency.DoneMessage;
        }
    }
}
