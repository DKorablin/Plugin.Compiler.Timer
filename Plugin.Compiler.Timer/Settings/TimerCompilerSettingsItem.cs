using System;
using System.ComponentModel;
using System.Drawing.Design;
using Plugin.Compiler.Timer.UI;

namespace Plugin.Compiler.Timer.Settings
{
	/// <summary>Timer and Executable Code Mapping Settings</summary>
	public class TimerCompilerSettingsItem
	{
		private String _timerName;
		private String _methodName;
		private String _description;
		private Boolean _isAutoStart;
		private TimerCompilerSettingsCollection _owner;

		internal void SetOwner(TimerCompilerSettingsCollection collection)
		{
			this._owner = collection;
			if(this.IsAutoStart)
				this.IsAutoStart = true;//ReCheck method & timer existence
		}
		internal TimerCompilerSettingsCollection GetOwner()
			=> this._owner;

		[DisplayName("Timer Key")]
		[Description("The timer key is used when registering a timer in a timer plugin.")]
		public String TimerKey => this.TimerName + "|" + this.MethodName;

		/// <summary>The name of the timer to which the binding occurs</summary>
		[Editor(typeof(TimersNameEditor), typeof(UITypeEditor))]
		[DisplayName("Timer Name")]
		[Description("Name of the running timer that is executed during execution")]
		[DefaultValue("")]
		public String TimerName
		{
			get => this._timerName;
			set => this._timerName = FixName(value);
		}

		/// <summary>Name of the executable compilation method</summary>
		[Editor(typeof(CompilerMethodEditor), typeof(UITypeEditor))]
		[DisplayName("Method Name")]
		[Description("Name of the method called at runtime")]
		[DefaultValue("")]
		public String MethodName
		{
			get => this._methodName;
			set => this._methodName = FixName(value);
		}

		/// <summary>Mapping description</summary>
		[Description("Description of the algorithm being executed")]
		[DefaultValue("")]
		public String Description
		{
			get => this._description;
			set =>this._description = value == null || value.Trim().Length == 0
				? null
				: value.Trim();
		}

		[Description("Checking for the existence of a method in a compilation plugin")]
		public Boolean IsMethodExists
			=> !String.IsNullOrEmpty(this.MethodName) && this._owner.Plugin.Compiler.IsMethodExists(this.MethodName);

		[Description("Checking for the existence of a timer in a plugin with timers")]
		public Boolean IsTimerExists
			=> !String.IsNullOrEmpty(this.TimerName) && this._owner.Plugin.Timers.IsTimerExists(this.TimerName);

		[DefaultValue(false)]
		[Description("Automatically start a timer element when the host starts")]
		public Boolean IsAutoStart
		{
			get => this._isAutoStart;
			set
			{
				this._isAutoStart = this._owner == null//When loading from JSON, the owner is not yet set.
					? value
					: value && this.IsMethodExists && this.IsTimerExists;
			}
		}

		private static String FixName(String value)
		{
			if(value == null)
				return null;

			for(Int32 loop = value.Length - 1; loop >= 0; loop--)
				if(!Char.IsLetterOrDigit(value[loop]))
					value = value.Remove(loop, 1);

			value = value.Trim();
			return value.Length == 0 ? null : value;
		}
	}
}