using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day25 :  Day
    {
        public override string First(string input)
        {
            var data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var intcode = new Intcode(data);
            var startInstructions = new string[]
            {
                "north",
                "take mutex",
                "east","east","east",
                "take whirled peas",
                "west","west","west","south","west",
                "take space law space brochure",
                "north", "take loom",
                "south","south","take hologram",
                "west","take manifold",
                "east","north","east",
                "south","take cake",
                "west","south","take easter egg",
                "south",
                "inv"

            };
            intcode.RunAsciiComputer(startInstructions);
            var output = intcode.GetOutputQueueAsAsciiStrings();
            var items = Enumerable.Range(output.Length - 1 - 8, 8).Select(i => output[i].Substring(2)).OrderBy(s => s).ToArray();
            //intcode.RunAsciiComputer();
            var previousState = 511;
            for (int newState = 511; newState > 0; newState--)
            {
                var instructions = GetStateChangeInstructions(items, newState, previousState);
                instructions.Add("inv");
                instructions.Add("south");
                foreach (var instruction in instructions)
                {
                    intcode.RunAsciiComputer(new string[] { instruction });
                    output = intcode.GetOutputQueueAsAsciiStrings();
                    //foreach (var line in output) Console.WriteLine(line);
                }
                if (!output[4].Contains("lighter") && !output[4].Contains("heavier")) break;
                previousState = newState;
            }
            foreach (var line in output) Console.WriteLine(line);
            return "X";
        }

        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
        }

        public override string FirstTest(string input)
        {
            // remove "SKIP" in Day25FirstTests.txt and play "by hand"
            var data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var intcode = new Intcode(data);
            intcode.RunAsciiComputer();
            return "X";
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        private List<string> GetStateChangeInstructions(string[] items, int newState, int previousState)
        {
            List<string> instructions = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                var val = (int)Math.Pow(2,i);
                bool newHasItem = (newState & val) == val;
                bool previousHasItem = (previousState & val) == val;
                bool mustChange = newHasItem ^ previousHasItem;
                if (mustChange)
                {
                    instructions.Add(string.Format("{0} {1}", newHasItem ? "take" : "drop", items[i]));
                }
            }
            return instructions;
        }
    }
}
