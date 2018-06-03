using System.Text.RegularExpressions;

namespace DataGeneratorLibrary.DataGenerators {
    public class EmailGenerator : Generator {
        private const string PrefixRegexExpresion = @"^[a-zA-Z0-9][a-zA-Z0-9\.]{2,15}";
        private const string DomainRegexExpression = @"(gmail|hotmail)\.(com|lt|de|pl|ru)";
        private const string ValidationExpression = @".+@.+\..+";

        protected override bool IsValid(string data) {
            var result = Regex.Match(data, ValidationExpression).ToString();
            return !result.Equals("");
        }

        protected override string GenerateData() {
            return GeneratePrefix() + "@" + GenerateDomain();
        }

        private string GeneratePrefix() {
            return GenerateString(PrefixRegexExpresion);
        }

        private string GenerateDomain() {
            return GenerateString(DomainRegexExpression);
        }
    }
}