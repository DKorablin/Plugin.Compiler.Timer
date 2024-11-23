using System;
using System.Threading;
using Plugin.CompilerTimer.Facade;
using Plugin.CompilerTimer.Runtime;
using Plugin.CompilerTimer.UI;
using SAL.Flatbed;

namespace Plugin.CompilerTimer
{
	public class Plugin : IPlugin, IPluginSettings<PluginSettings>
	{
		private PluginSettings _settings;

		internal IHost Host { get; }

		internal CompilerPlugin Compiler {get; private set;}

		internal TimersPlugin Timers { get; private set; }

		internal RuntimeCollection Runtime { get; private set; }

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Настройки для взаимодействия из плагина</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.Host.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		public Plugin(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		/// <summary>Получить расширенные настройки с кастомным UI</summary>
		/// <returns></returns>
		public Object GetPluginOptionsControl()
			=> new ConfigCtrl(this);

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			this.Host.Plugins.PluginsLoaded += Host_PluginsLoaded;
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			this.Runtime.StopAll();
			return true;
		}

		private void Host_PluginsLoaded(Object sender, EventArgs e)
		{
			this.Compiler = new CompilerPlugin(this);
			this.Timers = new TimersPlugin(this);
			this.Runtime = new RuntimeCollection(this);

			if(this.Timers.PluginInstance != null && this.Compiler.PluginInstance != null)
				ThreadPool.QueueUserWorkItem(new WaitCallback(StartAutoRunMappingsAsync), this);
		}

		private static void StartAutoRunMappingsAsync(Object state)
		{
			Plugin plugin = (Plugin)state;

			foreach(var settingsItem in plugin.Settings.Data)
				if(settingsItem.IsAutoStart)
					plugin.Runtime.Start(settingsItem);
		}
	}
}