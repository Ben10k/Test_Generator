using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer.Element;

namespace DataStorageLibrary {
    public class PreDefinedDataContainer {
        private readonly List<PreDefinedData> _preDefinedDatas = new List<PreDefinedData>();

        public PreDefinedDataContainer(List<PreDefinedData> preDefinedDatas) {
            _preDefinedDatas = preDefinedDatas;
        }

        public PreDefinedDataContainer() {
        }

        public void RemovePredefinedDataOfElement(IElement element) {
            for (int i = 0; i < _preDefinedDatas.Count; i++) {
                if (_preDefinedDatas[i].GetOwnerElement() == element) {
                    _preDefinedDatas.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddPredefinedData(PreDefinedData preDefinedData) {
            if (!ElementExists(preDefinedData.GetOwnerElement())) {
                _preDefinedDatas.Add(preDefinedData);
            }
            else {
                var existingItem = GetPreDefinedData(preDefinedData.GetOwnerElement());
                existingItem.SetData(preDefinedData.GetData());
            }
        }

        public PreDefinedData GetPreDefinedData(IElement element) {
            foreach (var preDefinedData in _preDefinedDatas) {
                if (element == preDefinedData.GetOwnerElement()) {
                    return preDefinedData;
                }
            }

            return null;
        }


        public PreDefinedData GetPreDefinedData(string elementValidName) {
            foreach (var preDefinedData in _preDefinedDatas) {
                if (elementValidName.Equals(preDefinedData.GetOwnerElement().GetValidIdentifier())) {
                    return preDefinedData;
                }
            }

            return null;
        }

        private bool ElementExists(IElement element) {
            foreach (var preDefinedData in _preDefinedDatas) {
                if (preDefinedData.GetOwnerElement() == element) {
                    return true;
                }
            }

            return false;
        }

        public void ClearData()
        {
            _preDefinedDatas.Clear();
        }
    }
}