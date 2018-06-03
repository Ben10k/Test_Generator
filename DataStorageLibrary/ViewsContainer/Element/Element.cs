using System;

namespace DataStorageLibrary.ViewsContainer.Element {
    public class Element : IElement {
        private readonly string _id;
        private readonly string _name;
        private readonly string _tagName;
        private string _typeName;

        private Element(string id, string name, string tagName, string typeName) {
            _id = id;
            _name = name;
            _tagName = tagName;
            _typeName = typeName;
        }

        public Element(string id, string tagName, string typeName) {
            _id = id;
            _tagName = tagName;
            _typeName = typeName;
        }

        public Element(string id, string tagName) {
            _id = id;
            _tagName = tagName;
            _typeName = String.Empty;
        }


        public string GetTagName() {
            return _tagName;
        }

        public string GetTypeName() {
            return _typeName;
        }

        public void SetTypeName(string typeName) {
            _typeName = typeName;
        }

        public string GetId() {
            return _id;
        }

        public string GetName() {
            return _name;
        }

        public string GetValidIdentifier() {
            return !string.IsNullOrWhiteSpace(_id)
                ? _id
                : _name;
        }

        public IElement Clone() {
            //return new Element(_id, _name, _tagName, _typeName); everything works but not type change 
            return this;
        }
    }
}