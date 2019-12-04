using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day04 : Day
    {
        public override string First(string input)
        {
            return CountValidPasswords(input, IsValid).ToString();
        }

        public override string Second(string input)
        {
            return CountValidPasswords(input, IsValidV2).ToString();
        }

        public override string FirstTest(string input)
        {
            return IsValid(input) ? "1" : "0";
        }

        public override string SecondTest(string input)
        {
            return IsValidV2(input) ? "1" : "0";
        }

        ////////////////////////////////////////////////////////

        private bool IsValid(string input)
        {
            int prev = 0;
            bool repeat = false;
            foreach (var curr in input.ToArray().Select(c => int.Parse(c.ToString())))
            {
                if (curr == prev) repeat = true;
                else if (curr < prev) return false;
                prev = curr;
            }
            return repeat == true;
        }

        private bool IsValidV2(string input)
        {
            int prev = 0;
            bool validRepeat = false;
            int repeatRunLength = 1;
            foreach (var curr in input.ToArray().Select(c => int.Parse(c.ToString())))
            {
                if (curr == prev)
                {
                    repeatRunLength++;
                }
                else if (curr < prev) return false;
                else
                {
                    if (repeatRunLength == 2) validRepeat = true;
                    repeatRunLength = 1;

                }
                prev = curr;
            }
            return validRepeat == true || repeatRunLength == 2;
        }

        private int CountValidPasswords(string input, Func<string, bool> ValidFunc)
        {
            var parts = input.GetParts("-");
            var start = int.Parse(parts[0]);
            var end = int.Parse(parts[1]);
            int count = 0;
            for (int pass = start; pass <= end; pass++)
            {
                if (ValidFunc(pass.ToString()))
                {
                    count++;
                }
            }
            return count;
        }

    }
}
