using System.Collections.Generic;

namespace Map.Objects
{
    public class KnotGraph
    {
        private readonly Dictionary<string, List<string>> adjacency = new();

        public void AddEdge(string idA, string idB)
        {
            if (!adjacency.ContainsKey(idA)) adjacency[idA] = new();
            if (!adjacency.ContainsKey(idB)) adjacency[idB] = new();

            adjacency[idA].Add(idB);
            adjacency[idB].Add(idA);
        }

        public List<string> GetNeighbors(string knotId)
        {
            return adjacency.TryGetValue(knotId, out var neighbors) ? neighbors : new();
        }
    }
}

