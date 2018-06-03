using System.Collections.Generic;
using System.Linq;
using DataStorageLibrary.ViewsContainer.Element;

namespace DataStorageLibrary.ViewsContainer {
    public class ViewData {
        private string _viewName;
        private readonly List<IElement> _elements = new List<IElement>();

        private ViewData(string viewName, List<IElement> viewElements) {
            _viewName = viewName;
            _elements = viewElements;
        }

        public ViewData() {
        }

        public void AddElement(IElement element) {
            _elements.Add(element);
        }

        public void SetViewName(string name) {
            _viewName = name;
        }

        public string GetViewName() {
            return _viewName;
        }

        public IElement GetElementByValidName(string validName) {
            return _elements.
                FirstOrDefault(element => element.
                    GetValidIdentifier()
                    .Equals(validName)
                );
        }

        internal ViewData Clone() {
            var resulElementsList = new List<IElement>();

            foreach (var viewElement in _elements) {
                resulElementsList.Add(viewElement.Clone());
            }

            return new ViewData(_viewName, resulElementsList);
        }

        public List<IElement> GetElements() {
            return _elements;
        }


        public void RemoveElement(IElement removableElement) {
            foreach (var element in _elements) {
                if (element == removableElement) {
                    _elements.Remove(element);
                    break;
                }
            }
        }
    }
}