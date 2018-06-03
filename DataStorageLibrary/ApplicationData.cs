using System.Collections.Generic;
using System.Linq;
using DataStorageLibrary.DataFilters;
using DataStorageLibrary.ScenariosContainer;
using DataStorageLibrary.ViewsContainer;

namespace DataStorageLibrary {
    public sealed class ApplicationData {
        private static volatile ApplicationData _instance;
        private static readonly object SyncRoot = new object();

        private List<ControllerData> _listControllerData = new List<ControllerData>();
        private readonly ViewDataGraph _viewDataGraph = new ViewDataGraph();
        private ScenariosConainer _scenariosContainer = new ScenariosConainer();
        private readonly IDataFilter _dataFilter = new RegularDataFilter();

        private ApplicationData() {
        }

        public static ApplicationData Instance {
            get {
                if (_instance == null) {
                    lock (SyncRoot) {
                        if (_instance == null)
                            _instance = new ApplicationData();
                    }
                }

                return _instance;
            }
        }

        public ScenariosConainer GetScenariosContainer()
        {
            return _scenariosContainer;
        }

        public List<ControllerData> GetControllerData() {
            var filteredData = _dataFilter.FilerData(_listControllerData);
            return filteredData;
        }

        public ControllerData GetControllerByName(string name) {
            return _listControllerData.FirstOrDefault(i => i.GetControllerName() == name);
        }

        public List<string> GetAllViewNames() {
            var allViews = new List<string>();
            foreach (var controllerData in _listControllerData) {
                foreach (var viewData in controllerData.GetViews()) {
                    allViews.Add(controllerData.GetControllerName() + "/" + viewData.GetViewName());
                }
            }
            return allViews;
        }

        private readonly List<string> _allActionNames = new List<string>();

        public void AddAction(string actionName) {
            _allActionNames.Add(actionName);
        }

        public List<string> GetAllActionNames() {
            return _allActionNames;
        }

        public ViewDataGraph GetViewDataGraph() {
            return _viewDataGraph;
        }

        public void SetControllers(List<ControllerData> controllers) {
            _listControllerData = controllers;
        }

        public void SetScenarioContainer(ScenariosConainer scenariosContainer)
        {
            _scenariosContainer = scenariosContainer;
        }

        public void ClearControllersData()
        {
            _listControllerData.Clear();
        }

        public void ClearScenariosData()
        {
            _scenariosContainer.ClearData();
        }

    }
}