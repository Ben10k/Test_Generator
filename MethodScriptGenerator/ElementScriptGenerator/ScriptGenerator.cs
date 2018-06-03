using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer.Element;

namespace MethodScriptGeneratorLibrary.ElementScriptGenerator {
    public abstract class ScriptGenerator{ 
        protected string ToUpperFirstLetter(string source) {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var letters = source.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);

            return new string(letters);
        }

        public abstract string Generate(IElement element, PreDefinedData preDefinedData);
    }
}