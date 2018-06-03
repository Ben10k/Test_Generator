using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;

namespace ControllerViewDataParserLibrary.ControllerViewParser {
    public class ControllerViewReader {
        private readonly ApplicationData _applicationData = ApplicationData.Instance;

        public List<ControllerData> GetControllerViews(string directory) {
            return ListControllerViews(
                DirectoryFileReader.ReadControllerClasses(directory),
                DirectoryFileReader.ReadViewClasses(directory));
        }

        private List<ControllerData> ListControllerViews(List<string> controllers, List<string> views) {
            var list = new List<ControllerData>();
            foreach (string controller in controllers)
                list.Add(GetViewsInController(ParseControllerName(controller), views));
            return list;
        }

        private ControllerData GetViewsInController(string controller, List<string> views) {
            var listViewData = new List<ViewData>();

            for (var i = 0; i < views.Count; i++) {
                if (views[i].Contains('\\' + controller + '\\')) {
                    var viewData = new ViewData();
                    viewData.SetViewName(ParseViewName(views[i]));
                    _applicationData.GetViewDataGraph().AddVertex(controller + "/" + ParseViewName(views[i]));
                    views[i] = "";
                    listViewData.Add(viewData);
                }
            }

            return new ControllerData(controller, listViewData);
        }

        private string ParseControllerName(string path) {
            var index = path.LastIndexOf('\\');
            return path.Substring(index + 1, path.Length - index - 14);
        }

        private string ParseViewName(string path) {
            var index = path.LastIndexOf('\\');
            return path.Substring(index + 1, path.Length - index - 8);
        }
    }
}