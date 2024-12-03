using Newtonsoft.Json;

namespace AdventOfCode2024.Puzzles
{
    internal static class Extensions
    {
        public static T ToEnum<T>(this string value, T defaultValue)
        {
            T result = defaultValue;

            try { result = (T)Enum.Parse(typeof(T), value, true); }
            catch (Exception) { }

            return result;
        }
        public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }
        public static T? FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MaxDepth = null });
        }

        public static T Clone<T>(this T obj)
        {
            if (obj == null)
                return obj;
            else
                return obj.ToJson().FromJson<T>();
        }
        public static List<string> ToLines(this string rawData)
        {
            return [.. rawData.Replace("\r", "").Split("\n")];
        }
    }
}
