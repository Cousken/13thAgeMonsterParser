using System;
using System.Diagnostics.Contracts;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace RpgPdfParser
{
    internal static class Program
    {
        private static TextToMonterDataParser m_parser;
        
        public static void Main(string[] args)
        {
            Contract.Assert(args.Any() , "No arguments passed.");

            var path = args.First();
            var extractedText = string.Empty;
            
            using (var pdfReader = new PdfReader(path))
            {
                extractedText = PdfTextExtractor.GetTextFromPage(pdfReader, 208, new SimpleTextExtractionStrategy());
            }
            
            m_parser = new TextToMonterDataParser(extractedText);
            var monsters = m_parser.GetMonsterData();

            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
    
    
}