using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace watermark_utility_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
            }
        }
    }
}
