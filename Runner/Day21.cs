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
            for (int j = 0; j < 512; j++)
            {
                var b = Convert.ToString(j, 2).PadLeft(9,'0');

                bool? jump = null;
                //   abcdefghi
                //   012345678
                //  @   @   @
                //
                if (b[3] == '0') continue;   //  screwed if d=., shouldn't have got here
                if (b[0] == '0' && b[4]=='0' && b[7] == '0') continue; // screwed if forced to jump to d, but forced to jump again and next jump to h but h=.
                if (b[0] == '0') jump = true;   //jump if a=.
                if (b[3] == '1' && b[4] == '0' && b[5] == '0' && b[6] == '0') jump = true; //jump if d=# & efg=...
                if (b[1] == '0' && b[3] == '1' && b[4] == '0') jump = true;
                // #..#.#######
                if (b[4] == '0' & b[7] == '0') jump = false; // no jump if e=. & h=.
                // #..##...#

                var zRunLength = 0;
                char lastChar = 'Z';
                bool skipThis = false;
                for (int d = 0; d < b.Length; d++)
                {
                    char c = b[d];
                    if (c == '0' && lastChar=='0')
                    {
                        if (++zRunLength >= 4) skipThis = true;

                    }
                    else
                    {
                        lastChar = c;
                        if (c == '0') zRunLength = 1;
                    }
                }
                if (skipThis) continue;
                LogLine("{0} {1}",b, jump.HasValue?(jump.Value?"J":"N"):"?");
            }
            LogLine("---------");
            var program = new string[]
            {
                //////   OR  A J          - J = A==1 ? 1 : J (J==1 ? 1 : A)  jump if A=#
                //////   AND A J          - J = A==0 ? 0 : J (J==1 ? A : 0)  no jump if A=.
                //////   NOT A J; AND A J - J = 0
                //////   NOT A J; OR  A J - J = 1
                //////   NOT A T; NOT T T - T = A
                //////   NOT A T; AND T J - J = A==1 ? 0 : J                 no jump if A=#
                //////   NOT A T; OR  T J - J = A==0 ? 1 : J                 jump if A=.
                //////   NOT A T; NOT T T; OR B T; AND T J                   no jump if A=. and B=.
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
                "NOT A T\nOR T J",                     //  must jump if  A==.
                //"NOT E T\nOR T J",                   //  must jump if  E==.
                //"AND D J",                             //  no jump if D==.
                "NOT E T\nNOT T T\nOR H T\nAND T J",   //  no jump if (e=. and h=.)
                                                       // must jump if b=. d=#, e=.
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
