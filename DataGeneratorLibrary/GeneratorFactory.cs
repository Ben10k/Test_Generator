using DataGeneratorLibrary.DataGenerators;
using DataStorageLibrary.ViewsContainer.Element.Input;


namespace DataGeneratorLibrary {
    public class GeneratorFactory {
        private EmailGenerator _emailGenerator;
        private NumberGenerator _intNumberGenerator;
        private TextGenerator _lettersGenerator;
        private RandomStringGenerator _randomStringGenerator;
        private FloatNumberGenerator _floatNumberGenerator;

        public IGenerator GetGenerator(InputTypes dataType) {
            switch (dataType) {
                case InputTypes.email:
                    return GetEmailGenerator();
                case InputTypes.number:
                    return GetIntNumberGenerator();
                case InputTypes.text:
                    return GetLettersGenerator();
                case InputTypes.password:
                    return GetRandomStrinGenerator();
                default:
                    return GetRandomStrinGenerator();
            }
        }

        private IGenerator GetEmailGenerator() {
            return _emailGenerator ?? (_emailGenerator = new EmailGenerator());
        }

        private IGenerator GetIntNumberGenerator() {
            return _intNumberGenerator ?? (_intNumberGenerator = new NumberGenerator());
        }

        private IGenerator GetLettersGenerator() {
            return _lettersGenerator ?? (_lettersGenerator = new TextGenerator());
        }

        private IGenerator GetRandomStrinGenerator() {
            return _randomStringGenerator ?? (_randomStringGenerator = new RandomStringGenerator());
        }
    }
}