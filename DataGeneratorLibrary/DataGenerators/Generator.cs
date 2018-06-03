using Fare;

namespace DataGeneratorLibrary.DataGenerators {
    public abstract class Generator : IGenerator {
        public string Generate() {
            var data = string.Empty;

            while (!IsValid(data))

                data = GenerateData();

            return data;
        }

        protected static string GenerateString(string regexExpression) {
            var generator = new Xeger(regexExpression);

            return generator.Generate();
        }

        protected abstract bool IsValid(string data);

        protected abstract string GenerateData();
    }
}