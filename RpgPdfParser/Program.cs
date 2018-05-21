using System;
using System.Collections.Generic;
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
    public struct StringWithFont
    {
        public string String;
        public float Width;
    }

    internal static class Program
    {
        private static TextToMonsterDataParser m_parser;
        public static List<StringWithFont> TextWithWidth = new List<StringWithFont>();

        public static void Main(string[] args)
        {
            Contract.Assert(args.Any(), "No arguments passed.");

            var path = args.First();
            var extractedText = string.Empty;

            using (var pdfReader = new PdfReader(path))
            {
                var pdfDocument = new PdfDocument(pdfReader);
                var page = pdfDocument.GetPage(208);

                var fontFilter = new CustomFilter(page.GetArtBox());
                var listener = new FilteredEventListener();

                var extractionStrategy = listener.AttachEventListener(new SimpleTextExtractionStrategy(), fontFilter);
                new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(208));

                extractedText = extractionStrategy.GetResultantText();
            }

            m_parser = new TextToMonsterDataParser(extractedText);
            var monsters = m_parser.GetMonsterData();

            Console.WriteLine("END");

            Console.WriteLine(extractedText);
            Console.ReadLine();
            
            foreach (var text in TextWithWidth)
            {
                Console.WriteLine("{0}({1})", text.String, text.Width);
            }

            
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

            var stringWithFont = new StringWithFont()
            {
                String = text,
                Width = renderInfo.GetSingleSpaceWidth()
            };

            if (!Program.TextWithWidth.Any())
            {
                Program.TextWithWidth.Add(stringWithFont);
                return true;
            }
                
            var lastEntry = Program.TextWithWidth.Last();

            if (Math.Abs(lastEntry.Width - stringWithFont.Width) < 0.001f)
            {
                Program.TextWithWidth.Remove(lastEntry);
                lastEntry.String += stringWithFont.String;
                Program.TextWithWidth.Add(lastEntry);
            }
            else
            {
                Program.TextWithWidth.Add(stringWithFont);
            }

            return true;
        }
    }
}