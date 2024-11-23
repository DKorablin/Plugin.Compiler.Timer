using System;
using System.Collections.Generic;

namespace Plugin.CompilerTimer.Settings
{
	/// <summary>Хранилище настроек таймеров</summary>
	internal class TimerCompilerSettingsCollection : IEnumerable<TimerCompilerSettingsItem>
	{
		/// <summary>Словарь хранилища настроек таймеров</summary>
		private readonly List<TimerCompilerSettingsItem> _timerData = new List<TimerCompilerSettingsItem>();
		private const String ConstMethodName = "CompilerTimerMethod";
		private const String ConstTimerName = "CompilerTimer";

		internal Plugin Plugin { get; }

		/// <summary>Получить маппинг по индексу</summary>
		/// <param name="index">Индекс по которому получить маппинг</param>
		/// <returns>Маппинг по индексу или null</returns>
		public TimerCompilerSettingsItem this[Int32 index]
		{
			get => index < 0 || index >= this._timerData.Count
				? null
				: this._timerData[index];
			set => this._timerData[index] = value;
		}

		/// <summary>Создание нового маппинга</summary>
		/// <returns>Созданный новый маппинг</returns>
		public TimerCompilerSettingsItem NewMapping()
		{
			TimerCompilerSettingsItem result = new TimerCompilerSettingsItem() { TimerName = TimerCompilerSettingsCollection.ConstTimerName, };

			result.SetOwner(this);
			return result;
		}

		/// <summary>Получить уникальное наименование метода для нового таймера</summary>
		/// <returns>Уникальное наименование метода</returns>
		public String GetUniqueMethodName()
		{
			String methodName = TimerCompilerSettingsCollection.ConstMethodName;
			UInt32 count = 1;
			while(this._timerData.Exists(p => p.MethodName == methodName))
				methodName = String.Join("_", new String[] { TimerCompilerSettingsCollection.ConstMethodName, (count++).ToString(), });

			return methodName;
		}

		/// <summary>Создание экземпляра коллекции маппингов таймеров с компилятором</summary>
		/// <param name="plugin">Плагин</param>
		/// <param name="json">JSON из которого создать коллекцию</param>
		public TimerCompilerSettingsCollection(Plugin plugin, String json)
		{
			this.Plugin = plugin;

			if(json != null)
				foreach(TimerCompilerSettingsItem item in Serializers.JavaScriptDeserialize<TimerCompilerSettingsItem[]>(json))
				{
					item.SetOwner(this);
					this._timerData.Add(item);
				}
		}

		/// <summary>Добавить или изменить элемент в списке</summary>
		/// <param name="item">Элемент для добавления</param>
		public void AddOrUpdate(TimerCompilerSettingsItem item)
		{
			Int32 index = this._timerData.IndexOf(item);
			if(index > -1)
				this._timerData[index] = item;
			else
				this._timerData.Add(item);
		}

		/// <summary>Удалить маппинг таймера к методу, с функцией удаления метода из компилятора</summary>
		/// <param name="item">Элемент маппинга таймера с компилятором</param>
		/// <returns>Маппинг удалён успешно или маппинг не найден</returns>
		public Boolean Remove(TimerCompilerSettingsItem item)
		{
			Int32 index = this._timerData.IndexOf(item);
			if(index > -1)
			{
				if(this.GetMethodUsageCount(item) == 1)
					this.Plugin.Compiler.DeleteMethod(item.MethodName);

				this._timerData.RemoveAt(index);
				return true;
			}
			return false;
		}

		/// <summary>Получить кол-во использования методов</summary>
		/// <param name="settings">Элемент маппинга для которого получить кол-во использования метода</param>
		/// <returns>Кол-во использования метода другими маппингами</returns>
		public Int32 GetMethodUsageCount(TimerCompilerSettingsItem settings)
		{
			Int32 count = 0;
			if(settings.MethodName != null)
				foreach(TimerCompilerSettingsItem item in this)
					if(item.MethodName == settings.MethodName)
						count++;
			return count;
		}

		/// <summary>Сконвертировать маппинги в JSON</summary>
		/// <returns>Строковое представление объекта</returns>
		public String ToJson()
		{
			if(this._timerData != null && this._timerData.Count > 0)
			{
				TimerCompilerSettingsItem[] data = new TimerCompilerSettingsItem[this._timerData.Count];
				Int32 loop = 0;
				foreach(TimerCompilerSettingsItem item in this)
					data[loop++] = item;
				return Serializers.JavaScriptSerialize(data);
			} else return null;
		}

		public IEnumerator<TimerCompilerSettingsItem> GetEnumerator()
		{
			foreach(TimerCompilerSettingsItem item in this._timerData)
				yield return item;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}