using System;
using System.Diagnostics.Contracts;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace RpgPdfParser
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Contract.Assert(args.Length > 0 , "No arguments passed.");

            var path = args.First();
            var extractedText = string.Empty;
            
            using (var pdfReader = new PdfReader(path))
            {
                extractedText = PdfTextExtractor.GetTextFromPage(pdfReader, 208, new SimpleTextExtractionStrategy());
            }

            Console.WriteLine(extractedText);
                
            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
    
    
}