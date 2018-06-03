using DataGeneratorLibrary;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer.Element;
using DataStorageLibrary.ViewsContainer.Element.Input;

namespace MethodScriptGeneratorLibrary.ElementScriptGenerator {
    public class InputScriptGenerator : ScriptGenerator {
        private readonly DataGenerator _dataGenerator = new DataGenerator();

        public override string Generate(IElement element, PreDefinedData preDefinedData) {
            var inputType = GetInputType(element.GetTypeName());
            var inputData = preDefinedData?.GetOwnerElement() != null
                ? preDefinedData.GetData()
                : _dataGenerator.Generate(inputType);


            return GetMethodName(element.GetValidIdentifier()) + "(\"" + inputData + "\");";
        }

        private string GetMethodName(string ElementName) {
            return "EnterTextTo" + ToUpperFirstLetter(ElementName);
        }

        private InputTypes GetInputType(string typeName) {
            return (InputTypes) System.Enum.Parse(typeof(InputTypes), typeName.ToLower());
        }
    }
}