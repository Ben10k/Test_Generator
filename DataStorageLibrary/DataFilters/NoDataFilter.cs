using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer;

namespace DataStorageLibrary.DataFilters
{
    public class NoDataFilter : DataFilter
    {
        public override List<ControllerData> FilerData(List<ControllerData> inputData)
        {
            return inputData;
        }
    }
}
