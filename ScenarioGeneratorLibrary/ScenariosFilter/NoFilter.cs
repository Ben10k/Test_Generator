using System.Collections.Generic;
using DataStorageLibrary;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public class NoFilter : ScenariosFilter {
        protected override List<Node> FilterNodeList(List<Node> oldNodes) {
            var newNodes = new List<Node>();
            foreach (var node in oldNodes)
                newNodes.Add(node);
            return newNodes;
        }
    }
}