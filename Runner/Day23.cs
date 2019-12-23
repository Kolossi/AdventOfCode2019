using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day23 : Day
    {
        public override string First(string input)
        {
            return RunNetwork(input);
        }

        // GOT RANK 993 on Day23 part 2 leaderboard!
        public override string Second(string input)
        {
            return RunNetwork(input, useNAT:true);
        }

        public override string FirstTest(string input)
        {
            throw new NotImplementedException("FirstTest");
        }

        public override string SecondTest(string input)
        {
            throw new NotImplementedException("SecondTest");
        }

        ////////////////////////////////////////////////////////
        
        private string RunNetwork(string input, bool useNAT = false)
        {
            long natX = -1, natY = -1, lastNatY = -1;
            var data = input.GetParts(",").Select(i => long.Parse(i)).ToArray();
            Intcode[] intcodes = new Intcode[50];
            for (int i = 0; i < 50; i++)
            {
                intcodes[i] = new Intcode(i, data);
            }
            while (true)
            {
                for (int i = 0; i < 50; i++)
                {
                    var intcode = intcodes[i];
                    if (intcode.AwaitingInput && !intcode.InputQueue.Any()) intcode.InputQueue.Enqueue(-1);
                    intcode.Resume();
                    while (intcode.OutputQueue.Count() >= 3)
                    {
                        var address = intcode.OutputQueue.Dequeue();
                        var x = intcode.OutputQueue.Dequeue();
                        var y = intcode.OutputQueue.Dequeue();
                        if (address == 255)
                        {
                            if (useNAT)
                            {
                                natX = x;
                                natY = y;
                            }
                            else
                            {
                                return y.ToString();
                            }
                        }
                        else
                        {
                            intcodes[address].InputQueue.Enqueue(x);
                            intcodes[address].InputQueue.Enqueue(y);
                        }
                    }
                }
                if (useNAT)
                {
                    if (intcodes.All(ic => ic.AwaitingInput) && intcodes.All(ic => !ic.InputQueue.Any()) && natY!=-1)
                    {
                        if (natY == lastNatY) return natY.ToString();
                        intcodes[0].InputQueue.Enqueue(natX);
                        intcodes[0].InputQueue.Enqueue(natY);
                        lastNatY = natY;
                    }
                }
            }
        }
    }
}
