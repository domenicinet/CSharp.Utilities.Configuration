using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domenici.Utilities.Configuration.Tests
{
    [TestClass]
    public class UnitTestConfiguration
    {
        [TestMethod]
        public void TestSettingsManager()
        {
            string test1 = SettingsManager.GetValue("TestSetting");
            Assert.AreEqual("This is a test setting.", test1);

            int test2 = (int)SettingsManager.GetValue("IntegerSetting", typeof(int));
            Assert.AreEqual(100, test2);
        }
    }
}
