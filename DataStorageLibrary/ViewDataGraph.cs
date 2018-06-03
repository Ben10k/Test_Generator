using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace DataStorageLibrary {
    public class ViewDataGraph {
        private readonly AdjacencyGraph<string, Edge<string>> _graph = new AdjacencyGraph<string, Edge<string>>();

        public void AddEdge(string source, string destination) {
            _graph.AddEdge(new Edge<string>(source, destination));
        }

        public void AddVertex(string vertex) {
            _graph.AddVertex(vertex);
        }

        public AdjacencyGraph<string, Edge<string>> GetGraph() {
            return _graph;
        }

        public List<Edge<String>> GetAllEdges() {
            return _graph.Edges.ToList();
        }

        public bool ContainsVertex(string vertex) {
            return _graph.ContainsVertex(vertex);
        }
        
        public bool ContainsBothVerices(string from, string to) {
            return _graph.ContainsVertex(from) && _graph.ContainsVertex(to);
        }
    }
}