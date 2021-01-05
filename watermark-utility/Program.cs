using iText.Kernel.Pdf;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace watermark_utility
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Configure CommandLine options
            var rootCommand = new RootCommand
            {
                new Option<String>(
                    "--input",
                    description: "The path to the file to add a watermark to"
                ),
                new Option<String>(
                    "--output",
                    description: "The path to write the output file that has been watermarked to"
                )
            };

            rootCommand.Description = "A simple utility that adds a watermark to a PDF and writes a new copy.";

            rootCommand.Handler = CommandHandler.Create<String, String>((input, output) =>
            {
                Console.WriteLine($"The value for --input is : {input}");
                Console.WriteLine($"The value for --ouptput is : {output}");

                PdfDocument document = new PdfDocument(new PdfReader(input), new PdfWriter(output));
                Console.WriteLine("Number of pages: " + document.GetNumberOfPages());
                document.Close();
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        /// <summary>
        /// Adds a watermark to a PDF
        /// </summary>
        protected void addWatermarkToPdf()
        {

        }
    }
}
