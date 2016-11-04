using System;

namespace Domenici.Utilities.Configuration
{
    public class SettingsException : Exception
    {
        public SettingsException(string message, Exception innerException) : base(message, innerException) { }
    }
}