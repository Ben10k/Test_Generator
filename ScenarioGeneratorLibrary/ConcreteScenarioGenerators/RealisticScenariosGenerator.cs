using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;

namespace ScenarioGeneratorLibrary.ConcreteScenarioGenerators
{
    public class RealisticScenariosGenerator: ScenarioGenerator
    {
        public override List<List<Node>> GenerateScenraciosList(ViewData view)
        {
            var primaryScenarios = GeneratePrimaryActions(view);         
            var secondaryScenarios = GenerateSecondaryActions(view);
            var thidScenarios = GenerateThirdActions(view);
            var lastScenarios = GenerateLastActions(view);
            var resultScenarios = CombineScenarios(primaryScenarios, secondaryScenarios);
            resultScenarios = CombineScenarios(resultScenarios, thidScenarios);
            resultScenarios = CombineScenarios(resultScenarios, lastScenarios);
            return resultScenarios;
        }


        private List<List<Node>> GeneratePrimaryActions(ViewData view)
        {
            var elements = GetResetingElements(view.GetElements());
            var scenarios = GenerateScenariosWithEmptyElements(elements);
            return scenarios;
        }

        private List<List<Node>> GenerateSecondaryActions(ViewData view)
        {
            var elements = GetInputElements(view.GetElements());
            var scenarios = GenerateScenariosWithEmptyElements(elements);
            return scenarios;
        }

        private List<List<Node>> GenerateThirdActions(ViewData view)
        {
            var elements = GetSubmitingElements(view.GetElements());
            var scenarios = GenerateScenariosWithEmptyElements(elements);
            return scenarios;
        }


        private List<List<Node>> GenerateLastActions(ViewData view)
        {
            var elements = GetButtonTypeElements(view.GetElements());
            var scenarios = GenerateScenariosWithEmptyElements(elements);
            return scenarios;
        }

        private List<List<Node>> CombineScenarios(List<List<Node>> firstScenarios, List<List<Node>> secondScenarios)
        {
            var newScenarios = new List<List<Node>>();
            if (firstScenarios.Count == 0)
            {
                newScenarios = secondScenarios;
            }else if (secondScenarios.Count==0)
            {
                newScenarios = firstScenarios;
            }
            else
            {
                foreach (var firstScenario in firstScenarios)
                {                    
                    foreach (var secondScenario in secondScenarios)
                    {
                        var newScenario = new List<Node>();
                        newScenario.AddRange(firstScenario);
                        newScenario.AddRange(secondScenario);
                        newScenarios.Add(newScenario);
                    }
                }
            }

            return newScenarios;
        }
    }
}
