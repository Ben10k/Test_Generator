using System.Collections.Generic;
using System.Linq;
using DataStorageLibrary;


namespace MethodScriptGeneratorLibrary {
    public class ScenarioScriptGenerator {
        private readonly MethodElementGenerator _methodElementGenerator = new MethodElementGenerator();

        public string GenerateScenarioScript(List<Node> scenario, PreDefinedDataContainer preDefinedDataContainer)
        {
            return (
                    from node in scenario
                    let element = node.GetElement()
                    let methodScript =
                        _methodElementGenerator.Generate(element, preDefinedDataContainer.GetPreDefinedData(element)) +
                        "\n"
                    where !node.IsEmpty()
                    select methodScript)
                .Aggregate("", (current, methodScript) => current + methodScript);
        }
    }
}