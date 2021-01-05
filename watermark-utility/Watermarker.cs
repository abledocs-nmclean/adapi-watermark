using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace watermark_utility
{
    class Watermarker
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Watermark Utility");
            var pageOptions = new ArrayList();
            pageOptions.Add("all");

            // Configure CommandLine options
            var rootCommand = new RootCommand
            {
                new Option<String>(
                    "--input",
                    description: "The path to the file to add a watermark to."
                ),
                new Option<String>(
                    "--output",
                    description: "The path to write the output file that has been watermarked to."
                ),
                new Option<String>(
                    "--pages",
                    description: "The pages that should be watermarked. Accepted values: 'all'. Future values: 'even', 'odd'.",
                    getDefaultValue: () => "all"
                ),
                new Option<String>(
                    "--text",
                    description: "The text to use as a watermark."
                )
            };

            rootCommand.Description = "A simple utility that adds a watermark to a PDF and writes a new copy.";

            rootCommand.Handler = CommandHandler.Create<String, String, String, String>((input, output, pages, text) =>
            {
                Console.WriteLine($"The value for --input is : {input}");
                Console.WriteLine($"The value for --ouptput is : {output}");
                Console.WriteLine($"The value for --pages is : {pages}");
                Console.WriteLine($"The value for --text is : {text}");

                if (String.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Missing value for --input");
                    return;
                }

                if (String.IsNullOrWhiteSpace(output))
                {
                    Console.WriteLine("Missing value for --output");
                    return;
                }

                if (String.IsNullOrWhiteSpace(pages))
                {
                    Console.WriteLine("Missing value for --pages");
                    return;
                }
                else if (pageOptions.Contains(pages.ToLower()) == false)
                {
                    Console.WriteLine("Invalid value for --pages");
                    return;
                }

                if (String.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("Missing value for --text");
                    return;
                }

                PdfDocument document = new PdfDocument(new PdfReader(input), new PdfWriter(output));

                addWatermarkToPdf(document, pages, text);

                document.Close();

                Console.WriteLine("Watermarking completed");
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        /// <summary>
        /// Adds a watermark to a PDF
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pages"></param>
        /// <param name="watermarkTextContent"></param>
        protected static void addWatermarkToPdf(PdfDocument document, String pages, String watermarkTextContent)
        {
            Paragraph watermarkText = new Paragraph(watermarkTextContent);
            watermarkText.SetFont(PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA)));
            watermarkText.SetFontSize(15);
            watermarkText.SetFontColor(ColorConstants.BLACK);

            if (pages == "all")
                for (int i = 1; i < document.GetNumberOfPages() + 1; i++)
                    addWatermarkToPage(document, i, watermarkText);
            else
                Console.WriteLine("Did not watermark any pages!");
        }

        /// <summary>
        /// Adds the provided text to the specified page in the PdfDocument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pageNumber"></param>
        /// <param name="text"></param>
        protected static void addWatermarkToPage(PdfDocument document, int pageNumber, Paragraph text)
        {
            PdfPage page = document.GetPage(pageNumber);
            Canvas pageCanvas = new Canvas(page, page.GetPageSize()).
                ShowTextAligned(text, 100, 100, pageNumber, TextAlignment.CENTER, VerticalAlignment.TOP, 45);
            pageCanvas.Close();
        }
    }
}
