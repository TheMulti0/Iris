using System;
using System.Collections.Generic;
using System.Text.Json;

namespace UpdatesProducer
{
    internal static class JsonElementExtensions
    {
        public static JsonElement? GetPropertyOrNull(
            this JsonElement element,
            string propertyName)
        {
            try
            {
                return element.GetProperty(propertyName);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public static int? GetIntOrNull(
            this JsonElement element)
        {
            try
            {
                return element.GetInt32();
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static double? GetDoubleOrNull(
            this JsonElement element)
        {
            try
            {
                return element.GetDouble();
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}