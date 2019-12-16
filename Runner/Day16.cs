using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day16 :  Day
    {
        public override string First(string input)
        {
            var parts = input.GetParts(",");
            var phaseCount = int.Parse(parts[0]);
            var fftInput = parts[1].ToCharArray().Select(c => long.Parse(c.ToString()));
            var result = OperateFftPhases(fftInput, phaseCount);
            return result;
        }

        public override string Second(string input)
        {
            //spot zeros and skip inputs completely
            // spot repeats and just sum x elements
            // etc to optimise
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////

        private string OperateFftPhases(IEnumerable<long> fftInput, int phaseCount)
        {
            var fftBasePattern = new LinkedList<long>(new long[] { 0, 1, 0, -1 });
            Queue<long> inputQueue = null;

            for (int phase = 0; phase < phaseCount; phase++)
            {
                var output = new List<long>();
                for (int outputDigit = 1; outputDigit <= fftInput.Count(); outputDigit++)
                {
                    var fftPattern = new LinkedList<long>();
                    foreach (var patternDigit in fftBasePattern)
                    {
                        for (int i = 0; i < outputDigit; i++)
                        {
                            fftPattern.AddLast(patternDigit);
                        }
                    }
                    var factor = fftPattern.First.Next;
                    inputQueue = new Queue<long>(fftInput);
                    long outputDigitValue = 0;
                    for (int inputDigit = 0; inputDigit < fftInput.Count(); inputDigit++)
                    {
                        outputDigitValue += (factor.Value * inputQueue.Dequeue());
                        factor = factor == fftPattern.Last ? fftPattern.First : factor.Next;
                    }
                    output.Add(Math.Abs(outputDigitValue) % 10);
                }
                fftInput = output;
            }
            return string.Join("", fftInput.Take(8));
        }

    }
}
