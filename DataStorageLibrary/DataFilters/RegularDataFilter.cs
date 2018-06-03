using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer;
using DataStorageLibrary.ViewsContainer.Element;

namespace DataStorageLibrary.DataFilters
{
    public class RegularDataFilter : DataFilter
    {
        public override List<ControllerData> FilerData(List<ControllerData> inputData)
        {
            var resultList = Clone(inputData);

            var removableElements = new List<IElement>();

            var validTags = ElementValidator.GetAllValidTags();
            var validTypes = ElementValidator.GetAllValidTypes();

            foreach (var controller in resultList)
            {
                foreach (var view in controller.GetViews())
                {
                    foreach (var viewElement in view.GetElements())
                    {
                        if (!ElementValidator.IsInList(viewElement.GetTypeName(), ElementValidator.GetAllValidTypes()) 
                            && !ElementValidator.IsInList(viewElement.GetTagName(), ElementValidator.GetAllValidTags()))
                        {
                            removableElements.Add(viewElement);
                        }
                    }
                    foreach(var removableElement in removableElements)
                    {
                        view.RemoveElement(removableElement);
                    }
                }
            }
            return resultList;
        }


    }
}
