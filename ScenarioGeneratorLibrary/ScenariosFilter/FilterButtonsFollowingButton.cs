using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public class FilterButtonsFollowingButton : ScenariosFilter {
        protected override List<Node> FilterNodeList(List<Node> oldNodes) {
            var newNodes = new List<Node>();

            bool encounteredClickable = false;

            foreach (var node in oldNodes)
                if (ElementValidator.IsClickable(node.GetElement())) {
                    if (!encounteredClickable) {
                        encounteredClickable = true;
                        newNodes.Add(node);
                    }
                }
                else {
                    newNodes.Add(node);
                }


            return newNodes;
        }
    }
}