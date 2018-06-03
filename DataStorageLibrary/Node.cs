using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer.Element;

namespace DataStorageLibrary
{
    public class Node
    {
        private readonly List<Node> _nodesList = new List<Node>();
        private bool _empty;
        private readonly IElement _element;

        public Node(IElement element)
        {
            _element = element;
        }

        private Node(IElement element, List<Node> nodes)
        {
            _element = element;
            _nodesList = nodes;
        }

        public void ChangeEmptyStatus(bool status)
        {
            _empty = status;
        }

        public bool IsEmpty()
        {
            return _empty;
        }

        public IElement GetElement()
        {
            return _element;
        }

        public void AddNode(Node node)
        {
            _nodesList.Add(node);
        }

        public List<Node> GetNodesList()
        {
            return _nodesList;
        }

        public bool NodeExist(Node node) {
            foreach (var node1 in _nodesList)
                if (node == node1)
                    return true;
            return false;
        }

        public Node Clone()
        {
            return new Node(_element, _nodesList);
        }
    }
}
