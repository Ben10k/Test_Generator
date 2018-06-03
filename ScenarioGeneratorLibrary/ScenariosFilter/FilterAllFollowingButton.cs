using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public class FilterAllFollowingButton : ScenariosFilter {
        protected override List<Node> FilterNodeList(List<Node> oldNodes) {
            var newNodes = new List<Node>();
            foreach (var node in oldNodes) {
                newNodes.Add(node);
                if (ElementValidator.IsClickable(node.GetElement()))
                    break;
            }

            return newNodes;
        }
    }
}