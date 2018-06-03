using System.Collections.Generic;
using System.IO;
using DataStorageLibrary;

namespace ControllerViewDataParserLibrary.FileParser {
    public class ControllerFileReader {
        private readonly string _projectRootDir;
        private readonly string _controller;
        private readonly ApplicationData _applicationData = ApplicationData.Instance;

        public ControllerFileReader(string projectRootDir, string controller) {
            _projectRootDir = projectRootDir;
            _controller = controller;
        }

        public void AddEdges() {
            foreach (var block in GetActionResultBlocks()) {
                int startOfViewName = block[0].IndexOf("public ActionResult ") + 20;
                int lengthOfViewName = block[0].IndexOf('(', startOfViewName) - startOfViewName;
                string viewName = block[0].Substring(startOfViewName, lengthOfViewName);
                if (viewName.Contains("<IActionResult> ")) {
                    viewName = viewName.Substring(viewName.LastIndexOf(" ") + 1);
                }

                string currentNode = _controller + '/' + viewName;
                _applicationData.AddAction(currentNode);
                if (!_applicationData.GetViewDataGraph().ContainsVertex(currentNode)) {
                    _applicationData.GetViewDataGraph().AddVertex(currentNode);
                }

                block.Remove(block[0]);
                foreach (string line in block) {
                    string temp = line;
                    if (temp.Contains("RedirectToContract(")) {
                        temp = temp.Replace("RedirectToContract(", "RedirectToAction(\"Edit\",");
                    }

                    if (temp.Contains("RedirectToAction")) {
                        int startOfRedirectedViewName = temp.IndexOf("RedirectToAction(\"") + 18;

                        if (startOfRedirectedViewName == 17) {
                            startOfRedirectedViewName = temp.IndexOf("RedirectToAction(nameof(") + 24;
                            int lengthOfRedirectedViewName =
                                temp.IndexOf(')', startOfRedirectedViewName) - startOfRedirectedViewName;

                            string redirectedViewName =
                                temp.Substring(startOfRedirectedViewName, lengthOfRedirectedViewName);


                            string from = currentNode;
                            string to = redirectedViewName.Contains("Controller.")
                                ? redirectedViewName.Replace("Controller.", "/")
                                : _controller + '/' + redirectedViewName;
                            AddEdge(from, to);
                        }
                        else {
                            int lengthOfRedirectedViewName =
                                temp.IndexOf('\"', startOfRedirectedViewName) - startOfRedirectedViewName;


                            string redirectedViewName =
                                temp.Substring(startOfRedirectedViewName, lengthOfRedirectedViewName);

                            string to;
                            if (line.Contains(redirectedViewName + '"' + ',')) {
                                int startOfRedirectedControllerName =
                                    temp.IndexOf('"',
                                        temp.IndexOf(redirectedViewName + '"' + ',') + redirectedViewName.Length + 1) +
                                    1;
                                int lengthOfRedirectedConreollerName =
                                    temp.IndexOf('"', startOfRedirectedControllerName + 1) -
                                    startOfRedirectedControllerName;
                                string toController = temp.Substring(startOfRedirectedControllerName,
                                    lengthOfRedirectedConreollerName);
                                to = toController + '/' + redirectedViewName;
                            }
                            else {
                                to = _controller + '/' + redirectedViewName;
                            }

                            AddEdge(currentNode, to);
                        }
                    }
                    else if (temp.Contains("RedirectToIndex(") || temp.Contains("Delete(") ||
                             temp.Contains("DeleteChild(")) {
                        string to = _controller + '/' + "Index";

                        AddEdge(currentNode, to);
                    }
                    else if (temp.Contains("RedirectToLocal(\"/\"")) {
                        string from = currentNode;
                        int startIndexOfDestionation = temp.IndexOf("RedirectToLocal(\"/\"") + 19;
                        int lengthOfDestination = temp.IndexOf("\"", startIndexOfDestionation);
                        string destination = lengthOfDestination == -1
                            ? "Index"
                            : temp.Substring(startIndexOfDestionation, lengthOfDestination);
                        string to = _controller + '/' + destination;

                        AddEdge(from, to);
                    }
                }
            }
        }

        private List<List<string>> GetActionResultBlocks() {
            var blocks = new List<List<string>>();
            var controller = ReadFile();
            for (int i = 0; i < controller.Length; i++)
                if (controller[i].Contains("public ActionResult ") ||
                    controller[i].Contains("async Task<IActionResult>")) {
                    var block = new List<string>();
                    while (true) {
                        block.Add(controller[i++]);
                        if (i == controller.Length - 1
                            || controller[i + 1].Contains("public ")
                            || controller[i + 1].Contains("protected ")
                            || controller[i + 1].Contains("private "))
                            break;
                    }

                    blocks.Add(block);
                }

            return blocks;
        }

        private void AddEdge(string from, string to) {
            if (!_applicationData.GetViewDataGraph().ContainsVertex(to)) {
                _applicationData.GetViewDataGraph().AddVertex(to);
            }

            _applicationData.GetViewDataGraph().AddEdge(from, to);
        }

        private string[] ReadFile() {
            return File.ReadAllLines(_projectRootDir + "Controllers\\" + _controller + "Controller.cs");
        }
    }
}