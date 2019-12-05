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
            // not 337106 too low - read the question, set noun & verb first!
            Day.LogEnabled = false;
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            var intcode = new Intcode() { Noun = 12, Verb = 2 };
            return intcode.Execute(data)[0].ToString();
        }

        public override string Second(string input)
        {
            Day.LogEnabled = false;
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            var intcode = new Intcode();
            return Solve(data, 19690720);
        }

        public override string FirstTest(string input)
        {
            Day.LogEnabled = false;
            Day.ShowInputForOKTest = false;
            var parts = input.Split(";");
            var testtype = parts[0];
            input = parts[1];
            if (testtype=="F")
            {
                return First(input);
            }
            else
            {
                var intcode = new Intcode();
                int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
                return intcode.Execute(data)[0].ToString();
            }
        }

//        public override string SecondTest(string input)
//        {
//            throw new NotImplementedException("SecondTest");
//        }

        ////////////////////////////////////////////////////////

        public string Solve(int[] data, int target)
        {
            var intcode = new Intcode();

            for (int verb = 14; verb < 100; verb++)
            {
                intcode.Verb = verb;
                for (int noun = 70; noun < 100; noun++)
                {
                    intcode.Noun = noun;
                    if (intcode.Execute(Intcode.CloneData(data))[0] == target)
                        return string.Format("{0}{1}", noun, verb);
                }
            }
            throw new InvalidOperationException("Huh?");
        }
    }
}
