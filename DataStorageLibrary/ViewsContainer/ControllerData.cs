using System.Collections.Generic;
using System.Linq;

namespace DataStorageLibrary.ViewsContainer
{
    public class ControllerData {
        private readonly string _controlerName;
        private readonly List<ViewData> _viewsList;

        public ControllerData(string controlerName, List<ViewData> viewList)
        {
            _controlerName = controlerName;
            _viewsList = viewList;
        }

        public ViewData GetView(string viewName) {
            return _viewsList
                .FirstOrDefault(view => viewName
                    .Equals(view.GetViewName()));
        }

        public string GetControllerName()
        {
            return _controlerName;
        }

        public List<ViewData> GetViews()
        {
            return _viewsList;
        }

        public ControllerData Clone()
        {
            var clonedList = new List<ViewData>();

            foreach(var view in _viewsList)
            {
                clonedList.Add(view.Clone());
            }

            return new ControllerData(_controlerName, clonedList);
        }
    }
}
