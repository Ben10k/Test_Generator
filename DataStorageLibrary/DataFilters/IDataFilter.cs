using System.Collections.Generic;
using DataStorageLibrary.ViewsContainer;

namespace DataStorageLibrary.DataFilters
{
    public interface IDataFilter
    {
        List<ControllerData> FilerData(List<ControllerData> inputData);
    }
}
