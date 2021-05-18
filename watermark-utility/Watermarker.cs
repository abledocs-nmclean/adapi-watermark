using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Resources;
using System.Text;

namespace watermark_utility
{
    public class Watermarker
    {
        public static int Main(string[] args)
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
                ),
                new Option<String>(
                    "--image",
                    description: "Should an image be added as a watermark? Accepted values: 'true' or 'false'",
                    getDefaultValue: () => "true"
                ),
                new Option<String>(
                    "--outputPassword",
                    description: "The password that should be applied to the document as the Owner Password",
                    getDefaultValue: () => null
                )
            };

            rootCommand.Description = "A simple utility that adds a watermark to a PDF and writes a new copy.";

            rootCommand.Handler =
                CommandHandler.Create<String, String, String, String, Boolean, String>((input, output, pages, text, image, outputPassword) =>
            {
                Console.WriteLine($"The value for --input is : {input}");
                Console.WriteLine($"The value for --output is : {output}");
                Console.WriteLine($"The value for --pages is : {pages}");
                Console.WriteLine($"The value for --text is : {text}");
                Console.WriteLine($"The value for --image is : {image}");

                if (outputPassword is null)
                    Console.WriteLine($"The value for --outputPassword is : null");
                else
                    Console.WriteLine($"The value for --outputPassword is not null");

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

                // Create the PdfWriter that is going to be used
                PdfWriter writer;
                if (outputPassword is not null)
                {
                    // if a password was provider, use it as the Owner Password
                    WriterProperties writerProperties = new WriterProperties();
                    writerProperties.SetStandardEncryption(
                        null, // no user password
                        Encoding.UTF8.GetBytes(outputPassword), // set owner password
                        EncryptionConstants.ALLOW_PRINTING | EncryptionConstants.ALLOW_SCREENREADERS,
                        // not all PDF utilities can deal with encryption that is stronger than STANDARD_ENCRYPTION_40 so
                        // change this at your own risk!
                        EncryptionConstants.STANDARD_ENCRYPTION_40 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA
                    );
                    writer = new PdfWriter(output, writerProperties);
                }
                else
                    writer = new PdfWriter(output);

                PdfDocument document = new PdfDocument(new PdfReader(input), writer);

                addWatermarkToPdf(document, pages, text, image);

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
        protected static void addWatermarkToPdf(PdfDocument document, String pages, String watermarkTextContent, Boolean addImage)
        {
            Paragraph watermarkText = new Paragraph(watermarkTextContent);
            watermarkText.SetFont(PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA)));
            watermarkText.SetFontSize(20);
            watermarkText.SetFontColor(new DeviceRgb(10, 129, 195));
            watermarkText.SetOpacity(0.5f);

            ResourceManager resourceManager = WatermarkResources.ResourceManager;
            byte[] imageData = (byte[])resourceManager.GetObject("AbleDocs_logo");
            ImageData watermarkImage = ImageDataFactory.Create(imageData);

            if (pages == "all")
                for (int i = 1; i < document.GetNumberOfPages() + 1; i++)
                {
                    addWatermarkToPage(document, i, watermarkText);

                    if (addImage == true)
                    {
                        addImageWatermarkToPage(document, i, watermarkImage);
                    }
                }
            else
                Console.WriteLine("Did not watermark any pages!");
        }

        /// <summary>
        /// Adds the provided text to the specified page in the PdfDocument in the center of the page
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pageNumber"></param>
        /// <param name="text"></param>
        protected static void addWatermarkToPage(PdfDocument document, int pageNumber, Paragraph text)
        {
            PdfPage page = document.GetPage(pageNumber);
            Rectangle pageSize = page.GetPageSizeWithRotation();
            Canvas pageCanvas = new Canvas(page, page.GetPageSize()).
                ShowTextAligned(
                    text,
                    (pageSize.GetRight() - pageSize.GetLeft()) / 2, // Center the watermark on the horizontal axis of the page
                    (pageSize.GetTop() - pageSize.GetBottom()) / 2, // Center the watermark on the vertical axis of the page
                    pageNumber,
                    TextAlignment.CENTER,
                    VerticalAlignment.TOP,
                    0
                );

            pageCanvas.Close();
        }

        /// <summary>
        /// Adds the provided ImageData to the specified page in the PdfDocument in the center of the page as
        /// a watermark.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pageNumber"></param>
        /// <param name="watermarkImage"></param>
        protected static void addImageWatermarkToPage(PdfDocument document, int pageNumber, ImageData watermarkImage)
        {
            PdfPage page = document.GetPage(pageNumber);
            Rectangle pageSize = page.GetPageSizeWithRotation();
            PdfCanvas canvas = new PdfCanvas(page);

            // Create a new graphics state with an opacity of 50%
            PdfExtGState graphicsState = new PdfExtGState().SetFillOpacity(0.5f);
            canvas.SaveState();
            canvas.SetExtGState(graphicsState);

            // Add the image to the canvas
            canvas.AddImageWithTransformationMatrix(
                watermarkImage,
                watermarkImage.GetWidth(), // use the whole width of the image
                0,
                0,
                watermarkImage.GetHeight(), // use the whole height of the image
                ((pageSize.GetRight() - pageSize.GetLeft()) / 2) - (watermarkImage.GetWidth() / 2), // Center the watermark on the horizontal axis of the page
                (pageSize.GetTop() - pageSize.GetBottom()) / 2, // Center the watermark on the vertical axis of the page
                false
            );

            // Restore the initial graphics state
            canvas.RestoreState();
        }
    }
}
