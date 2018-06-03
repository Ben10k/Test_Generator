using DataGeneratorLibrary.DataGenerators;
using DataStorageLibrary.ViewsContainer.Element.Input;


namespace DataGeneratorLibrary
{
    public class DataGenerator
    {
        private readonly GeneratorFactory _generatorFactory = new GeneratorFactory();

        public string Generate(InputTypes dataType)
        {
            var generator = GetGenerator(dataType);

            return generator.Generate();
        }

        private IGenerator GetGenerator(InputTypes dataType)
        {
            return _generatorFactory.GetGenerator(dataType);
        }
    }
}
