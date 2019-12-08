using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day08 :  Day
    {
        const int LAYERLENGTH = 150;
        const int ROWLENGTH = 25;

        public override string First(string input)
        {
            var layers = SplitString(input, LAYERLENGTH);
            var most0Layer = layers.OrderBy(l => l.Count(c => c == '0')).First();
            return (most0Layer.Count(c => c == '1') * most0Layer.Count(c => c == '2')).ToString();
        }

        public override string Second(string input)
        {
            var layers = SplitString(input, LAYERLENGTH).ToArray();
            string image = ResolveImage(layers);
            var rows = SplitString(image, ROWLENGTH);
            return Environment.NewLine + string.Join(Environment.NewLine, rows);
        }

        ////////////////////////////////////////////////////////

        private static IEnumerable<string> SplitString(string input, int splitLength)
        {
            return Enumerable.Range(0, input.Length / splitLength)
                .Select(i => input.Substring(i * splitLength, splitLength));
        }

        private static string ResolveImage(string[] layers)
        {
            return string.Join(
                "",
                Enumerable.Range(0, LAYERLENGTH)
                    .Select(i => layers.Select(l => l[i]).First(c => c != '2'))
                    .Select(c => c == '1' ? "#" : " "));
        }
    }
}
