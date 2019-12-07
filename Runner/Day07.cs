using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day07:  Day
    {
        public override string First(string input)
        {
            var program = input.GetParts(",").Select(p => int.Parse(p)).ToArray();
            return AmpFindHighest(program).ToString();
        }

        public override string Second(string input)
        {
            var program = input.GetParts(",").Select(p => int.Parse(p)).ToArray();
            return AmpFindHighest(program, phaseOffset: 5, feedback: true).ToString();
        }

        public override string FirstTest(string input)
        {
            Day.LogEnabled = false;
            return PerformTest(input);
        }

        public override string SecondTest(string input)
        {
            Day.LogEnabled = false;
            return PerformTest(input, phaseOffset: 5, feedback: true);
        }

        ////////////////////////////////////////////////////////

        private string PerformTest(string input, int phaseOffset=0, bool feedback=false)
        {
            var parts = input.Split(":");
            input = parts[1];
            var program = input.GetParts(",").Select(p => int.Parse(p)).ToArray();
            if (parts[0] == "AUTO")
            {
                return AmpFindHighest(program, phaseOffset, feedback).ToString();
            }
            else
            {
                var phases = parts[0].Select(c => int.Parse(c.ToString())).ToArray();
                return AmpRun(phases, program, feedback).ToString();
            }
        }

        private object AmpFindHighest(int[] program, int phaseOffset=0, bool feedback=false)
        {
            List<int[]> phasesCombos = GetPhasesCombos(phaseOffset);

            var highest = 0;
            foreach (var phases in phasesCombos)
            {
                var result = AmpRun(phases, program, feedback);
                if (result > highest) highest = result;
            }
            return highest;
        }

        private static List<int[]> GetPhasesCombos(int offset = 0)
        {
            var phasesCombos = new List<int[]>();
            foreach (var a in Enumerable.Range(offset, 5))
            {
                foreach (var b in Enumerable.Range(offset, 5).Except(new int[] { a }))
                {
                    foreach (var c in Enumerable.Range(offset, 5).Except(new int[] { a, b }))
                    {
                        foreach (var d in Enumerable.Range(offset, 5).Except(new int[] { a, b, c }))
                        {
                            foreach (var e in Enumerable.Range(offset, 5).Except(new int[] { a, b, c, d }))
                            {
                                phasesCombos.Add(new int[] { a, b, c, d, e });
                            }
                        }
                    }

                }
            }

            return phasesCombos;
        }

        public int AmpRun(int[] phases, int[] program, bool feedback=false)
        {
            var intcodes = phases.Select(p => new Intcode(p, program)).ToArray();
            int lastOutput = 0;
            do
            {
                for (int i = 0; i < phases.Length; i++)
                {
                    Intcode intcode = intcodes[i];
                    intcode.InputQueue.Enqueue(lastOutput);
                    intcode.Resume();
                    lastOutput = intcode.OutputQueue.Dequeue();
                }
            } while (feedback && !intcodes[phases.Length - 1].Halt);
            return lastOutput;
        }
    }
}
