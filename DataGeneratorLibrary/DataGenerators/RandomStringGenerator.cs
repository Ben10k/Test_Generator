using System.Text.RegularExpressions;

namespace DataGeneratorLibrary.DataGenerators
{
    public class RandomStringGenerator: Generator
    {
        private const string RandomStringRegex = @"(\w|\d){5,30}";
        private const string ValidatorRefex = @".{5,30}";

        protected override bool IsValid(string data)
        {
            var result = Regex.Match(data, ValidatorRefex).ToString();
            var valid = !result.Equals("");
            return valid;
        }

        protected override string GenerateData()
        {
            return GenerateString(RandomStringRegex);
        }
    }
}
