namespace DataStorageLibrary.ViewsContainer.Element {
    public interface IElement {
        string GetId();
        string GetName();
        string GetTagName();
        string GetTypeName();
        void SetTypeName(string typeName);
        string GetValidIdentifier();
        IElement Clone();
    }
}