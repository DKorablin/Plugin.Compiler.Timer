using System;
using System.Collections.Generic;

namespace Plugin.Compiler.Timer.Settings
{
	/// <summary>Timer settings storage</summary>
	internal class TimerCompilerSettingsCollection : IEnumerable<TimerCompilerSettingsItem>
	{
		/// <summary>Timer settings storage dictionary</summary>
		private readonly List<TimerCompilerSettingsItem> _timerData = new List<TimerCompilerSettingsItem>();
		private const String ConstMethodName = "CompilerTimerMethod";
		private const String ConstTimerName = "CompilerTimer";

		internal Plugin Plugin { get; }

		/// <summary>Get mapping by index</summary>
		/// <param name="index">Index to get mapping by</param>
		/// <returns>Mapping by index or null</returns>
		public TimerCompilerSettingsItem this[Int32 index]
		{
			get => index < 0 || index >= this._timerData.Count
				? null
				: this._timerData[index];
			set => this._timerData[index] = value;
		}

		/// <summary>Creating a new mapping</summary>
		/// <returns>Created new mapping</returns>
		public TimerCompilerSettingsItem NewMapping()
		{
			TimerCompilerSettingsItem result = new TimerCompilerSettingsItem() { TimerName = TimerCompilerSettingsCollection.ConstTimerName, };

			result.SetOwner(this);
			return result;
		}

		/// <summary>Get a unique method name for the new timer</summary>
		/// <returns>Unique method name</returns>
		public String GetUniqueMethodName()
		{
			String methodName = TimerCompilerSettingsCollection.ConstMethodName;
			UInt32 count = 1;
			while(this._timerData.Exists(p => p.MethodName == methodName))
				methodName = String.Join("_", new String[] { TimerCompilerSettingsCollection.ConstMethodName, (count++).ToString(), });

			return methodName;
		}

		/// <summary>Creating a timer mapping collection instance with the compiler</summary>
		/// <param name="plugin">The current plugin instance</param>
		/// <param name="json">JSON from which to create the collection</param>
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

		/// <summary>Add or change an item in the list</summary>
		/// <param name="item">Item to add</param>
		public void AddOrUpdate(TimerCompilerSettingsItem item)
		{
			Int32 index = this._timerData.IndexOf(item);
			if(index > -1)
				this._timerData[index] = item;
			else
				this._timerData.Add(item);
		}

		/// <summary>Remove the timer mapping to the method, using the compiler's method removal function.</summary>
		/// <param name="item">Timer mapping element to the compiler.</param>
		/// <returns>Mapping removed successfully or mapping not found.</returns>
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

		/// <summary>Get the number of method usages</summary>
		/// <param name="settings">Mapping element for which to get the number of method usages</param>
		/// <returns>Number of method usages by other mappings</returns>
		public Int32 GetMethodUsageCount(TimerCompilerSettingsItem settings)
		{
			Int32 count = 0;
			if(settings.MethodName != null)
				foreach(TimerCompilerSettingsItem item in this)
					if(item.MethodName == settings.MethodName)
						count++;
			return count;
		}

		/// <summary>Convert mappings to JSON</summary>
		/// <returns>String representation of the object</returns>
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