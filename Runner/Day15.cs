using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day15 :  Day
    {
        public override string First(string input)
        {
            var data = input.GetParts(",").Select(p => long.Parse(p)).ToArray();
            var intcode = new Intcode(data);
            var map = GetMap(intcode);
            return "x";
        }


        public override string Second(string input)
        {
            throw new NotImplementedException("Second");
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

        private Dictionary<long, Direction> RobotToMapDirection = new Dictionary<long, Direction>()
        {
            { 1, Direction.North },
            { 2, Direction.South },
            { 3, Direction.West },
            { 4, Direction.East }
        };

        private Dictionary<Direction, long> MapToRobotDirection = new Dictionary<Direction, long>()
        {
            { Direction.North, 1 },
            { Direction.South, 2 },
            { Direction.West, 3 },
            { Direction.East, 4 }
        };

        public enum ShipMap
        {
            Unexplored = 0,
            Free = 1,
            Wall = 2,
            Oxygen = 3
        }

        public Dictionary<ShipMap, char> SHIP_VALUE_MAP = new Dictionary<ShipMap, char>()
        {
            {ShipMap.Unexplored, ' ' },
            {ShipMap.Free, '.' },
            {ShipMap.Wall, 'W' },
            {ShipMap.Oxygen, 'O' }
        };

        //        public static MapWalkState<NodeType> WalkMap(
            //XY startPosition,
            //Func<MapWalkState<NodeType>,IEnumerable<Direction>> getNextDirections,
            //Func<MapWalkState<NodeType>, Direction, MapDetailResponse<NodeType>> getMapDetail,
            //Func<MapWalkState<NodeType>,bool> isComplete)
        {

        private IEnumerable<Direction> GetNextDirections(MapWalkState<ShipMap> mapWalkState)
        {
            return XY.GetAllDirections().Where(d => !mapWalkState.Map.Has(mapWalkState.Position.Move(d)));
        }

        private bool IsComplete(MapWalkState<ShipMap> mapWalkState)
        {
            return mapWalkState.Map.GetAllValues().Any(v => v == ShipMap.Oxygen);
        }

        private MapDetailResponse<ShipMap> GetMapDetail(MapWalkState<ShipMap> mapWalkState, Direction direction)
        {
            // TODO: get droid to mapWalkState.Pos:
            RouteSolver<ShipMap>.FindSingleMapRoute(mapWalkState.Map, currentPos, mapWalkState.Pos)
            //  then run intcode with direction (mapped to droid direction) as input
            // then go back to previous position
        }

        private Map<ShipMap> GetMap(Intcode intcode)
        {
            var mapState = RouteSolver<ShipMap>.MapWalkState(new XY(0,0), GetNextDirections, GetMapDetail, IsComplete)
        }
    }
}
