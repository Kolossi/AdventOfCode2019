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

    public class RouteSolver<NodeType>
    {
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
