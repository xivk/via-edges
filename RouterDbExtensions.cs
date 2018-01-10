using Itinero;
using Itinero.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VIA.EdgesInRoute
{
    static class RouterDbExtensions
    {
        /// <summary>
        /// Returns a coverage of the given edge.
        /// </summary>
        /// <returns></returns>
        public static List<EdgeCover> CoveredEdges(this Router router, RouterPoint origin, RouterPoint target, EdgePath<float> path)
        {
            // TODO: (optional) parameter validation.
            // TODO: (optional) make sure the routerpoints match the path.

            var pathList = path.ToList();
            var edges = new List<EdgeCover>(pathList.Count - 1);

            // loop over all pairs, or in other words over all edges.
            EdgePath<float> previous = null;
            for (var i = 0; i < pathList.Count; i++)
            {
                var current = pathList[i];
                if (previous != null)
                {
                    var startPercentage = 0f;
                    if (previous.Vertex == Constants.NO_VERTEX)
                    {
                        startPercentage = ((float)origin.Offset / ushort.MaxValue) * 100.0f;
                        if (current.Edge < 0)
                        {
                            startPercentage = 100 - startPercentage;
                        }
                    }
                    var endPercentage = 100f;
                    if (current.Vertex == Constants.NO_VERTEX)
                    {
                        endPercentage = ((float)target.Offset / ushort.MaxValue) * 100.0f;
                        if (current.Edge < 0)
                        {
                            endPercentage = 100 - endPercentage;
                        }
                    }
                    if (current.Vertex != Constants.NO_VERTEX &&
                        previous.Vertex != Constants.NO_VERTEX)
                    {
                        var edge = router.Db.Network.GetEdgeEnumerator(previous.Vertex).First(x => x.To == current.Vertex);
                    }

                    edges.Add(new EdgeCover()
                    {
                        DirectedEdgeId = new DirectedEdgeId(current.Edge),
                        EndPercentage = endPercentage,
                        StartPercentage = startPercentage
                    });
                }
                previous = current;
            }

            return edges;
        }

        public class EdgeCover
        {
            /// <summary>
            /// Gets or sets the edge id.
            /// </summary>
            public DirectedEdgeId DirectedEdgeId { get; set; }
            
            /// <summary>
            /// Gets or sets the start percentage.
            /// </summary>
            public float StartPercentage { get; set; }

            /// <summary>
            /// Gets or sets the end percentage.
            /// </summary>
            public float EndPercentage { get; set; }

            public override string ToString()
            {
                return string.Format("{0}[{1}%-{2}%]", this.DirectedEdgeId,
                    this.StartPercentage.ToInvariantString(), this.EndPercentage.ToInvariantString());
            }
        }
    }
}
