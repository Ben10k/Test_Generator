using System.Collections.Generic;

namespace DataStorageLibrary.ScenariosContainer {
    public class ScenariosConainer {
        private List<Scenario> _scenarios = new List<Scenario>();

        public ScenariosConainer(List<Scenario> scenarios) {
            _scenarios = scenarios;
        }

        public ScenariosConainer() {
        }

        public void AddScenarios(List<Scenario> scenarios) {
            _scenarios = scenarios;
        }

        public void AddSenario(Scenario scenario) {
            _scenarios.Add(scenario);
        }

        public List<Scenario> GetScenarios() {
            return _scenarios;
        }

        public List<Scenario> GetScenarios(string controllerName, string viewName) {
            var scenarios = new List<Scenario>();
            foreach (var scenario in _scenarios) {
                if (scenario.GetControllerName().Equals(controllerName) && scenario.GetViewName().Equals(viewName))
                    scenarios.Add(scenario);
            }

            return scenarios;
        }

        public Scenario GetScenario(string controllerName, string viewName, string scenarioId) {
            foreach (var scenario in _scenarios) {
                if (scenario.GetControllerName().Equals(controllerName) && scenario.GetViewName().Equals(viewName) &&
                    scenario.GetScenarioId().Equals(scenarioId)) {
                    return scenario;
                }
            }

            return null;
        }

        public void RemoveFailedScenario(string controllerName, string viewName, string scenarioId) {
            foreach (var scenario in _scenarios) {
                if (scenario.GetControllerName().Equals(controllerName) && scenario.GetViewName().Equals(viewName) &&
                    scenario.GetScenarioId().Equals(scenarioId)) {
                    _scenarios.Remove(scenario);
                    break;
                }
            }
        }

        public List<string> GetControllerNames() {
            var controllerNames = new List<string>();

            foreach (var scenario in _scenarios) {
                if (!IsInList(scenario.GetControllerName(), controllerNames)) {
                    controllerNames.Add(scenario.GetControllerName());
                }
            }

            return controllerNames;
        }

        public List<string> GetViewNames(string controllerName) {
            var viewNames = new List<string>();

            foreach (var scenario in _scenarios) {
                if (controllerName.Equals(scenario.GetControllerName()) &&
                    !IsInList(scenario.GetViewName(), viewNames)) {
                    viewNames.Add(scenario.GetViewName());
                }
            }

            return viewNames;
        }

        private bool IsInList(string targetValue, List<string> targetList) {
            foreach (var value in targetList) {
                if (value.Equals(targetValue)) {
                    return true;
                }
            }

            return false;
        }

        public void ClearData()
        {
            _scenarios.Clear();
        }
    }
}