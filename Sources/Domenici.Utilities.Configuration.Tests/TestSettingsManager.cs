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
            string test1 = SettingsManager.GetValue("TestSetting1");
            Assert.AreEqual("This is a test setting.", test1);

            int test2 = (int)SettingsManager.GetValue("IntegerSetting1", typeof(int));
            Assert.AreEqual(100, test2);

            test1 = SettingsManager.GetValue("TestSetting2");
            Assert.AreEqual("This is a test setting.", test1);

            test2 = (int)SettingsManager.GetValue("IntegerSetting2", typeof(int));
            Assert.AreEqual(100, test2);

            test1 = SettingsManager.GetValue("TestSetting3");
            Assert.AreEqual("This is a test setting.", test1);

            test2 = (int)SettingsManager.GetValue("IntegerSetting3", typeof(int));
            Assert.AreEqual(100, test2);
        }
    }
}
