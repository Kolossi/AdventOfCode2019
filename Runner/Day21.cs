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
                //////   OR  A J          - J = A==1 ? 1 : J (J==1 ? 1 : A)  jump if A=#
                //////   AND A J          - J = A==0 ? 0 : J (J==1 ? A : 0)  no jump if A=.
                //////   NOT A J; AND A J - J = 0
                //////   NOT A J; OR  A J - J = 1
                //////   NOT A T; AND T J - J = A==1 ? 0 : J                 no jump if A=#
                //////   NOT A T; OR  T J - J = A==0 ? 1 : J                 jump if A=.
                //////
                //////  ABCD
                ////// @
                ////// ##?.#   jump
                ////// ##?##   no jump
                ////// #.???   jump (no alternative)
                //////
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
                //////   OR  A J          - J = A==1 ? 1 : J (J==1 ? 1 : A)  jump if A=#
                //////   AND A J          - J = A==0 ? 0 : J (J==1 ? A : 0)  no jump if A=.
                //////   NOT A J; AND A J - J = 0
                //////   NOT A J; OR  A J - J = 1
                //////   NOT A T; NOT T T - T = A
                //////   NOT A T; AND T J - J = A==1 ? 0 : J                 no jump if A=#
                //////   NOT A T; OR  T J - J = A==0 ? 1 : J                 jump if A=.
                //////
                //////  ABCDEFGHI
                ////// @
                ////// ##.#.##.#.   jump
                ////// ###.#.##.#   dont jump
                ////// ##.###..#.   jump
                ////// ##..#.####   jump
                ////// #.           must jump
                //////
                // jump if:
                // D=# && (NOT E==. && H==.) || (NOT F==. && I==.)
                // but not E==. and H==.
                //   if  E==. && H==. (would have to jump at D, but would land in hole at H)
                //"OR D J",  //   if  D==# // landing tile
                //"NOT C T", // \ 
                //"AND T J", // / and C==.
                //"NOT A T", // \ 
                //"OR T J",  // / or A==.
                //"NOT E T", // \
                //"OR H T",  //  > and not (e=. and h=.)
                //"AND T J", // /
                //"RUN",
                //""
                //------------------------
                //"OR D J",                            //  jump if  D==# // landing tile
                //"NOT C T\nAND T J",                  //  no jump if C==.
                //"NOT B T\nNOT T T\nOR E T\nAND T J", // no jump if (b=. and e=.)
                "NOT A T\nOR T J",                   //  must jump if  A==.
                //"NOT E T\nOR T J",                   //  must jump if  E==.
                "NOT D T\nAND T J",                  //  no jump if D==.
                "NOT E T\nNOT T T\nOR H T\nAND T J", //  no jump if (e=. and h=.)
                "RUN",
                ""
                 //---
                //---
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
