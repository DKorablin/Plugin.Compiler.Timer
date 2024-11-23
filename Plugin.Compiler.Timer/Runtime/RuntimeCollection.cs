using System;
using System.Collections.Generic;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.Runtime
{
	/// <summary>Коллекция запущенных модулей</summary>
	internal class RuntimeCollection : IEnumerable<RuntimeItem>
	{
		private readonly Object _lock = new Object();
		private readonly List<RuntimeItem> _timerData = new List<RuntimeItem>();

		public Plugin Plugin { get; }

		/// <summary>Коллекция с запущенными таймерами</summary>
		/// <param name="plugin">Плагин</param>
		internal RuntimeCollection(Plugin plugin)
			=> this.Plugin = plugin;

		/// <summary>Запустить таймер из настроек</summary>
		/// <param name="settingsItem">Настройки из которых запустить таймер</param>
		public void Start(TimerCompilerSettingsItem settingsItem)
		{
			if(this.Plugin.Timers.PluginInstance == null)
			{
				Exception exc = new EntryPointNotFoundException("Timer plugin not found");
				exc.Data.Add("Name", this.Plugin.Timers.Name);
				throw exc;
			}
			if(this.Plugin.Compiler.PluginInstance == null)
			{
				Exception exc = new EntryPointNotFoundException("Compiler plugin not found");
				exc.Data.Add("Name", this.Plugin.Compiler.Name);
				throw exc;
			}

			lock(this._lock)
			{
				RuntimeItem startedItem = this.Find(settingsItem);
				if(startedItem != null)
					this.Stop(settingsItem);//Останавливаю таймер, если он уже был запущен

				RuntimeItem item = new RuntimeItem(this, settingsItem);
				this._timerData.Add(item);
			}
		}

		/// <summary>Поиск запущенного таймера через настройки</summary>
		/// <param name="settingsItem">Элемент настроек</param>
		/// <returns>Найденный элемент запущенного таймера</returns>
		public RuntimeItem Find(TimerCompilerSettingsItem settingsItem)
		{
			foreach(RuntimeItem item in this)
				if(item.TimerKey == settingsItem.TimerKey
					|| item.SettingsTimerKey == settingsItem.TimerKey)//Если ключ изменился, то ищем по запущенному ключу
					return item;

			return null;
		}

		/// <summary>Остановить и удалить таймер</summary>
		/// <param name="settingsItem">Настройки под которыми был запущен таймер</param>
		public void Stop(TimerCompilerSettingsItem settingsItem)
		{
			RuntimeItem item = this.Find(settingsItem)
				?? throw new ArgumentException(nameof(settingsItem), "Settings item to stop is not found");

			lock(this._lock)
			{
				item.Stop();
				this._timerData.Remove(item);
			}
		}

		/// <summary>Оставносить все запущенные таймеры</summary>
		public void StopAll()
		{
			lock(this._lock)
				while(this._timerData.Count > 0)
				{
					RuntimeItem item = this._timerData[0];
					item.Stop();
					this._timerData.RemoveAt(0);
				}
		}

		public IEnumerator<RuntimeItem> GetEnumerator()
		{
			foreach(var item in this._timerData)
				yield return item;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}