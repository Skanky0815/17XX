using System.Collections.Generic;
using Map.Objects;
using System.Linq;

namespace Map
{
    public static class SplinePathfinder
    {
        public static List<string> FindPath(string startId, string goalId, KnotGraph graph)
        {
            var openSet = new SortedSet<(float fScore, string id)>(Comparer<(float, string)>.Create((a, b) =>
            {
                int compare = a.Item1.CompareTo(b.Item1);
                return compare == 0 ? a.Item2.CompareTo(b.Item2) : compare;
            }));

            var cameFrom = new Dictionary<string, string>();
            var gScore = new Dictionary<string, float> { [startId] = 0 };
            var fScore = new Dictionary<string, float> { [startId] = Heuristic(startId, goalId) };

            openSet.Add((fScore[startId], startId));

            while (openSet.Count > 0)
            {
                var current = openSet.Min.id;
                openSet.Remove(openSet.Min);

                if (current == goalId)
                    return ReconstructPath(cameFrom, current);

                foreach (var neighbor in graph.GetNeighbors(current))
                {
                    float tentativeG = gScore[current] + 1f; // konstante Kosten, optional anpassbar

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, goalId);

                        // Duplikate vermeiden
                        openSet.RemoveWhere(e => e.id == neighbor);
                        openSet.Add((fScore[neighbor], neighbor));
                    }
                }
            }

            return new List<string>(); // Kein Pfad gefunden
        }

        private static float Heuristic(string aId, string bId)
        {
            // Beispiel: einfache Heuristik basierend auf KnotIndex-Differenz
            var aParts = aId.Split(':').Select(int.Parse).ToArray();
            var bParts = bId.Split(':').Select(int.Parse).ToArray();

            int splineDiff = System.Math.Abs(aParts[0] - bParts[0]);
            int knotDiff = System.Math.Abs(aParts[1] - bParts[1]);

            return splineDiff + knotDiff;
        }

        private static List<string> ReconstructPath(Dictionary<string, string> cameFrom, string current)
        {
            var path = new List<string> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }
    }
}