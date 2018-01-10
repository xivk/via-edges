using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using System;
using System.IO;

namespace VIA.EdgesInRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            // STAGING: setup a routerdb.
            Download.ToFile("http://files.itinero.tech/data/OSM/planet/europe/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf").Wait();

            // build routerdb from raw OSM data.
            // check this for more info on RouterDb's: https://github.com/itinero/routing/wiki/RouterDb
            var routerDb = new RouterDb();
            using (var sourceStream = File.OpenRead("luxembourg-latest.osm.pbf"))
            {
                routerDb.LoadOsmData(sourceStream, Itinero.Osm.Vehicles.Vehicle.Car);
            }

            // some test locations in luxem.
            var profile = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();
            var router = new Router(routerDb);
            var location1 = new Coordinate(49.8245220050769f, 5.962958335876464f);
            var location2 = new Coordinate(49.8241828502970f, 5.969148874282837f);
            var location3 = new Coordinate(49.8280864472092f, 5.965898036956786f);
            var location4 = new Coordinate(49.6521994505660f, 6.266278624534607f);
            var location5 = new Coordinate(49.65199453942951f, 6.26626253128051f);

            // resolve test locations.
            var routerpoint1 = router.Resolve(profile, location1);
            var routerpoint2 = router.Resolve(profile, location2);
            var routerpoint3 = router.Resolve(profile, location3);
            var routerpoint4 = router.Resolve(profile, location4);
            var routerpoint5 = router.Resolve(profile, location5);

            // a few shorter test routes.
            var rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint1, routerpoint2, null);
            var edgeCoverage = router.CoveredEdges(routerpoint1, routerpoint2, rawRoute.Value);
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint2, routerpoint1, null);
            edgeCoverage = router.CoveredEdges(routerpoint1, routerpoint2, rawRoute.Value);
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint1, routerpoint3, null);
            edgeCoverage = router.CoveredEdges(routerpoint1, routerpoint3, rawRoute.Value);
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint3, routerpoint1, null);
            edgeCoverage = router.CoveredEdges(routerpoint3, routerpoint1, rawRoute.Value);

            // very long route with forward and backward mixed.
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint1, routerpoint4, null);
            edgeCoverage = router.CoveredEdges(routerpoint1, routerpoint4, rawRoute.Value);
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint4, routerpoint1, null);
            edgeCoverage = router.CoveredEdges(routerpoint4, routerpoint1, rawRoute.Value);

            // tiny within one edge routes.
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint5, routerpoint4, null);
            edgeCoverage = router.CoveredEdges(routerpoint5, routerpoint4, rawRoute.Value);
            rawRoute = router.TryCalculateRaw(profile, router.GetDefaultWeightHandler(profile), routerpoint4, routerpoint5, null);
            edgeCoverage = router.CoveredEdges(routerpoint4, routerpoint5, rawRoute.Value);
        }
    }
}