using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;

namespace RpgPdfParser
{
    public class TextToMonterDataParser
    {
        private readonly string m_originalText;
        private static readonly int Cores = Environment.ProcessorCount;

        public TextToMonterDataParser(string text)
        {
            m_originalText = text;
        }
        
        public IEnumerable<IMonsterData> GetMonsterData()
        {
            var lines = SplitIntoLines(m_originalText);
            var linesSegment = lines.Length / Cores;
            var result = new List<IMonsterData>();

            for (int i = 0; i < Cores; i++)
            {
                result.Add(ParseLinesIntoMonsterData(
                        i * linesSegment, (i + 1) * linesSegment,
                        new ReadOnlyCollection<string>(lines))
                );
            }

            return result;
        }

        private IMonsterData ParseLinesIntoMonsterData(int startingLine, int lastLine, ReadOnlyCollection<string> lines)
        {
            var linesSubset = lines.ToList().GetRange(startingLine, lastLine - startingLine);
            var levelNumberEnding = new List<string>() {"st", "nd", "rd", "th"};
            var indexii = GetIndexiiOfSubstrings(levelNumberEnding, linesSubset);

            foreach (var index in indexii)
            {
                var indexOf = startingLine + index;
                Console.WriteLine($"There is a monster on line {indexOf}");
                return null;
            }

            return null;
        }

        private static IEnumerable<int> GetIndexiiOfSubstrings(IList<string> substring, IList<string> lines)
        {
            var linesCount = lines.Count();
            for (var i = 0; i < linesCount; i++)
            {
                if (substring.Any(s => s.Equals(lines[i])))
                {
                    yield return i;
                }
            }
        }

        private static string[] SplitIntoLines(string text)
        {
            return text.Split(new[]{'\n'}, StringSplitOptions.None);
        }
    }
}