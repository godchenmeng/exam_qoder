using System;
using System.Text.Json;

namespace ExamSystem.Infrastructure.Utils
{
    /// <summary>
    /// JSON序列化工具类
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// 序列化对象为JSON字符串
        /// </summary>
        public static string Serialize<T>(T obj)
        {
            if (obj == null)
                return null;

            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        /// <summary>
        /// 反序列化JSON字符串为对象
        /// </summary>
        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json, DefaultOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        /// <summary>
        /// 尝试反序列化
        /// </summary>
        public static bool TryDeserialize<T>(string json, out T result)
        {
            result = default;

            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
