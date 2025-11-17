using System.Collections.Generic;

namespace Map.Objects
{
    public class KnotGraph
    {
        private readonly Dictionary<string, List<string>> _adjacency = new();

        public void AddEdge(string idA, string idB)
        {
            if (!_adjacency.ContainsKey(idA)) _adjacency[idA] = new List<string>();
            if (!_adjacency.ContainsKey(idB)) _adjacency[idB] = new List<string>();

            _adjacency[idA].Add(idB);
            _adjacency[idB].Add(idA);
        }

        public List<string> GetNeighbors(string knotId)
        {
            return _adjacency.TryGetValue(knotId, out var neighbors) ? neighbors : new List<string>();
        }
    }
}

