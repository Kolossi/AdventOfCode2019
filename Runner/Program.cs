using System;
using System.Collections.Generic;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Day> days = new List<Day>()
            {
                //new Day01(),
                //new Day02(),
                //new Day03(),
                //new Day04(),
                //new Day05(),
                //new Day06(),
                //new Day07(),
                //new Day08(),
                //new Day09(),
                //new Day10(),
                //new Day11(),
                //new Day12(),
                //new Day13(),
                //new Day14(),
                //new Day15(),
                //new Day16(), // todo part 2 fft
                //new Day17(),
                //new Day18(), // todo part 2 doors and keys
                //new Day19(),
                //new Day20(), // todo part 2 donut maze
                //new Day21(), 
                //new Day22(), // todo part 2 modulo number theory
                //new Day23(),
                //new Day24(),
                new Day25()
            };

            foreach (var day in days)
            {
                Console.WriteLine(day.GetType().Name);
                Console.WriteLine("  First  : ");
#if DEBUG
                day.TestFirst();
#endif
                Console.WriteLine(day.SolveFirst());
                Console.WriteLine("  Second : ");
#if DEBUG
                day.TestSecond();
#endif
                Console.WriteLine(day.SolveSecond());
                Console.WriteLine();
                Console.WriteLine("FINISHED");
                Console.WriteLine();
            }
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
