using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day06 :  Day
    {
        public override string First(string input)
        {
            var solarSystem = new SolarSystem(input);
            var total = solarSystem.Bodies.Values.Select(b => b.Depth).Sum();
            return total.ToString();
        }

        public override string Second(string input)
        {
           var solarSystem = new SolarSystem(input);
            var start = solarSystem.Bodies["YOU"].Orbits;
            var end = solarSystem.Bodies["SAN"].Orbits;
            var visited = new HashSet<string>();
            var route = new List<string>();
            route.Add(start.Name);
            visited.Add("SAN");
            visited.Add("YOU");
            visited.Add(start.Name);
            var result = TryPath(solarSystem, visited, route, start, end);
            if (!result.Found) throw new InvalidOperationException();
            return result.Route.Count.ToString();
        }
        public class Result
        {
            public bool Found;
            public List<string> Route;
        }

        public Result TryPath(SolarSystem solarSystem, HashSet<string> visited, List<string> route, Body start, Body end)
        {
            foreach (var nextBodyName in start.Orbitees.Union(new Body[] { start.Orbits }).Where(b => b != null).Select(b => b.Name).Except(visited))
            {
                if (nextBodyName == end.Name) return new Result() { Found = true, Route = route };
                var newRoute = new List<string>(route);
                var newVisited = new HashSet<string>(visited);
                newRoute.Add(nextBodyName);
                newVisited.Add(nextBodyName);
                var nextBody = solarSystem.Bodies[nextBodyName];
                var result = TryPath(solarSystem, newVisited, newRoute, nextBody, end);
                if (result.Found) return result;
            }
            return new Result { Found = false };
        }

        //public override string SecondTest(string input)
        //{
        //    throw new NotImplementedException("SecondTest");
        //}

        ////////////////////////////////////////////////////////
    }

    public class SolarSystem
    {
        public Dictionary<string, Body> Bodies = new Dictionary<string, Body>();
        public SolarSystem(string input)
        {
            Bodies["COM"]=new Body()
            {
                Name = "COM"
            };


            foreach (var name in input.GetLines().SelectMany(l=>l.Split(")")))
            {
                AddBody(name);
            }
            foreach (var spec in input.GetLines())
            {
                AddOrbit(spec);
            }
            Bodies["COM"].SetDepthOfOrbitees();
        }

        public SolarSystem AddBody(string name)
        {
            var body = new Body()
            {
                Name = name,
            };
            Bodies[body.Name] = body;

            return this;
        }

        public SolarSystem AddOrbit(string spec)
        {
            var parts = spec.Split(")");
            var orbits = Bodies[parts[0]];
            var body = Bodies[parts[1]];
            body.Orbits = orbits;
 //           body.Depth = orbits.Depth + 1;
            orbits.Orbitees.Add(body);

            return this;
        }
    }
    public class Body
    {
        public string Name;
        public Body Orbits;
        public List<Body> Orbitees = new List<Body>();
        public int Depth;

        public Body SetDepthOfOrbitees()
        {
            foreach (var orbitee in Orbitees)
            {
                orbitee.Depth = this.Depth + 1;
                orbitee.SetDepthOfOrbitees();
            }

            return this;
        }
    }
}
