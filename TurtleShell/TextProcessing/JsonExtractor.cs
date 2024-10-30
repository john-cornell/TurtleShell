using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TurtleShell.TextProcessing
{
    public class JsonExtractor
    {
        public JsonExtractor()
        {

        }

        public string ExtractJSON(string text)
        {
            string json = StripJson(text);

            json = json.Replace("»", "\"");

            // Some models simply forget to add the closing bracket, so we add it here
            if (!json.TrimEnd().EndsWith("}"))
            {
                json += "}";
            }

            json = RemoveLineFeedsOutsideQuotes(json);

            return json;
        }



        private static string RemoveLineFeedsOutsideQuotes(string jsonString)
        {
            // This regex matches 0x0A (line feed) characters that are not inside quotes
            string pattern = @"(?<!\\)(?=(?:[^""]*""[^""]*"")*[^""]*$)\x0A";

            // Replace matched line feeds with an empty string
            return Regex.Replace(jsonString, pattern, string.Empty);
        }

        private static string StripJson(string text)
        {
            //Strip first instance of a complete JSON object from response, count open braces and closed braces

            StringBuilder builder = new();
            int openBraces = 0;
            int closedBraces = 0;
            bool found = false;

            foreach (char c in text)
            {
                if (c == '{')
                {
                    openBraces++;
                    found = true;
                }
                else if (c == '}')
                {
                    closedBraces++;
                }

                if (found)
                {
                    builder.Append(c);
                }

                if (openBraces > 0 && openBraces == closedBraces)
                {
                    break;
                }
            }

            return builder.ToString();
        }
    }
}
