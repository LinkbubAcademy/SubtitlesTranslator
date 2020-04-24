using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace SubtitlesTranslator.App.WPF.Lib.Models
{
    public class SubtitleLine
    {
        #region statics

        public static List<SubtitleLine> GetFromTextLines(List<string> lines)
        {
            var output = new List<SubtitleLine>();

            for (var i = 2; i < lines.Count - 3; i++)
            {
                if (int.TryParse(lines[i], out int lineNumber))
                {
                    var subtitleLine = new SubtitleLine()
                    {
                        LineNumber = lineNumber,
                        Period = lines[i + 1],
                        Text = lines[i + 2]
                    };

                    output.Add(subtitleLine);
                    i += 3;
                }
                else
                {
                    throw new ArgumentException($"los subtítulos no están bien, la línea:{i} [{lines[i]}] tenía que ser un número entero");
                }

            }

            return output;
        }

        #endregion

        public int LineNumber { get; set; }

        public string Period { get; set; }

        public string Text { get; set; }
    }
}
