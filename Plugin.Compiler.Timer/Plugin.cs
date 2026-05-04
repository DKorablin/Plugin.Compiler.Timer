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
		private PluginSettings _settings;

		private ITraceSource Trace { get; }

		internal IHost Host { get; }

		internal CompilerPlugin Compiler {get; private set;}

		internal TimersPlugin Timers { get; private set; }

		internal RuntimeCollection Runtime { get; private set; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
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

		public Plugin(IHost host, ITraceSource trace)
		{
			this.Host = host ?? throw new ArgumentNullException(nameof(host));
			this.Trace = trace ?? throw new ArgumentNullException(nameof(trace));
		}

		/// <summary>Get advanced settings with a custom UI</summary>
		/// <returns></returns>
		public Object GetPluginOptionsControl()
			=> new ConfigCtrl(this);

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			this.Host.Plugins.PluginsLoaded += this.Host_PluginsLoaded;
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
	}
}