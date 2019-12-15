using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    public class RouteResult<NodeType>
    {
        public bool Found;
        public List<NodeType> Route;
    }

    class RouteAttempt<NodeType>
    {
        public NodeType Node;
        public List<NodeType> Route;
        public HashSet<NodeType> Visited;
    }

    public class MapWalkState<NodeType>
    {
        public Map<NodeType> Map;
        public XY Position;
    }

    public class MoveAttempt
    {
        public XY Position;
        public Direction Direction;
    }

    public class MapDetailResponse<NodeType>
    {
        public NodeType Value;
        public bool CanMove;
    }

    public class RouteSolver<NodeType>
    {
        // example of usage:
        //  2019 day 15
        public static MapWalkState<NodeType> WalkMap(
            XY startPosition,
            Func<MapWalkState<NodeType>,IEnumerable<Direction>> getNextDirections,
            Func<MapWalkState<NodeType>, Direction, MapDetailResponse<NodeType>> getMapDetail,
            Func<MapWalkState<NodeType>,bool> isComplete)
        {
            var mapState = new MapWalkState<NodeType>()
            {
                Position = new XY(startPosition),
                Map = new Map<NodeType>()
            };
            var moveAttemptQueue = new Queue<MoveAttempt>(getNextDirections(mapState)
                .Select(d=>new MoveAttempt()
                {
                    Position = startPosition,
                    Direction = d 
                }));
            while (moveAttemptQueue.Any() && !isComplete(mapState))
            {
                var moveAttempt = moveAttemptQueue.Dequeue();
                mapState.Position = moveAttempt.Position;
                var mapDetail = getMapDetail(mapState, moveAttempt.Direction);
                mapState.Map.Set(mapState.Position, mapDetail.Value);
                if (!mapDetail.CanMove) continue;
                foreach (var nextDirection in getNextDirections(mapState))
                {
                    moveAttemptQueue.Enqueue(new MoveAttempt()
                    {
                        Position = mapState.Position,
                        Direction = nextDirection
                    });
                }
            }

            return mapState;
        }

        internal static object MapWalkState(XY xY, object getNextDroidDirections, object getDroidMapDetail, object foundOxygen)
        {
            throw new NotImplementedException();
        }

        // example of usage:
        //   2019 day 6
        public static RouteResult<NodeType> FindSingleRoute(
            NodeType startNode,
            Func<NodeType, IEnumerable<NodeType>> getNextNodes,
            Func<NodeType, bool> haveFoundEnd
        )
        {
            var attemptQueue = new Queue<RouteAttempt<NodeType>>();
            attemptQueue.Enqueue(new RouteAttempt<NodeType>()
            {
                Node = startNode,
                Route = new List<NodeType>(),
                Visited = new HashSet<NodeType>()
            });

            while (attemptQueue.Any())
            {
                var attempt = attemptQueue.Dequeue();
                var node = attempt.Node;
                var route = attempt.Route;
                var visited = attempt.Visited;
                route.Add(node);
                if (haveFoundEnd(node)) return new RouteResult<NodeType>() { Found = true, Route = route };
                visited.Add(node);

                foreach (var nextNode in getNextNodes(node).Where(n => n != null).Except(visited))
                {
                    var nextRoute = new List<NodeType>(route);
                    var nextVisited = new HashSet<NodeType>(visited);
                    attemptQueue.Enqueue(new RouteAttempt<NodeType>()
                    {
                        Node = nextNode,
                        Route = nextRoute,
                        Visited = nextVisited
                    });
                }
            }
            return new RouteResult<NodeType> { Found = false };
        }
    }
}
