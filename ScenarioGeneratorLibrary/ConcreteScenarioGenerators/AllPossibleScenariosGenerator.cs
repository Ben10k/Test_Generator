using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;

namespace ScenarioGeneratorLibrary.ConcreteScenarioGenerators
{
    public class AllPossibleScenariosGenerator : ScenarioGenerator
    {
        public override List<List<Node>> GenerateScenraciosList(ViewData view)
        {
            var elements = view.GetElements();
            var nodes = GetNodes(elements);
            ConnectAllNodes(nodes);
            var scenarios = new List<List<Node>>();
            foreach (var node in nodes)
            {
                var paths = GetPaths(node, new List<Node>(), new List<List<Node>>());
                scenarios.AddRange(paths);
            }

            return scenarios;
        }
    }
}