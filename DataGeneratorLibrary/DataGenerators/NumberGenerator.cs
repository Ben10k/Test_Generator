using System.Text.RegularExpressions;

namespace DataGeneratorLibrary.DataGenerators
{
    public class NumberGenerator: Generator
    {
        private const string IntNumberRegex = @"[0-9]{2,15}";
        private const string ValidatorRegex = @"[0-9]+";

        protected override bool IsValid(string data)
        {
            var result = Regex.Match(data, ValidatorRegex).ToString();

            long intValue = 0;

            return !result.Equals("") && long.TryParse(result, out intValue);
        }

        protected override string GenerateData()
        {
            return GenerateString(IntNumberRegex);
        }
    }
}
