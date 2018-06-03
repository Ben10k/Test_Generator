using System.Collections.Generic;

namespace DataStorageLibrary.ScenariosContainer {
    public class Scenario {
        private readonly string _scenarioId;
        private readonly List<Node> _actionElements;
        private readonly string _controllerName;
        private readonly string _viewName;
        private string _scenarioState;

        public Scenario(string scenarioId, string controllerName, string viewName, List<Node> actionElements) {
            _scenarioId = scenarioId;
            _controllerName = controllerName;
            _viewName = viewName;
            _actionElements = actionElements;
            _scenarioState = "unknown";
        }

        public List<Node> GetScenarioNodes() {
            return _actionElements;
        }

        public string GetScenarioId() {
            return _scenarioId;
        }

        public string GetControllerName() {
            return _controllerName;
        }

        public string GetViewName() {
            return _viewName;
        }

        public string GetScenarioResult() {
            return _scenarioState;
        }

        public void SetScenarioResult(string scenarioState) {
            _scenarioState = scenarioState;
        }
    }
}