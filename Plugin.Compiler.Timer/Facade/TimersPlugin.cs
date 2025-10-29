using System;
using SAL.Flatbed;

namespace Plugin.Compiler.Timer.Facade
{
	/// <summary>Wrapper for a plugin with timers</summary>
	internal class TimersPlugin
	{
		private readonly Plugin _plugin;
		private IPluginDescription _pluginTimer;

		/// <summary>Timer Plugin</summary>
		private static class PluginConstants
		{
			/// <summary>ID of the plugin with timers</summary>
			public const String Name = "69c79417-c168-434a-a597-4e224237a527";

			/// <summary>Public Plugin Methods</summary>
			public static class Methods
			{
				/// <summary>Register a timer</summary>
				public const String RegisterTimer = "RegisterTimer";

				/// <summary>Unregister a timer</summary>
				public const String UnregisterTimer = "UnregisterTimer";

				/// <summary>Execute a timer by key</summary>
				public const String InvokeTimer = "InvokeTimer";

				/// <summary>Get a list of created timers</summary>
				public const String GetTimersName = "GetTimersName";

				/// <summary>A timer with this name exists</summary>
				public const String IsTimerExists = "IsTimerExists";
			}
		}

		/// <summary>Link to the description of your own plugin</summary>
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

		/// <summary>Link to the compilation plugin description</summary>
		public IPluginDescription PluginInstance => this._pluginTimer ?? (this._pluginTimer = this._plugin.Host.Plugins[this.Name]);

		/// <summary>Name of the timer plugin</summary>
		public String Name => PluginConstants.Name;

		/// <summary>Creating a timer plugin wrapper</summary>
		/// <param name="plugin">Current plugin instance</param>
		public TimersPlugin(Plugin plugin)
		=> this._plugin = plugin;

		/// <summary>Starting a timer via a plugin</summary>
		/// <param name="key">Timer key, used as a unique index</param>
		/// <param name="timerName">Name of the timer settings key used to calculate the time until the timer fires</param>
		/// <param name="callback">Callback event used to call when the timer fires</param>
		/// <param name="state">Data passed to the callback method when the timer fires</param>
		public void RegisterTimer(String key, String timerName, EventHandler<EventArgs> callback, Object state)
		{
			IPluginDescription pluginTimer = this.PluginInstance
			?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.RegisterTimer)
			.Invoke(key, timerName, callback, state);
		}

		/// <summary>Stopping a timer via a plugin</summary>
		/// <param name="key">Timer key, used as a unique index</param>
		/// <returns>Result of disabling the timer</returns>
		public Boolean UnregisterTimer(String key)
		{
			IPluginDescription pluginTimer = this.PluginInstance
			?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			return (Boolean)pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.UnregisterTimer)
			.Invoke(key);
		}

		/// <summary>Execute a running timer by key</summary>
		/// <param name="key">Timer key to execute</param>
		public void InvokeTimer(String key)
		{
			IPluginDescription pluginTimer = this.PluginInstance
			?? throw new InvalidOperationException($"Plugin {this.Name} not installed");

			pluginTimer.Type.GetMember<IPluginMethodInfo>(PluginConstants.Methods.InvokeTimer)
			.Invoke(key);
		}

		/// <summary>Get the names of all created timers</summary>
		/// <returns>List of names of created timers</returns>
		public String[] GetTimersName()
		{
			IPluginDescription pluginTimer = this.PluginInstance;
			if(pluginTimer == null)
				return new String[] { };

			return (String[])pluginTimer.Type
			.GetMember<IPluginMethodInfo>(PluginConstants.Methods.GetTimersName)
			.Invoke();
		}

		/// <summary>Checking for the existence of a timer by timer name</summary>
		/// <param name="timerName">Timer name to check for existence</param>
		/// <returns>A timer with this name exists</returns>
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