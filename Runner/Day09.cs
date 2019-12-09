using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day09 :  Day
    {
        public override string First(string input)
        {
            var intcode = new IntcodeV2();
            long[] data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            intcode.InputQueue.Enqueue(1);
            intcode.Execute(data);
            return string.Join(",",intcode.OutputQueue);
        }

        public override string Second(string input)
        {
            var intcode = new IntcodeV2();
            long[] data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            intcode.InputQueue.Enqueue(2);
            intcode.Execute(data);
            return string.Join(",",intcode.OutputQueue);
        }

        public override string FirstTest(string input)
        {
            Day.LogEnabled = false;
            var intcode = new IntcodeV2();
            long[] data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            intcode.Execute(data);
            return string.Join(",",intcode.OutputQueue);
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////
    }
}
