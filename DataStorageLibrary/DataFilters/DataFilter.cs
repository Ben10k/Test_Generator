using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer;

namespace DataStorageLibrary.DataFilters
{
    public abstract class DataFilter : IDataFilter
    {
        public abstract List<ControllerData> FilerData(List<ControllerData> inputData);

        protected List<ControllerData> Clone(List<ControllerData> data)
        {
            var resultList = new List<ControllerData>();

            foreach (var controller in data)
            {
                resultList.Add(controller.Clone());
            }
            return resultList;
        }

    }
}
