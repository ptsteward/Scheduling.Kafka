﻿using Newtonsoft.Json.Linq;
using System.Text;

namespace KafkaTesting.ksqlDB
{
    public class KsqlRowParser
    {
        public string ParseStreamRowToJson(string input, string[] headers)
        {
            var json = BuildJson(input, headers);
            return json;
        }

        private string BuildJson(string input, string[] headers)
        {
            var builder = new StringBuilder();
            var values = JArray.Parse(input);
            builder.AppendLine("{");

            for (int k = 0; k < values.Count; k++)
            {
                Parse_RawToken(values[k], headers[k], builder);

                if (k < values.Count - 1)
                    builder.AppendLine(",");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        private void Parse_RawToken(JToken token, string header, StringBuilder builder)
        {
            if (token.Type == JTokenType.String)
            {
                builder.Append($@"""{header}"":");
                Parse_RawStringValue(token.ToString(), builder);
            }
            else
            {
                builder.Append($@"""{header}"":{token}");
            }
        }

        private (string leftOver, int removed) Parse_RawStringValue(string input, StringBuilder builder)
        {
            if (input.StartsWith("["))
            {
                var arrayResult = Parse_Array(input);
                builder.AppendLine($"{arrayResult.json}");
                return (input.Remove(0, arrayResult.removed), arrayResult.removed);
            }
            else if (input.StartsWith("Struct"))
            {
                var structResult = Parse_Object(input);
                builder.Append($"{structResult.json}");
                return (input.Remove(0, structResult.removed), structResult.removed);
            }
            else if (input.StartsWith("{"))
            {
                var mapResult = Parse_Object(input);
                builder.Append($"{mapResult.json}");
                return (input.Remove(0, mapResult.removed), mapResult.removed);
            }
            else
            {
                var bareResult = Parse_BareValue(input);
                builder.Append($@"""{bareResult.json}""");
                return (input.Remove(0, bareResult.removed), bareResult.removed);
            }
        }

        private (string json, int removed) Parse_Array(string input)
        {
            var array = input.Substring(0, input.IndexOf("]") + 1);
            return (array, array.Length);
        }

        private (string json, int removed) Parse_Object(string input)
        {
            var builder = new StringBuilder();
            var removed = 0;
            var leftOver = input;
            var start = leftOver.IndexOf("{") + 1;
            builder.AppendLine("{");

            while (leftOver.Length > 1 && !leftOver.StartsWith("}"))
            {
                (leftOver, int remove) = Parse_Header(leftOver, start, builder);
                removed += remove;

                (leftOver, remove) = Parse_RawStringValue(leftOver, builder);
                removed += remove;

                (leftOver, remove) = AssignMoreToGo(leftOver, builder);
                removed += remove;

                start = 0;
            }

            builder.Append($"{Environment.NewLine}}}");

            if (leftOver.StartsWith("}"))
                removed++;

            return (builder.ToString(), removed);
        }

        private (string json, int removed) Parse_BareValue(string input)
        {
            Console.WriteLine($"BareValue:{input}");
            var valueLength = new[] { input.IndexOf(","), input.IndexOf(@""""), input.IndexOf("}"), input.Length }.Where(idx => idx > 0).Min();
            var value = input.Substring(0, valueLength);            
            return (value, value.Length);
        }        

        private (string leftOver, int removed) Parse_Header(string input, int start, StringBuilder builder)
        {
            var headerLength = input.IndexOf("=") - start;
            var header = input.Substring(start, headerLength).Trim();
            builder.Append($@"""{header}"":");
            var removeLength = input.IndexOf("=") + 1;
            return (input.Remove(0, removeLength), removeLength);
        }

        private (string leftOver, int removed) AssignMoreToGo(string input, StringBuilder builder)
        {
            if (input.StartsWith(","))
            {
                builder.AppendLine(",");
                return (input.Remove(0, 1), 1);
            }
            return (input, 0);
        }
    }
}
