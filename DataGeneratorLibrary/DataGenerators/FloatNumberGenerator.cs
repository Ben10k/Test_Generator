using System.Text.RegularExpressions;

namespace DataGeneratorLibrary.DataGenerators
{
    public class FloatNumberGenerator: Generator
    {
        private const string PrefixNumberRegex = @"[0-9]{5,15}";
        private const string PostfixNumberRegex = @"[0-9]{3,5}";
        private const string ValidatorRefex = @"[0-9]+";

        protected override bool IsValid(string data)
        {
            var result = Regex.Match(data, ValidatorRefex).ToString();

            double value = 0;

            return !result.Equals("") && double.TryParse(result, out value);

        }

        protected override string GenerateData()
        {
            var prefixValue = GenerateString(PrefixNumberRegex);

            var postfixValue = GenerateString(PostfixNumberRegex);

            var result = prefixValue + "," + postfixValue;

            return result;
        }

    }
}
