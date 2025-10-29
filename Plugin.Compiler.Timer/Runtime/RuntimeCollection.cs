using System;
using System.Collections.Generic;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.Runtime
{
	/// <summary>Collection of running modules</summary>
	internal class RuntimeCollection : IEnumerable<RuntimeItem>
	{
		private readonly Object _lock = new Object();
		private readonly List<RuntimeItem> _timerData = new List<RuntimeItem>();

		public Plugin Plugin { get; }

		/// <summary>Collection with running timers</summary>
		/// <param name="plugin">Plugin</param>
		internal RuntimeCollection(Plugin plugin)
			=> this.Plugin = plugin;

		/// <summary>Start timer from settings</summary>
		/// <param name="settingsItem">Settings from which to start the timer</param>
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
					this.Stop(settingsItem);//I stop the timer if it was already running.

				RuntimeItem item = new RuntimeItem(this, settingsItem);
				this._timerData.Add(item);
			}
		}

		/// <summary>Finding a running timer via settings</summary>
		/// <param name="settingsItem">Settings item</param>
		/// <returns>The found running timer item</returns>
		public RuntimeItem Find(TimerCompilerSettingsItem settingsItem)
		{
			foreach(RuntimeItem item in this)
				if(item.TimerKey == settingsItem.TimerKey
					|| item.SettingsTimerKey == settingsItem.TimerKey)//If the key has changed, then we search by the running key
					return item;

			return null;
		}

		/// <summary>Stop and delete the timer</summary>
		/// <param name="settingsItem">Settings under which the timer was started</param>
		public void Stop(TimerCompilerSettingsItem settingsItem)
		{
			RuntimeItem item = this.Find(settingsItem)
				?? throw new ArgumentException("Settings item to stop is not found", nameof(settingsItem));

			lock(this._lock)
			{
				item.Stop();
				this._timerData.Remove(item);
			}
		}

		/// <summary>Stop all running timers</summary>
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