using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day11 :  Day
    {
        public override string First(string input)
        {
            return Solve(input, PanelColour.Black);
        }

        public override string Second(string input)
        {
            return Solve(input, PanelColour.White);
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

        private string Solve(string input, PanelColour colour)
        {
            var map = Paint(input, colour);
            LogEnabled = true;
            LogLine(map.GetStateString(PANEL_VALUE_DICT));
            return map.Count.ToString();
        }

        public class Robot
        {
            public Direction Direction;
            public XY XY;
        }

        public enum PanelColour
        {
            None=-1,
            Black=0,
            White=1
        }

        private Dictionary<PanelColour, char> PANEL_VALUE_DICT = new Dictionary<PanelColour, char>() { { PanelColour.None, '.' }, { PanelColour.Black, ' ' }, { PanelColour.White, '#' } };

        public Map<PanelColour> Paint(string input, PanelColour startColour)
        {
            LogEnabled = false;
            var program = input.GetParts(",").Select(s => long.Parse(s)).ToArray();
            var intcode = new Intcode(program);
            var panelMap = new Map<PanelColour>();
            var robot = new Robot()
            {
                Direction = Direction.North,
                XY = new XY(0, 0)
            };
            panelMap.Set(robot.XY, startColour);

            do
            {
                intcode.Resume();
                if (intcode.OutputQueue.Any())
                {
                    var colour = intcode.OutputQueue.Dequeue();
                    var turn = intcode.OutputQueue.Dequeue();
                    panelMap.Set(robot.XY, (PanelColour)colour);
                    robot.Direction = turn == 0 ? robot.Direction.TurnLeft() : robot.Direction.TurnRight();
                    robot.XY = robot.XY.Move(robot.Direction);
                }
                if (intcode.AwaitingInput)
                {
                    if (!panelMap.TryGetValue(robot.XY, out PanelColour colour)) colour = PanelColour.Black;
                    intcode.InputQueue.Enqueue((long)colour);
                }
            } while (intcode.AwaitingInput);

            return panelMap;
        }
    }
}
