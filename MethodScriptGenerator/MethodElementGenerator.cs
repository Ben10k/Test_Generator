using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer;
using DataStorageLibrary.ViewsContainer.Element;
using MethodScriptGeneratorLibrary.ElementScriptGenerator;

namespace MethodScriptGeneratorLibrary {
    public class MethodElementGenerator {
        public string Generate(IElement element, PreDefinedData preDefinedData) {
            var generator = GetScriptGenerator(element);

            return generator == null
                ? ""
                : generator.Generate(element, preDefinedData);
        }

        private ScriptGenerator GetScriptGenerator(IElement element) {
            return ElementValidator.IsClickable(element)
                ? (ScriptGenerator) new ClickScriptGenerator()
                : (ElementValidator.IsInputField(element)
                    ? new InputScriptGenerator()
                    : null);
        }
    }
}