#if NETFRAMEWORK
using System.Web.Script.Serialization;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif
using System;

namespace Plugin.Compiler.Timer
{
	/// <summary>Serialization</summary>
	internal static class Serializers
	{
#if NETFRAMEWORK
		private static JavaScriptSerializer _serializer;

		private static JavaScriptSerializer Serializer
			=> _serializer ?? (_serializer = new JavaScriptSerializer());
#else
		private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
			PropertyNameCaseInsensitive = true,
			WriteIndented = false,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
#endif

		/// <summary>Deserialize a string into an object</summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="json">String in JSON format</param>
		/// <returns>Deserialized object</returns>
		internal static T JavaScriptDeserialize<T>(String json)
		{
			if(String.IsNullOrEmpty(json))
				return default;

#if NETFRAMEWORK
			return Serializer.Deserialize<T>(json);
#else
			return JsonSerializer.Deserialize<T>(json, _serializerOptions);
#endif
		}

		/// <summary>Serialize object</summary>
		/// <param name="item">Object to serialize</param>
		/// <returns>String in JSON format</returns>
		internal static String JavaScriptSerialize(Object item)
		{
			if(item == null)
				return null;

#if NETFRAMEWORK
			return Serializer.Serialize(item);
#else
			return JsonSerializer.Serialize(item, _serializerOptions);
#endif
		}
	}
}
