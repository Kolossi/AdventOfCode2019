using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day21 :  Day
    {
        public override string First(string input)
        {
            var program = new string[]
            {
                "OR D J",           //   jump if D==#
                "NOT C T\nAND T J", //   no jump if C==#
                "NOT A T\nOR T J",  //   jump if A==.
                "WALK",
                ""
            };
            var springDroid = new SpringDroid(input);
            var output = springDroid.Execute(program);
            return output;
        }

        public override string Second(string input)
        {
            var program = new string[]
            {
                "NOT B T\nOR T J",          // jump if B=. & d(landing)=#
                "NOT C T\nAND H T\nOR T J", // jump if C=. and d(landing)=# and h(next landing)=#
                "AND D J",                  // dont jump if d(landing)=.
                "NOT A T\nOR T J",          // must jump if  A==.
                "RUN",
                ""

            };
            var springDroid = new SpringDroid(input);
            var output = springDroid.Execute(program);
            return output;
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
        ///
        public class SpringDroid
        {
            public long[] Data;

            public SpringDroid(string stringData)
            {
                Data = stringData.GetParts(",").Select(p => long.Parse(p)).ToArray();
            }

            public string Execute(string[] springCodeString)
            {
                long[] springCode = string.Join('\n', springCodeString).Select(s => (long)s).ToArray();
                var intcode = new Intcode(Data);
                foreach (var input in springCode) intcode.InputQueue.Enqueue(input);
                intcode.Resume();
                var outputs = new List<long>();
                while (intcode.OutputQueue.Any())
                {
                    outputs.Add(intcode.OutputQueue.Dequeue());
                }
                if (outputs.Last() > 256) return outputs.Last().ToString();
                else return string.Join("", outputs.Select(o => (char)o));
            }
        }
    }
}
