using System;
using System.Web.Script.Serialization;

namespace Plugin.Compiler.Timer
{
	/// <summary>Сериализация</summary>
	internal static class Serializers
	{
		private static JavaScriptSerializer _serializer;

		private static JavaScriptSerializer Serializer//_serializer.RegisterConverters(new JavaScriptConverter[] { new TimeSpanJsonConverter(), new WorkHoursJsonConverter(), });
			=> _serializer ?? (_serializer = new JavaScriptSerializer());

		/// <summary>Десериализовать строку в объект</summary>
		/// <typeparam name="T">Тип объекта</typeparam>
		/// <param name="json">Строка в формате JSON</param>
		/// <returns>Десериализованный объект</returns>
		internal static T JavaScriptDeserialize<T>(String json)
			=> String.IsNullOrEmpty(json)
				? default
				: Serializers.Serializer.Deserialize<T>(json);

		/// <summary>Сериализовать объект</summary>
		/// <param name="item">Объект для сериализации</param>
		/// <returns>Строка в формате JSON</returns>
		internal static String JavaScriptSerialize(Object item)
			=> item == null
				? null
				: Serializers.Serializer.Serialize(item);
	}
}