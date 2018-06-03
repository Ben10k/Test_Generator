using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer.Element;
using DataStorageLibrary.ViewsContainer.Element.Clickable;

namespace MethodScriptGeneratorLibrary.ElementScriptGenerator {
    public class ClickScriptGenerator : ScriptGenerator {
        public override string Generate(IElement element, PreDefinedData preDefinedData) {
            return GetClickableMethodScript(element.GetTypeName()) + ToUpperFirstLetter(element.GetValidIdentifier()) +
                   "();";
        }

        private string GetClickableMethodScript(string typeName) {
            switch (GetClickableType(typeName)) {
                case ClickableTypes.button: return "ClickButtonOn";
                case ClickableTypes.reset: return "ClickResetOn";
                case ClickableTypes.submit: return "SubmitFormOn";
                default: return "// clickable element type not found. ";
            }
        }

        private ClickableTypes GetClickableType(string typeName) {
            return (ClickableTypes) System.Enum.Parse(typeof(ClickableTypes), typeName.ToLower());
        }
    }
}