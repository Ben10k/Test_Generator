using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;
using DataStorageLibrary.ViewsContainer.Element;

namespace ScenarioGeneratorLibrary
{
    public abstract class ScenarioGenerator: IScenarioGenerator
    {

        public abstract List<List<Node>> GenerateScenraciosList(ViewData view);


        protected List<List<Node>> GenerateScenariosWithEmptyElements(List<IElement> elements)
        {
            var nodes = GetNodes(elements);
            ConnectAllNodes(nodes);
            var scenarios = new List<List<Node>>();
            foreach (var node in nodes)
            {
                var paths = GetPaths(node, new List<Node>(), new List<List<Node>>());
                scenarios.AddRange(paths);
            }

            return scenarios;
        }

        public List<Node> GetNodes(List<IElement> elements)
        {
            var nodes = new List<Node>();

            foreach (var element in elements)
            {
                nodes.Add(new Node(element));
            }

            return nodes;
        }


        public void ConnectAllNodes(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var node1 in nodes)
                {
                    ConnectNodes(node, node1);
                    ConnectNodes(node1, node);
                }
            }
        }

        private void ConnectNodes(Node parrentNode, Node childNode)
        {
            if (parrentNode != childNode && !parrentNode.NodeExist(childNode))
            {
                parrentNode.AddNode(childNode);
            }
        }

        protected List<List<Node>> GetPaths(Node entry, List<Node> path, List<List<Node>> paths)
        {
            var newPath = CloneList(path);
            newPath.Add(entry);

            if (NodeHasElements(newPath, entry))
            {
                paths.Add(newPath);
            }
            else
            {
                foreach (var node in entry.GetNodesList())
                {
                    if (!IsInList(newPath, node))
                    {
                        GetPaths(node, newPath, paths);
                        var emptyNode = node.Clone();
                        emptyNode.ChangeEmptyStatus(true);
                        GetPaths(emptyNode, newPath, paths);
                    }
                }
            }

            return paths;
        }


        private bool NodeHasElements(List<Node> elements, Node node)
        {
            foreach (var localNode in node.GetNodesList())
            {
                if (!IsInList(elements, localNode))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInList(List<Node> list, Node element)
        {
            foreach (var listElement in list)
            {
                if (listElement.GetElement() == element.GetElement())
                {
                    return true;
                }
            }

            return false;
        }

        private List<Node> CloneList(List<Node> list)
        {
            var newList = new List<Node>();
            foreach (var element in list)
            {
                newList.Add(element);
            }
            return newList;
        }

        public List<IElement> GetInputElements(List<IElement> elements)
        {
            var resultList = new List<IElement>();
            foreach (var element in elements)
            {
                if (ElementValidator.IsInputField(element))
                {
                    resultList.Add(element);
                }
            }

            return resultList;
        }

        private List<IElement> GetClickableElements(List<IElement> elements)
        {
            var resultList = new List<IElement>();
            foreach (var element in elements)
            {
                if (ElementValidator.IsClickable(element))
                {
                    resultList.Add(element);
                }
            }

            return resultList;
        }

        public List<IElement> GetResetingElements(List<IElement> elements)
        {
            var resetingElements = new List<IElement>();

            const string resetType = "reset";
            foreach (var clickableElement in GetClickableElements(elements))
            {
                var elementTypeName = clickableElement.GetTypeName();
                if (IsEqualString(resetType, elementTypeName))
                {
                    resetingElements.Add(clickableElement);
                }
            }

            return resetingElements;
        }

        public List<IElement> GetSubmitingElements(List<IElement> elements)
        {
            var rsubmitingElements = new List<IElement>();

            foreach (var clickableElement in GetClickableElements(elements))
            {
                var elementTypeName = clickableElement.GetTypeName();
                var resetType = "submit";
                if (IsEqualString(resetType, elementTypeName))
                {
                    rsubmitingElements.Add(clickableElement);
                }
            }

            return rsubmitingElements;
        }

        public List<IElement> GetButtonTypeElements(List<IElement> elements)
        {
            var buttonTypeElements = new List<IElement>();

            foreach (var clickableElement in GetClickableElements(elements))
            {
                var elementTypeName = clickableElement.GetTypeName();
                var resetType = "button";
                if (IsEqualString(resetType, elementTypeName))
                {
                        buttonTypeElements.Add(clickableElement);
                }
            }

            return buttonTypeElements;
        }

        private bool IsEqualString(string first, string second)
        {
            return (first.ToLower().Equals(second.ToLower()));
        }
    }
}
