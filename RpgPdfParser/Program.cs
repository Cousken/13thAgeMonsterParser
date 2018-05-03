using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout.Element;

namespace RpgPdfParser
{
    internal static class Program
    {
        private static TextToMonsterDataParser m_parser;
        
        public static void Main(string[] args)
        {
            Contract.Assert(args.Any() , "No arguments passed.");

            var path = args.First();
            var extractedText = string.Empty;
            
            using (var pdfReader = new PdfReader(path))
            {                   
                var pdfDocument = new PdfDocument(pdfReader);
                var rect = new Rectangle(36, 750, 523, 56);
                
                var fontFilter = new CustomFilter(rect);
                var listener = new FilteredEventListener();
                
                var extractionStrategy = listener.AttachEventListener(new LocationTextExtractionStrategy(), fontFilter);
                new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(208));
 
                extractedText = extractionStrategy.GetResultantText();
            }
            
            m_parser = new TextToMonsterDataParser(extractedText);
            var monsters = m_parser.GetMonsterData();

            Console.WriteLine("END");
            Console.ReadLine();

            Console.WriteLine(extractedText);

            Console.ReadLine();
        }
    }

    public class CustomFilter : TextRegionEventFilter
    {
        public CustomFilter(Rectangle filterRect) : base(filterRect)
        {
            
        }

        public override bool Accept(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT)) 
                return false;
            
            var renderInfo = (TextRenderInfo) data;
            string text = renderInfo.GetText();
            if (text == null)
                return false;
            
            if(text.Contains("Ant"))
                Debugger.Break();
            
            if(text.Contains("ather-be-foraging-than-fighting"))
                Debugger.Break();
            
            var font = renderInfo.GetFont();
            var fontSize = renderInfo.GetFontSize();
            
            if (null == font) 
                return false;
            
            
            var fontName = font.GetFontProgram().GetFontNames().GetFontName();
            return fontName.EndsWith("Bold") || fontName.EndsWith("Oblique");
        }
    }
    
}