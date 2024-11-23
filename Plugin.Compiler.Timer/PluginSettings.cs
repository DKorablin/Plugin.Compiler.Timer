using System;
using System.IO;
using System.Text;
using Plugin.CompilerTimer.Settings;

namespace Plugin.CompilerTimer
{
	public class PluginSettings
	{
		private readonly Plugin _plugin;
		private TimerCompilerSettingsCollection _data;

		internal TimerCompilerSettingsCollection Data
		{
			get => this._data ?? (this._data = this.GetSettings());
			private set => this._data = value;
		}

		internal PluginSettings(Plugin plugin)
			=> this._plugin = plugin;

		private TimerCompilerSettingsCollection GetSettings()
		{
			using(Stream stream = this._plugin.Host.Plugins.Settings(this._plugin).LoadAssemblyBlob("DataJson"))
				if(stream != null)
					using(StreamReader reader = new StreamReader(stream))
						return new TimerCompilerSettingsCollection(this._plugin, reader.ReadToEnd());

			return new TimerCompilerSettingsCollection(this._plugin, null);
		}

		public void SaveSettings()
			=> this.SaveSettings(this.Data);

		private void SaveSettings(TimerCompilerSettingsCollection collection)
		{
			String json = collection.ToJson();
			if(json == null)
				this._plugin.Host.Plugins.Settings(this._plugin).SaveAssemblyBlob("DataJson", null);
			else
			{
				Byte[] payload = Encoding.UTF8.GetBytes(json);
				using(MemoryStream stream = new MemoryStream(payload))
					this._plugin.Host.Plugins.Settings(this._plugin).SaveAssemblyBlob("DataJson", stream);
			}
		}
	}
}