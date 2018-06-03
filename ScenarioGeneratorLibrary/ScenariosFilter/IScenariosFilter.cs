using System.Collections.Generic;
using DataStorageLibrary;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public interface IScenariosFilter {
        List<List<Node>> Filter(List<List<Node>> input);
    }
}