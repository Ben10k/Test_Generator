using DataStorageLibrary.ViewsContainer.Element;

namespace DataStorageLibrary
{
    public class PreDefinedData
    {
        private string _data;
        private readonly IElement _ownerElement;

        public PreDefinedData(IElement ownerElement, string data)
        {
            _data = data;
            _ownerElement = ownerElement;
        }

        public void SetData(string data)
        {
            _data = data;
        }

        public IElement GetOwnerElement()
        {
            return _ownerElement;
        }

        public string GetData()
        {
            return _data;
        }
    }
}
