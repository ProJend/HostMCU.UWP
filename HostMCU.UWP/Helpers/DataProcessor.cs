using System.Text.RegularExpressions;

namespace HostMCU.UWP.Helpers
{
    public class DataProcessor
    {
        public string GetValueFromPattern(string data, string pattern)
        {
            Match match = Regex.Match(data, pattern);
            if (match.Success && match.Value != "00")
            {
                return match.Value;
            }
            return "NULL";
        }

        public string RemoveLineWithPattern(string data, string pattern)
        {
            var regex = new Regex(@"^(.*" + pattern + @".*)\r?\n?", RegexOptions.Multiline);
            data = regex.Replace(data, string.Empty);
            return data;
        }
    }
}
