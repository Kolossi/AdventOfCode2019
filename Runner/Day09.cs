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
            var intcode = new Intcode();
            long[] data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            intcode.InputQueue.Enqueue(1);
            intcode.Execute(data);
            return string.Join(",",intcode.OutputQueue);
        }

        public override string Second(string input)
        {
            long[] data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            var intcode = new Intcode(2, data);
            intcode.Resume();
            return string.Join(",",intcode.OutputQueue);
        }

        public override string FirstTest(string input)
        {
            Day.LogEnabled = false;
            var intcode = new Intcode();
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
