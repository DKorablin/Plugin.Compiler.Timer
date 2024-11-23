using System;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.Runtime
{
	public class RuntimeItem
	{
		private readonly RuntimeCollection _collection;
		private readonly TimerCompilerSettingsItem _settingsItem;

		public String TimerKey { get; }
		public String SettingsTimerKey => this._settingsItem.TimerKey;

		internal RuntimeItem(RuntimeCollection collection, TimerCompilerSettingsItem settingsItem)
		{
			this._collection = collection;
			this._settingsItem = settingsItem;
			this.TimerKey = settingsItem.TimerKey;
			this.Start();
		}

		public void Start()
			=> this._collection.Plugin.Timers.RegisterTimer(this.TimerKey, this._settingsItem.TimerName, this.OnInvokeTimer, this);

		public void Stop()
			=> this._collection.Plugin.Timers.UnregisterTimer(this.TimerKey);

		private void OnInvokeTimer(Object state, EventArgs e)
			=> this._collection.Plugin.Compiler.InvokeDynamicMethod(this._settingsItem.MethodName, this);

		public override Int32 GetHashCode()
			=> this.TimerKey.GetHashCode();
	}
}