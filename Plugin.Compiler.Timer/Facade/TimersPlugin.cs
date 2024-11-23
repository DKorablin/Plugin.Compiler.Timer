using System;
using SAL.Flatbed;

namespace Plugin.Compiler.Timer.Facade
{
	/// <summary>Обёртка над плагином с таймерами</summary>
	internal class TimersPlugin
	{
		private readonly Plugin _plugin;
		private IPluginDescription _pluginTimer;

		/// <summary>Плагин таймера</summary>
		private static class PluginConstants
		{
			/// <summary>ID плагина с таймерами</summary>
			public const String Name = "69c79417-c168-434a-a597-4e224237a527";

			/// <summary>Публичные методы плагина</summary>
			public static class Methods
			{
				/// <summary>Зарегистрировать таймер</summary>
				public const String RegisterTimer = "RegisterTimer";

				/// <summary>Удалить регистрацию таймера</summary>
				public const String UnregisterTimer = "UnregisterTimer";

				/// <summary>Выполнить таймер по ключу</summary>
				public const String InvokeTimer = "InvokeTimer";

				/// <summary>Получить список созданных таймеров</summary>
				public const String GetTimersName = "GetTimersName";

				/// <summary>Таймер с таким наименованием - существует</summary>
				public const String IsTimerExists = "IsTimerExists";
			}
		}

		/// <summary>Ссылка на описание собственного плагина</summary>
		private IPluginDescription PluginSelf
		{
			get
			{
				foreach(IPluginDescription plugin in this._plugin.Host.Plugins)
					if(plugin.Instance == this._plugin)
						return plugin;
				throw new InvalidOperationException();
			}
		}

		/// <summary>Ссылка на описание плагина компиляции</summary>
		public IPluginDescription PluginInstance => this._pluginTimer ?? (this._pluginTimer = this._plugin.Host.Plugins[this.Name]);

		/// <summary>Наименование плагина с таймерами</summary>
		public String Name => PluginConstants.Name;

		/// <summary>Создание обёртки плагина таймера</summary>
		/// <param name="plugin">Инстанс текущего плагина</param>
		public TimersPlugin(Plugin plugin)
			=> this._plugin = plugin;

		/// <summary>Запуск таймера через плагин</summary>
		/// <param name="key">Ключ таймера, используется как уникальный индекс</param>
		/// <param name="timerName">Наименование ключа настроек таймера, которые используются для подсчёта времени до срабатывания</param>
		/// <param name="callback">Событие обратного вызова, которое используется для вызова при срабатывании таймера</param>
		/// <param name="state">Данные, которые передаются в метод обратного вызова при срабатывании таймера</param>
		public void RegisterTimer(String key, String timerName, EventHandler<EventArgs> callback, Object state)
		{
			IPluginDescription pluginTimer = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.RegisterTimer)
				.Invoke(key, timerName, callback, state);
		}

		/// <summary>Остановка таймера через плагин</summary>
		/// <param name="key">Ключ таймера, используется как уникальный индекс</param>
		/// <returns>Результат отключения таймера</returns>
		public Boolean UnregisterTimer(String key)
		{
			IPluginDescription pluginTimer = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			return (Boolean)pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.UnregisterTimer)
				.Invoke(key);
		}

		/// <summary>Выполнить запущенный таймер по ключу</summary>
		/// <param name="key">Ключ таймера для выполнения</param>
		public void InvokeTimer(String key)
		{
			IPluginDescription pluginTimer = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.InvokeTimer)
				.Invoke(key);
		}

		/// <summary>Получить наименования всех созданных таймеров</summary>
		/// <returns>Список наименований созданных таймеров</returns>
		public String[] GetTimersName()
		{
			IPluginDescription pluginTimer = this.PluginInstance;
			if(pluginTimer == null)
				return null;

			return (String[])pluginTimer.Type
				.GetMember<IPluginMethodInfo>(PluginConstants.Methods.GetTimersName)
				.Invoke();
		}

		/// <summary>Проверка на существование таймера по наименоваию таймера</summary>
		/// <param name="timerName">Наименование таймера на проверку существования</param>
		/// <returns>Таймер с таким наименованием - существует</returns>
		public Boolean IsTimerExists(String timerName)
		{
			IPluginDescription pluginTimer = this.PluginInstance;
			if(pluginTimer == null)
				return false;

			return (Boolean)pluginTimer.Type
				.GetMember<IPluginMethodInfo>(PluginConstants.Methods.IsTimerExists)
				.Invoke(timerName);
		}
	}
}