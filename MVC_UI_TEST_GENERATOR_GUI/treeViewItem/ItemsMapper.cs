using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataStorageLibrary.ScenariosContainer;
using DataStorageLibrary.ViewsContainer;

namespace MVC_UI_TEST_GENERATOR_GUI.treeViewItem {
    public static class ItemsMapper {
        public static ObservableCollection<Item> MapControllerItems(List<ControllerData> controllersList) {
            var resultCollection = new ObservableCollection<Item>();

            foreach (var controllerData in controllersList) {
                var item = new Item {
                    Title = controllerData.GetControllerName()
                };
                item.Path = item.Title + "/";
                resultCollection.Add(item);
                ControllerDataMapper(controllerData, item);
            }

            return resultCollection;
        }

        private static void ControllerDataMapper(ControllerData constrollerData, Item item) {
            foreach (var viewData in constrollerData.GetViews()) {
                var viewItem = new Item {
                    Title = viewData.GetViewName()
                };
                item.AddItem(viewItem);
                ViewDataMapper(viewData, viewItem);
            }
        }

        private static void ViewDataMapper(ViewData viewData, Item item) {
            foreach (var element in viewData.GetElements()) {
                var elementItem = new Item {
                    Title = element.GetValidIdentifier()
                };
                item.AddItem(elementItem);
            }
        }

        public static ObservableCollection<Item> MapItemsTreeToList(ObservableCollection<Item> items) {
            var newItemsCollection = new ObservableCollection<Item>();
            foreach (Item controllerItem in items) {
                foreach (Item viewItem in controllerItem.Items) {
                    foreach (Item elementItem in viewItem.Items) {
                        newItemsCollection.Add(new Item {
                            Title = elementItem.Path.Substring(0,elementItem.Path.Length-1),
                            Path = elementItem.Path,
                            Color = elementItem.Color
                        });
                    }
                }
            }

            return newItemsCollection;
        }

        public static ObservableCollection<Item> MapScenarioItems(ScenariosConainer scenarioContainer) {
            var resultCollection = new ObservableCollection<Item>();

            MapControllerItemsForScenario(scenarioContainer, resultCollection);

            MapViewItemsForScenario(scenarioContainer, resultCollection);

            foreach (var scenario in scenarioContainer.GetScenarios()) {
                if (!IsInList(scenario, resultCollection) && scenario.GetScenarioResult() != null) {
                    AddScenarioToCollection(scenario, resultCollection);
                }
            }

            RemoveUnfiledNodes(resultCollection);

            return resultCollection;
        }

        private static void MapControllerItemsForScenario(ScenariosConainer scenariosConainer,
            ObservableCollection<Item> scenariosCollection) {
            var controllerNames = scenariosConainer.GetControllerNames();
            foreach (var controllerName in controllerNames) {
                scenariosCollection.Add(new Item() {Title = controllerName, Path = controllerName + "/"});
            }
        }

        private static void MapViewItemsForScenario(ScenariosConainer scenariosConainer,
            ObservableCollection<Item> scenariosCollection) {
            foreach (var item in scenariosCollection) {
                foreach (var viewName in scenariosConainer.GetViewNames(item.Title)) {
                    item.AddItem(new Item() {Title = viewName});
                }
            }
        }

        private static void AddScenarioToCollection(Scenario scenario, ObservableCollection<Item> treeItems) {
            foreach (var controllerItem in treeItems) {
                if (controllerItem.Title.Equals(scenario.GetControllerName())) {
                    foreach (var viewItem in controllerItem.Items) {
                        if (viewItem.Title.Equals(scenario.GetViewName())) {
                            viewItem.AddItem(new Item() {
                                Title = GetTestStateIcon(scenario) + scenario.GetScenarioId(),

                                Color = GetColor(scenario)
                            });
                        }
                    }
                }
            }
        }

        private static string GetColor(Scenario scenario) {
            switch (scenario.GetScenarioResult()) {
                case "fail": return "Red";
                case "pass": return "#228B22";
                case "crashed": return "Orange";
                default: return "Blue";
            }
        }

        private static string GetTestStateIcon(Scenario scenario) {
            switch (scenario.GetScenarioResult()) {
                case "pass": return "✔";
                case "fail": return "❌";
                case "crashed": return "🚫";
                default: return "🔵";
            }
        }

        private static bool IsInList(Scenario targetScenario, ObservableCollection<Item> treeViewContainer) {
            foreach (var controllerItem in treeViewContainer) {
                if (controllerItem.Title.Equals(targetScenario.GetControllerName())) {
                    foreach (var viewItem in controllerItem.Items) {
                        if (viewItem.Title.Equals(targetScenario.GetViewName())) {
                            foreach (var scenarioItem in viewItem.Items) {
                                if (scenarioItem.Title.Equals(targetScenario.GetScenarioId())) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }


        private static void RemoveUnfiledNodes(ObservableCollection<Item> inputData) {
            List<Item> removeViewIndexes = new List<Item>();
            List<Item> removecontrollerIndexes = new List<Item>();


            foreach (var controler in inputData) {
                foreach (var view in controler.Items) {
                    if (view.Items.Count == 0) {
                        removeViewIndexes.Add(view);
                    }
                }

                foreach (var item in removeViewIndexes) {
                    controler.Items.Remove(item);
                }

                if (controler.Items.Count == 0) {
                    removecontrollerIndexes.Add(controler);
                }
            }

            foreach (var item in removecontrollerIndexes) {
                inputData.Remove(item);
            }
        }
    }
}