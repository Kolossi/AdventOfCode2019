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
        public Direction Direction;
        public XY Position;
    }

    public class MapPathState<NodeType>
    {
        public Map<NodeType> Map;
        public Path Path;
    }

    public class MoveAttempt
    {
        public XY Position;
        public Direction Direction;
    }

    public class MapMoveResponse<NodeType>
    {
        public NodeType Value;
        public bool CanMove;
    }

    public class RouteSolver<NodeType>
    {
        // example of usage:
        //  2019 day 15

       public static MapWalkState<NodeType> WalkMap(
            MapWalkState<NodeType> mapState,
            Func<MapWalkState<NodeType>,Direction> getNextDirection,
            Func<MapWalkState<NodeType>, MapMoveResponse<NodeType>> getMoveResult,
            Func<MapWalkState<NodeType>,bool> isComplete)
        {
            mapState.Direction = getNextDirection(mapState);
            while (!isComplete(mapState))
            {
                var mapDetail = getMoveResult(mapState);
                var newPosition = mapState.Position.Move(mapState.Direction);
                mapState.Map.Set(newPosition, mapDetail.Value);
                if (mapDetail.CanMove)
                {
                    mapState.Position = newPosition;
                }
                mapState.Direction = getNextDirection(mapState);
            }

            return mapState;
        }
     
        // example of usage:
        //  2019 day 15
        //  2019 day 17

        public static Path FindSingleShortestPath(
            XY startPosition,
            Map<NodeType> map,
            Func<MapPathState<NodeType>, IEnumerable<XY>> getNextNodes,
            Func<MapPathState<NodeType>, bool> haveFoundEnd)
        {
            return FindSingleShortestPath(startPosition, map, getNextNodes, haveFoundEnd, RouteSolver<NodeType>.ScorePathLength);
        }

        public static Path FindSingleShortestPath(
            XY startPosition,
            Map<NodeType> map,
            Func<MapPathState<NodeType>, IEnumerable<XY>> getNextNodes,
            Func<MapPathState<NodeType>, bool> haveFoundEnd,
            Func<MapPathState<NodeType>, long> scorePathLength)
        {
            var distanceMap = new Map<long>();
            var path = new Path();
            Path shortestPath = null;
            long shortestPathLengthScore = long.MaxValue;
            path.Move(startPosition);
            var toProcess = new Queue<Path>();
            toProcess.Enqueue(path);
            while (toProcess.Any())
            {
                path = toProcess.Dequeue();
                MapPathState<NodeType> mapPathState = new MapPathState<NodeType>() { Map = map, Path = path };
                var pathLengthScore = scorePathLength(mapPathState);
                if (pathLengthScore >= shortestPathLengthScore) continue;
                if (haveFoundEnd(mapPathState))
                {
                    shortestPath = mapPathState.Path;
                    shortestPathLengthScore = pathLengthScore;
                    continue;
                }
                foreach (var newXY in getNextNodes(mapPathState))
                {
                    if (path.Visited.Has(newXY)) continue;
                    toProcess.Enqueue(new Path(path).Move(newXY));
                }
            }

            return shortestPath;
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

        public static long ScorePathLength(MapPathState<NodeType> mapPathState)
        {
            return mapPathState.Path.Length;
        }

    }
}
