using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day02 :  Day
    {
        public override string First(string input)
        {
            // not 337106 too low - read the question, reset data first!
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            int noun = 12;
            int verb = 2;
            return Intcode.Rerun(data, noun, verb)[0].ToString();
        }

        public override string Second(string input)
        {
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            return Solve(data, 19690720);
        }

        public override string FirstTest(string input)
        {
            return Intcode.Execute(input.GetParts(",").Select(i => int.Parse(i)).ToArray())[0].ToString();
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        public string Solve(int[] data, int target)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                for (int noun = 0; noun < 100; noun++)
                {
                    if (Intcode.Rerun(data, noun, verb)[0] == target)
                        return string.Format("{0}{1}", noun, verb);
                }
            }
            throw new InvalidOperationException("Huh?");
        }
    }
}
