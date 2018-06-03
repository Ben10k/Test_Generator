using System.Text.RegularExpressions;

namespace DataGeneratorLibrary.DataGenerators
{
    public class TextGenerator: Generator
    {
        private const string LettersRegex = @"[a-zA-Z]{10,20}";
        private const string ValidatorRefex = @"[a-zA-Z]+";

        protected override bool IsValid(string data)
        {
            var result = Regex.Match(data, ValidatorRefex).ToString();
            var valid = !result.Equals("");
            return valid;
        }

        protected override string GenerateData()
        {
            return GenerateString(LettersRegex);
        }
    }
}
