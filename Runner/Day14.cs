using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day14 : Day
    {
        public override string First(string input)
        {
            LogEnabled = false;
            ShowInputForOKTest = false;
            var reactionDefinitions = GetReactionDefinitions(input);
            var reactionVessel = new ReactionVessel()
            {
                Reaction = new ReactionDefinition(reactionDefinitions["FUEL"]),
                ReactionDefinitions = reactionDefinitions
            };
            return reactionVessel.React().OreUsed().ToString();
        }

        public override string Second(string input)
        {
            return BinaryChopToFindFuel(input).ToString();
        }


        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}

        //public override string SecondTest(string input)
        //{
        //    throw new NotImplementedException("SecondTest");
        //}

        ////////////////////////////////////////////////////////

        public class ReactionUnit
        {
            public long Quantity;
            public string Chemical;

            public ReactionUnit()
            {

            }

            public ReactionUnit(ReactionUnit other)
            {
                Quantity = other.Quantity;
                Chemical = other.Chemical;
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", Quantity, Chemical);
            }
        }

        public class ReactionDefinition
        {
            public List<ReactionUnit> Inputs;
            public ReactionUnit Output;

            public ReactionDefinition()
            {

            }

            public ReactionDefinition(ReactionDefinition other)
            {
                Output = new ReactionUnit(other.Output);
                Inputs = other.Inputs.Select(i => new ReactionUnit(i)).ToList();
            }

            public override string ToString()
            {
                return string.Format("{0} => {1}", ConvertEnumerableArgToCSVString(Inputs), Output);
            }
        }

        public class ReactionVessel
        {
            public ReactionDefinition Reaction;
            public Dictionary<string, long> LeftOvers;
            public Dictionary<string, ReactionDefinition> ReactionDefinitions;

            public ReactionVessel React(long outputMultiplier = 1)
            {
                Reaction.Output.Quantity *= outputMultiplier;
                foreach (var input in Reaction.Inputs) input.Quantity *= outputMultiplier;
                if (LeftOvers == null) LeftOvers = new Dictionary<string, long>();
                LogLine(ReactionDefinitions);
                while (Reaction.Inputs.Any(r => r.Chemical != "ORE"))
                {
                    LogLine();
                    LogLine("-===============================-");
                    LogLine();
                    LogLine(Reaction);
                    foreach (var unit in Reaction.Inputs.Where(r => r.Chemical != "ORE").ToArray())
                    {
                        Log("unit {0}: ", unit);
                        var unitReaction = ReactionDefinitions[unit.Chemical];
                        LogLine("reaction: {0}", unitReaction);
                        LogLine("leftovers: {0}", LeftOvers);
                        Reaction.Inputs.Remove(unit);
                        var quantityNeeded = unit.Quantity;
                        LeftOvers.TryGetValue(unit.Chemical, out long previousLeftover);
                        var leftOversToUse = Math.Min(previousLeftover, quantityNeeded);
                        quantityNeeded = Math.Max(0, quantityNeeded - leftOversToUse);
                        var multiplier = (long)Math.Ceiling((decimal)quantityNeeded / unitReaction.Output.Quantity);
                        var leftover = (unitReaction.Output.Quantity * multiplier) - quantityNeeded + previousLeftover - leftOversToUse;
                        if (leftover < 0) throw new InvalidOperationException("-ve leftover?");
                        LeftOvers[unit.Chemical] = leftover;
                        Reaction.Inputs.AddRange(unitReaction.Inputs.Select(i => new ReactionUnit() { Chemical = i.Chemical, Quantity = i.Quantity * multiplier }));
                        LogLine("After: {0}", Reaction);
                    }
                }
                return this;
            }

            public long OreUsed()
            {
                if (Reaction.Inputs.Any(r => r.Chemical != "ORE")) throw new InvalidOperationException("Reaction incomplete");
                LeftOvers.TryGetValue("ORE", out long leftover);
                return Reaction.Inputs.Sum(i => i.Quantity) + leftover;
            }
        }

        public Dictionary<string, ReactionDefinition> GetReactionDefinitions(string input)
        {
            var reactionDefinitions = new Dictionary<string, ReactionDefinition>();
            foreach (var line in input.GetLines())
            {
                var parts = line.Split("=>");
                var inputs = parts[0].Split(",");
                var output = parts[1];
                var def = new ReactionDefinition()
                {
                    Inputs = inputs.Select(i => GetReactionUnit(i)).ToList(),
                    Output = GetReactionUnit(output)
                };
                if (reactionDefinitions.ContainsKey(def.Output.Chemical)) throw new InvalidOperationException("dup");
                reactionDefinitions[def.Output.Chemical] = def;
            }
            return reactionDefinitions;
        }

        private ReactionUnit GetReactionUnit(string input)
        {
            var parts = input.GetParts();
            return new ReactionUnit()
            {
                Quantity = long.Parse(parts[0]),
                Chemical = parts[1]
            };
        }

        private long BinaryChopToFindFuel(string input)
        {
            long oreAvailable = 1000000000000;
            var reactionDefinitions = GetReactionDefinitions(input);
            long outputMultiplier = 1;
            long multJump = 1;
            long oreUsed = 0;
            long lastOreUsed;
            while (true)
            {
                var reactionVessel = new ReactionVessel()
                {
                    Reaction = new ReactionDefinition(reactionDefinitions["FUEL"]),
                    ReactionDefinitions = reactionDefinitions
                };
                lastOreUsed = oreUsed;
                oreUsed = reactionVessel.React(outputMultiplier).OreUsed();
                if (oreUsed == oreAvailable) return outputMultiplier;
                if (multJump == 1 && lastOreUsed < oreAvailable && oreUsed > oreAvailable) return outputMultiplier - 1;
                if (multJump == 1 && lastOreUsed > oreAvailable && oreUsed < oreAvailable) return outputMultiplier;
                if (outputMultiplier == 1)
                {
                    multJump = (long)(1.2 * oreAvailable / (oreUsed * outputMultiplier));
                    outputMultiplier = multJump;
                }
                else
                {
                    multJump = Math.Max(1, (multJump / 2));
                    outputMultiplier += multJump * (oreUsed < oreAvailable ? 1 : -1);
                }
            }
        }
    }
}
