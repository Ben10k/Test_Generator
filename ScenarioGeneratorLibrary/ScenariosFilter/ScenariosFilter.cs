using System.Collections.Generic;
using DataStorageLibrary;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public abstract class ScenariosFilter : IScenariosFilter {
        public List<List<Node>> Filter(List<List<Node>> input) {
            var output = new List<List<Node>>();
            foreach (List<Node> oldNodes in input)
                output.Add(FilterNodeList(oldNodes));
            return RemoveDuplicates(output);
        }

        protected abstract List<Node> FilterNodeList(List<Node> oldNodes);

        private static List<List<Node>> RemoveDuplicates(IReadOnlyList<List<Node>> output) {
            var cleanedNodeList = new List<List<Node>>();
            for (int i = 0; i < output.Count - 1; i++) {
                bool isUnique = true;
                for (int j = i + 1; j < output.Count; j++)
                    if (AreEqual(output[i], output[j]))
                        isUnique = false;
                if (isUnique)
                    cleanedNodeList.Add(output[i]);
            }

            cleanedNodeList.Add(output[output.Count - 1]);
            return cleanedNodeList;
        }

        private static bool AreEqual(IReadOnlyList<Node> a, IReadOnlyList<Node> b) {
            if (a.Count != b.Count) {
                return false;
            }

            for (var i = 0; i < a.Count; i++) {
                Node nodeA = a[i];
                Node nodeB = b[i];
                if (nodeA.GetElement() != nodeB.GetElement() || nodeA.IsEmpty() != nodeB.IsEmpty()) {
                    return false;
                }
            }

            return true;
        }
    }
}