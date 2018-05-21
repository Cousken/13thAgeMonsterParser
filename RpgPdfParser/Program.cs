using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace RpgPdfParser
{
    public struct StringWithFont
    {
        public string String;
        public float Width;
    }

    internal static class Program
    {
        public static LinkedList<StringWithFont> TextWithWidth = new LinkedList<StringWithFont>();

        public static void Main(string[] args)
        {
            Contract.Assert(args.Any(), "No arguments passed.");

            var path = args.First();
            var extractedText = string.Empty;

            // monsters are on page 208 - 253

            using (var pdfReader = new PdfReader(path))
            {
                var stringBuilder = new StringBuilder();
                var pdfDocument = new PdfDocument(pdfReader);
                var listener = new FilteredEventListener();
                CustomFilter customFilter = null;
                SimpleTextExtractionStrategy extractionStrategy = null;
                PdfCanvasProcessor pdfProcessor = null;

                for (int i = 208; i <= 253; i++)
                {
                    Console.WriteLine(@"Reading page {0}.", i);
                    var page = pdfDocument.GetPage(i);

                    if (pdfProcessor == null)
                    {
                        customFilter = new CustomFilter(page.GetArtBox());
                        extractionStrategy = listener.AttachEventListener(new SimpleTextExtractionStrategy(), customFilter);
                        pdfProcessor = new PdfCanvasProcessor(listener);
                    }

                    pdfProcessor.ProcessPageContent(page);
                    stringBuilder.AppendLine(extractionStrategy.GetResultantText());

                    if (Program.TextWithWidth.Last.Value.Equals(Program.TextWithWidth.Last.Previous.Value))
                        Debugger.Break();
                }

                extractedText = stringBuilder.ToString();
            }

            //m_parser = new TextToMonsterDataParser(extractedText);
            //var monsters = m_parser.GetMonsterData();


            Console.WriteLine("Read all text.");
            Console.ReadLine();

            if(System.IO.File.Exists(@"AllLines.txt"))
                System.IO.File.Delete(@"AllLines.txt");

            if (System.IO.File.Exists(@"EntireText.txt"))
                System.IO.File.Delete(@"EntireText.txt");

            Console.WriteLine("Printing text.");
            System.IO.File.WriteAllText(@"EntireText.txt", extractedText);

            Console.WriteLine("Found {0} duplicates.", TextWithWidth.Count(n => n.String.Equals("die’")));

            Console.WriteLine("Printing lines.");

            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"AllLines.txt", false))
            {
                var node = TextWithWidth.First;
                while(node != null)
                {
                    file.WriteLine("{0}({1})", node.Value.String, node.Value.Width);
                    node = node.Next;
                }
            }

            Console.ReadLine();
        }
    }

    public class CustomFilter : TextRegionEventFilter
    {
        StringWithFont? previousEntry;

        public CustomFilter(Rectangle filterRect) : base(filterRect)
        {
        }

        public override bool Accept(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT))
                return false;

            var renderInfo = (TextRenderInfo)data;
            string text = renderInfo.GetText();
            if (text == null)
                return false;

            if (text.Equals("die’"))
                Debugger.Break();

            var stringWithFont = new StringWithFont()
            {
                String = text,
                Width = renderInfo.GetSingleSpaceWidth()
            };

            if (previousEntry == null)
            {
                Program.TextWithWidth.AddLast(stringWithFont);
                previousEntry = stringWithFont;
                return true;
            }

            var previousWidth = previousEntry.Value.Width;
            var stringWithFontWidth = stringWithFont.Width;
            var delta = Math.Abs(previousWidth - stringWithFontWidth);

            if ((!previousEntry.Value.String.Equals(stringWithFont.String)
                && previousEntry.Value.String.EndsWith(stringWithFont.String))
                || previousEntry.Value.String.Equals(stringWithFont.String))
            {
                Program.TextWithWidth.RemoveLast();
                previousEntry = new StringWithFont()
                {
                    String = previousEntry.Value.String,
                    Width = Math.Max(stringWithFont.Width, previousWidth)
                };
                Program.TextWithWidth.AddLast(previousEntry.Value);
            }
            else if (delta < 0.001f
                && !previousEntry.Value.String.Equals(stringWithFont.String))
            {
                DebugCheck(stringWithFont, previousEntry.Value);

                Program.TextWithWidth.RemoveLast();
                previousEntry = new StringWithFont()
                {
                    String = previousEntry.Value.String + stringWithFont.String,
                    Width = Math.Max(stringWithFont.Width, previousWidth)
                };

                Program.TextWithWidth.AddLast(previousEntry.Value);
            }
            else
            {
                DebugCheck(stringWithFont, previousEntry.Value);
                Program.TextWithWidth.AddLast(stringWithFont);
                previousEntry = stringWithFont;
            }

            return true;
        }

        private static void DebugCheck(StringWithFont current, StringWithFont previous)
        {
            if (previous.String.Equals(current.String))
                Debugger.Break();
        }
    }
}