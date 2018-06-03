using System.Collections.Generic;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;


namespace ScenarioGeneratorLibrary
{
    public interface IScenarioGenerator
    {
        List<List<Node>> GenerateScenraciosList(ViewData view);
    }
}