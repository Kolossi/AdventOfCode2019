using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day05:  Day
    {
        public override string First(string input)
        {
            Day.LogEnabled = false;
            var intcode = new Intcode();
            intcode.InputQueue.Enqueue(1);
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            intcode.Execute(data);
            return intcode.OutputQueue.Last().ToString();
        }

        public override string Second(string input)
        {
            Day.ShowInputForOKTest = false;
            Day.LogEnabled = false;
            return SecondTest("5;" + input);
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        public override string SecondTest(string input)
        {
            Day.ShowInputForOKTest = false;
            Day.LogEnabled = false;
            var parts = input.Split(";");
            var inputData = int.Parse(parts[0]);
            input = parts[1];
            var intcode = new Intcode();
            intcode.InputQueue.Enqueue(inputData);
            int[] data = input.GetParts(",").Select(i => int.Parse(i)).ToArray();
            intcode.Execute(data);
            return intcode.OutputQueue.Last().ToString();
        }

        ////////////////////////////////////////////////////////
    }
}
