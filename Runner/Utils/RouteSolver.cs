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

    public class RouteAttempt<NodeType>
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
        public ICloneable MyState;
    }

    public class MapPathState<NodeType>
    {
        public Map<NodeType> Map;
        public Path Path;
        public ICloneable MyState;
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
        private const int MAX_ATTEMPTS_QUEUED = 1000000;

        // example of usage:
        //  2019 day 15

        public static MapWalkState<NodeType> WalkMap(
             MapWalkState<NodeType> mapState,
             Func<MapWalkState<NodeType>, Direction> getNextDirection,
             Func<MapWalkState<NodeType>, MapMoveResponse<NodeType>> getMoveResult,
             Func<MapWalkState<NodeType>, bool> isComplete)
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
        //  2019 day 18 (to make NodeLinks)

        public static Path FindSingleShortestPath(
            XY startPosition,
            Map<NodeType> map,
            Func<MapPathState<NodeType>, IEnumerable<XY>> getNextNodes,
            Func<MapPathState<NodeType>, bool> haveFoundEnd,
            bool routeRevisitsAllowed = false
            )
        {
            return FindSingleShortestPath(startPosition, map, getNextNodes, haveFoundEnd, RouteSolver<NodeType>.ScorePathLength, routeRevisitsAllowed);
        }

        public static Path FindSingleShortestPath(
            XY startPosition,
            Map<NodeType> map,
            Func<MapPathState<NodeType>, IEnumerable<XY>> getNextNodes,
            Func<MapPathState<NodeType>, bool> haveFoundEnd,
            Func<MapPathState<NodeType>, long> scorePath,
            bool routeRevisitsAllowed = false)
        {
            var distanceMap = new Map<long>();
            var originalPath = new Path();
            Path shortestPath = null;
            long shortestPathLengthScore = long.MaxValue - 1;
            originalPath.Move(startPosition);
            var toProcess = new Queue<MapPathState<NodeType>>();
            toProcess.Enqueue(new MapPathState<NodeType>()
            {
                Path = originalPath,
                Map = map
            });
            while (toProcess.Any())
            {
                var mapPathState = toProcess.Dequeue();
                var pathLengthScore = scorePath(mapPathState);
                if (!distanceMap.TryGetValue(mapPathState.Path.XY, out long bestDistanceToHere))
                {
                    distanceMap.Set(mapPathState.Path.XY, pathLengthScore);
                }
                else if (pathLengthScore >= bestDistanceToHere && !routeRevisitsAllowed)
                {
                    continue;
                }
                if (pathLengthScore >= shortestPathLengthScore) continue;
                if (haveFoundEnd(mapPathState))
                {
                    shortestPath = mapPathState.Path;
                    shortestPathLengthScore = pathLengthScore;
                    continue;
                }

                IEnumerable<XY> nextNodes = getNextNodes(mapPathState);
                foreach (var newXY in nextNodes)
                {
                    if (originalPath.Visited.Has(newXY) && !routeRevisitsAllowed) continue;
                    toProcess.Enqueue(new MapPathState<NodeType>()
                    {
                        Path = new Path(mapPathState.Path).Move(newXY),
                        Map = map,
                        MyState = (ICloneable)mapPathState.MyState?.Clone()
                    });
                    //throw new NotImplementedException("%%% the above clone doesnt seem to work start path a,b newXY = c, get path a,c");
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

                var nextNodes = getNextNodes(node);
                if (nextNodes == null) continue;
                foreach (var nextNode in nextNodes.Where(n => n != null).Except(visited))
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

        public static List<NodeType> FindSingleShortestRoute(
            NodeType startNode,
            Func<RouteAttempt<NodeType>, IEnumerable<NodeType>> getNextNodes,
            Func<RouteAttempt<NodeType>, bool> haveFoundEnd,
            Func<RouteAttempt<NodeType>, long> scoreRoute,
            Func<RouteAttempt<NodeType>, object> getGlobalVisitedHash = null,
            bool routeRevisitsAllowed = false
        )
        {
            var bestScoreForHash = new Dictionary<object, long>();
            var attemptQueue = new Queue<RouteAttempt<NodeType>>();
            attemptQueue.Enqueue(new RouteAttempt<NodeType>()
            {
                Node = startNode,
                Route = new List<NodeType>(),
                Visited = new HashSet<NodeType>()
            });
            List<NodeType> shortestRoute = null;
            long shortestRouteScore = long.MaxValue - 1;

            while (attemptQueue.Any())
            {
                if (attemptQueue.Count() > MAX_ATTEMPTS_QUEUED) throw new InvalidOperationException(">1m attempts queued, forget about it!");
                var attempt = attemptQueue.Dequeue();
                var node = attempt.Node;
                var route = attempt.Route;
                var visited = attempt.Visited;
                route.Add(node);
                visited.Add(node);
                var routeScore = scoreRoute(attempt);
                if (routeScore >= shortestRouteScore) continue;
                if (getGlobalVisitedHash!=null)
                {
                    var hash = getGlobalVisitedHash(attempt);
                    if (bestScoreForHash.TryGetValue(hash, out long prevScore) && prevScore < routeScore) continue;
                    bestScoreForHash[hash] = routeScore;
                }

                if (haveFoundEnd(attempt))
                {
                    shortestRoute = attempt.Route;
                    shortestRouteScore = routeScore;
                    continue;
                }

                var nextNodes = getNextNodes(attempt);
                if (nextNodes == null) continue;

                nextNodes = nextNodes.Where(n => n != null);
                if (!routeRevisitsAllowed) nextNodes = nextNodes.Except(visited);
                foreach (var nextNode in nextNodes)
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
            return shortestRoute;
        }

        public static long ScorePathLength(MapPathState<NodeType> mapPathState)
        {
            return mapPathState.Path.Length;
        }

        private static object RevisitsAllowed(RouteAttempt<NodeType> routeAttempt)
        {
            return default(NodeType);
        }

        private static object GetSimpleRevisitHash(RouteAttempt<NodeType> routeAttempt)
        {
            return routeAttempt.Node;
        }

    }
}
