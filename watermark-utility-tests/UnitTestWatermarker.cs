using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using Moq;
using iText.Kernel.Pdf;

namespace watermark_utility_tests
{
    [TestClass]
    public class UnitTestWatermarker
    {
        /// <summary>
        /// Verify the correct Console output is returned if empty/missing value for --input
        /// </summary>
        [TestMethod]
        public void TestEmptyInputParameter()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var args = new string[] { "--input", "" };
                watermark_utility.Watermarker.Main(args);

                var output = sw.ToString().Trim();
                Assert.AreEqual("Watermark Utility\r\nThe value for --input is : \r\nThe value for --ouptput is : \r\nThe value for --pages is : all\r\nThe value for --text is : \r\nMissing value for --input", output);
            }
        }

        /// <summary>
        /// Verify the correct Console output is returned if empty/missing value for --output
        /// </summary>
        [TestMethod]
        public void TestEmptyOutputParameter()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var args = new string[] { "--input", "not-valid-input", "--output", "" };
                watermark_utility.Watermarker.Main(args);

                var output = sw.ToString().Trim();
                Assert.AreEqual("Watermark Utility\r\nThe value for --input is : not-valid-input\r\nThe value for --ouptput is : \r\nThe value for --pages is : all\r\nThe value for --text is : \r\nMissing value for --output", output);
            }
        }

        /// <summary>
        /// Verify the correct Console output is returned if empty/missing value for --text
        /// </summary>
        [TestMethod]
        public void TestEmptyTextParameter()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                var args = new string[] { "--input", "not-valid-input", "--output", "not-valid-output", "--text", "" };
                watermark_utility.Watermarker.Main(args);

                var output = sw.ToString().Trim();
                Assert.AreEqual("Watermark Utility\r\nThe value for --input is : not-valid-input\r\nThe value for --ouptput is : not-valid-output\r\nThe value for --pages is : all\r\nThe value for --text is : \r\nMissing value for --text", output);
            }
        }
    }
}
