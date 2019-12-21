using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Runner
{
    public abstract class Day
    {
        public abstract string First(string input);
        public abstract string Second(string input);

        public string Solve(string set, Func<string,string> solver)
        {
            string input = string.Empty;
            try
            {
                input = GetInput(set);
            }
            catch (FileNotFoundException)
            {
            }

            if (string.IsNullOrWhiteSpace(input)) return "INPUT MISSING";

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result =  solver(input);
                sw.Stop();
                Console.Write("(Took : {0}) ",sw.Elapsed);
                return result;
            }
            catch (NotImplementedException)
            {
                return "NOT IMPLEMENTED";
            }
        }

        public string SolveFirst()
        {
            return Solve("First", this.First);
        }

        public string SolveSecond()
        {
            return Solve("Second", this.Second);
        }

        public virtual string FirstTest(string input)
        {
            return this.First(input);
        }

        public virtual string SecondTest(string input)
        {
            return this.Second(input);
        }

        public bool TestFirst()
        {
            return Test("First", this.FirstTest);
        }

        public bool TestSecond()
        {
            return Test("Second", this.SecondTest);
        }

        public static bool ShowInputForOKTest = true;

        public bool Test(string set, Func<string, string> solver)
        {
            bool result = true;
            string input = string.Empty;

            Console.WriteLine(string.Format("    {0} Tests", set));
            try
            {
                input = GetInput(string.Format("{0}Tests", set));
            }
            catch (FileNotFoundException)
            {
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("      TEST INPUT MISSING");
                return false;
            }

            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (lines[lines.Length-1][0]==':')
            {
                var sb = new StringBuilder();
                var testLines = new List<string>();

                foreach (var testLine in lines)
                {
                    if (testLine[0]==':')
                    {
                        sb.Append(testLine);
                        testLines.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(testLine);
                    }
                }
                lines = testLines.ToArray();
            }
            if (lines[0]=="SKIP")
            {
                Console.WriteLine("      TEST INPUT STARTS WITH SKIP");
                return false;
            }
            foreach (var line in lines)
            {

                int colon = line.LastIndexOf(":");
                var parts = line.Split(":");
                var testInput = line.Substring(0,colon);
                var expectedOutput = line.Substring(colon+1,line.Length-colon-1);
                string output = string.Empty;
                try
                {
                    output = solver(testInput);
                    if (output != expectedOutput)
                    {
                        result = false;
                        Console.WriteLine(string.Format("    {0} : FAILED", line));
                        Console.WriteLine(string.Format("      Input :{3}{0}, Expected : {1}, Got : {2}", testInput,
                            expectedOutput, output, testInput.Contains(Environment.NewLine) ? Environment.NewLine : " "));
                    }
                    else
                    {
                        if (ShowInputForOKTest)
                        {
                            Console.Write(string.Format("{1}{0} : ",
                                line,
                                line.Contains(Environment.NewLine) ? Environment.NewLine : "    "));
                        }
                        Console.WriteLine("OK");
                    }
                }
                catch (NotImplementedException)
                {
                    Console.WriteLine(string.Format("    {0} : NOT IMPLEMENTED", line));
                    result = false;
                }
            }
            return result;
        }

        public string GetInput(string set)
        {
            string filename = string.Format("Inputs/{0}{1}.txt",this.GetType().Name, set);
            if (set == "Second" && !File.Exists(filename)) return GetInput("First");
            string input = System.IO.File.ReadAllText(filename);
            if (set == "Second" && input == "FIRST") return GetInput("First");
            return input;
        }

        public static string[] GetLines(string input, string removeChars = null)
        {
            return input.GetLines(removeChars);
        }

        public static string[] GetParts(string input, string removeChars = null)
        {
            return input.GetParts(removeChars);
        }

        public static object[] ConvertEnumerableArgsToCSVString(object[] args)
        {
            if (!args.Any(a => a is System.Collections.IEnumerable && !(a is string))) return args;
            var newArgs = new List<object>();
            foreach(var a in args)
            {
                object newArg = ConvertEnumerableArgToCSVString(a);
                newArgs.Add(newArg);
            }
            return newArgs.ToArray();
        }

        public static object ConvertEnumerableArgToCSVString(object a)
        {
            object newArg;
            if (a is System.Collections.IEnumerable && !(a is string))
            {
                var strValues = new List<string>();
                foreach (var o in (a as System.Collections.IEnumerable))
                {
                    strValues.Add(ConvertEnumerableArgToCSVString(o).ToString());
                }
                newArg = string.Format("[{0}]",string.Join(", ", strValues));
            }
            else
            {
                newArg = a;
            }

            return newArg;
        }

#if DEBUG
        public static bool LogEnabled = true;
#else
        public static bool LogEnabled = false;
#endif

        public static void LogLine(string format, params object[] args)
        {
#if DEBUG
            if (LogEnabled)
            {
                args = ConvertEnumerableArgsToCSVString(args);
                Console.WriteLine(string.Format(format, args));
            }
#endif
        }

        public static void LogLine(object arg)
        {
#if DEBUG
            if (LogEnabled) Console.WriteLine(ConvertEnumerableArgToCSVString(arg));
#endif
        }

        public static void LogLine()
        {
#if DEBUG
            if (LogEnabled) Console.WriteLine();
#endif
        }

        public static void Log(string format, params object[] args)
        {
#if DEBUG
            args = ConvertEnumerableArgsToCSVString(args);
            if (LogEnabled) Console.Write(string.Format(format, args));
#endif
        }

        public static void Log(object arg)
        {
#if DEBUG
            if (LogEnabled) Console.Write(ConvertEnumerableArgToCSVString(arg));
#endif
        }

        public bool PredictResultBasedOnCycle(IEnumerable<long> results, long targetIteration, out long prediction)
        {
            prediction = long.MinValue;
            if (results.Count() <= 2)
            {
                return false;
            }
            var resultArray = results.ToArray();
            var endIndex = resultArray.Length - 1;
            var last = resultArray[endIndex];
            int startIndex = -1;
            for (int i = resultArray.Length - 2; i >= 0; i--)
            {
                if (resultArray[i] == last)
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex < 0) return false;
            var cycleLength = endIndex - startIndex;
            var targetIndex = (int)(startIndex + (targetIteration - 1 - startIndex) % cycleLength);
            prediction = resultArray[targetIndex];
            LogLine("repeatVal={0},start={1},length={2},target={3},targetIndex={4},prediction={5}", last, startIndex, cycleLength, targetIteration, targetIndex, prediction);
            return true;
        }

        public LoopResult DetectLoop<T>(IEnumerable<T> results, int matchesRequired = 3)
        {
            if (matchesRequired < 1) throw new ArgumentOutOfRangeException("matches required", "must be >=1");
            if (results.Count() <= Math.Max(2, matchesRequired))
            {
                return new LoopResult { LoopDetected = false };
            }
            var resultArray = results.ToArray();
            var endIndex = resultArray.Length - 1;
            int startIndex = -1;
            // work from the end to make sure the loop has "kicked in" past any initial non-loop
            for (int i = matchesRequired; i < resultArray.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < matchesRequired; j++)
                {
                    if (!resultArray[i+j].Equals(resultArray[j]))
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    startIndex = i;
                    break;
                }
            }
            if (startIndex < 0)
            {
                return new LoopResult { LoopDetected = false };
            }
            var loopLength = endIndex - startIndex;

            return new LoopResult
                    {
                        LoopDetected = true,
                        StartIndex = startIndex- matchesRequired + 1,
                        LoopLength = loopLength
                    };

        }
    }

    public struct LoopResult
    {
        public bool LoopDetected;
        public long StartIndex;
        public long LoopLength;

        public override string ToString()
        {
            if (!LoopDetected) return "No loop";
            return string.Format("Loop starting at {0} of {1}", StartIndex, LoopLength);
        }
    }

    public static class DayUtils
    {
        public static string[] GetParts(this string input, string removeChars=null)
        {
            if (!string.IsNullOrEmpty(removeChars))
            {
                foreach (var c in removeChars)
                {
                    input = input.Replace(c, ' ');
                }
            }
            return input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetLines(this string input, string removeChars = null)
        {
            if (input == removeChars) throw new InvalidOperationException("input == removeChars, you really didn't mean that!");
            if (removeChars != null)
            {
                foreach (var c in removeChars)
                {
                    input = input.Replace(c, ' ');
                }
            }
            return input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
