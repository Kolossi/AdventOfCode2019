using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day22 :  Day
    {
        public override string First(string input)
        {
            var cards = new KLinkedList<long>(Enumerable.Range(0, 10007).Select(x=>(long)x));
            var result = Shuffle(cards, input.GetLines());
            int pos = 0;
            var card = result.First;
            while (true)
            {
                if (card.Value == (long)2019) break;
                card = card.Advance(1);
                pos++;
            }
            return pos.ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
        }

        //public override string FirstTest(string input)
        //{
        //    throw new NotImplementedException("FirstTest");
        //}
        public override string FirstTest(string input)
        {
            var cards = new KLinkedList<long>(Enumerable.Range(0, 10).Select(x=>(long)x));
            ShowInputForOKTest = false;
            var result = Shuffle(cards, input.GetLines());
            return string.Join(" ", result.Values);
        }

        public override string SecondTest(string input)
        {
            return DealWith(10, long.Parse(input), 3).ToString();
        }

        ////////////////////////////////////////////////////////
        
        public KLinkedList<long> Shuffle(KLinkedList<long> cards, string[] instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.StartsWith("deal into"))
                {
                    cards = cards.Reverse();
                }
                else if (instruction.StartsWith("cut"))
                {
                    var count = long.Parse(instruction.GetParts().Last());
                    if (count < 0) count = cards.Length + count;
                    var sliceStart = cards.First;
                    var newFirst = sliceStart.Advance(count, wrap:true);
                    var sliceEnd = newFirst.Prev;
                    var last = cards.Last;
                    sliceEnd.Next = null;
                    newFirst.Prev = null;
                    last.Next = sliceStart;
                    sliceStart.Prev = last;
                    cards.First = newFirst;
                    cards.Last = sliceEnd;
                }
                else if (instruction.StartsWith("deal with"))
                {
                    var inc = long.Parse(instruction.GetParts().Last());
                    var newCards = new long[cards.Length];
                    long pos = 0;
                    var card = cards.First;
                    while (card != null)
                    {
                        newCards[pos] = card.Value;
                        pos = (pos + inc) % cards.Length;
                        card = card.Advance(1);
                    }
                    cards = new KLinkedList<long>(newCards);

                }
                else throw new InvalidOperationException();
            }

            return cards;
        }

        private long GetCardPosition(long cardCount, string[] instructions, long osition)
        {
            // note not card value but card position
            throw new NotImplementedException();
            //var position = cardValue;
            //foreach (var instruction in instructions)
            //{
            //    if (instruction.StartsWith("deal into"))
            //    {
            //        position = cardCount - cardValue;
            //    }
            //    else if (instruction.StartsWith("cut"))
            //    {
            //        var count = long.Parse(instruction.GetParts().Last());
            //        position = position - count;
            //        if (position < 0) position = position + cardCount;
            //    }
            //    else if (instruction.StartsWith("deal with"))
            //    {
            //        var inc = long.Parse(instruction.GetParts().Last());
            //        position = DealWith(cardCount, position, inc);
            //    }
            //    else throw new InvalidOperationException();
            //}
            //return position;
            
        }

        private static long DealWith(long cardCount, long position, long inc)
        {
            throw new NotImplementedException();
            var wholeIncsInPack = (cardCount / inc);   //8,10,3 : 3
            var packRemainder = (cardCount % inc);   //8,10,3 : 1
            long incMult = position % (wholeIncsInPack + 1);
            //long offset = position / (wholeIncsInPack + 1);
            long offset = inc - packRemainder*(position/(wholeIncsInPack +1)) % inc;
            return (incMult * inc) + offset;
            
            //var incsInPackRemainder = cardCount % inc; //8,10,3 : 1
            //var wholePackCyclesInPos = ((position / inc) / wholeIncsInPack);  // 8,10,3 8/3 = 2
            //var remainder = position - (wholePackCyclesInPos * wholeIncsInPack * inc) - (wholePackCyclesInPos * incsInPackRemainder); // 8,10,3 : position-(2*3*3)-(2*1)
            //position -= wholePackCyclesInPos* 
            //if (position < wholeIncsInPack) position = position * inc;
            //return position;
        }
    }
}
