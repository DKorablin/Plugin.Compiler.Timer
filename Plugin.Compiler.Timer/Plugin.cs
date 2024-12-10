using System;
using System.Diagnostics;
using System.Threading;
using Plugin.Compiler.Timer.Facade;
using Plugin.Compiler.Timer.Runtime;
using Plugin.Compiler.Timer.UI;
using SAL.Flatbed;

namespace Plugin.Compiler.Timer
{
	public class Plugin : IPlugin, IPluginSettings<PluginSettings>
	{
		private TraceSource _trace;
		private PluginSettings _settings;

		private TraceSource Trace => this._trace ?? (this._trace = Plugin.CreateTraceSource<Plugin>());

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

			try
			{
				foreach(var settingsItem in plugin.Settings.Data)
					if(settingsItem.IsAutoStart)
						plugin.Runtime.Start(settingsItem);
			}catch(Exception exc)
			{
				plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
			}
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}