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
            var result = RouteSolver<Body>.FindSingleRoute(
                start,
                (Body body) => body.Orbitees.Union(new Body[] { body.Orbits }),
                (Body body) => body.Name == end.Name
            );
            if (!result.Found) throw new InvalidOperationException();
            return (result.Route.Count-1).ToString();
        }

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

        public override string ToString()
        {
            return string.Format("{0}({1}:{2}", Name, Orbits.Name, Day.ConvertEnumerableArgToCSVString(Orbitees.Select(o => o.Name)));
        }

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
