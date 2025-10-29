using System;
using Plugin.Compiler.Timer.Runtime;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.Compiler.Timer.Facade
{
	/// <summary>Wrapper over the compiler plugin</summary>
	internal class CompilerPlugin
	{
		private readonly Plugin _plugin;
		private IPluginDescription _pluginCompiler;

		/// <summary>Compilation plugin</summary>
		private static class PluginConstants
		{
			/// <summary>ID of the plugin with runtime compilation</summary>
			public const String Name = "425f8b0c-f049-44ee-8375-4cc874d6bf94";

			/// <summary>Public plugin methods</summary>
			public static class Methods
			{
				/// <summary>Check for code existence</summary>
				public const String IsMethodExists = "IsMethodExists";

				/// <summary>Get a list of all methods created for this plugin</summary>
				public const String GetMethods = "GetMethods";

				/// <summary>Delete code</summary>
				public const String DeleteMethod = "DeleteMethod";

				/// <summary>Execute dynamic code</summary>
				public const String InvokeDynamicMethod = "InvokeDynamicMethod";
			}

			/// <summary>Plugin Windows</summary>
			public static class Windows
			{
				/// <summary>.NET Source Code Editing Window</summary>
				public const String DocumentCompiler = "Plugin.Compiler.DocumentCompiler";

				/// <summary>Data Save Event in the <c>DocumentCompiler</c></summary>
				public const String SaveEventName = "SaveEvent";
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
		public IPluginDescription PluginInstance
		=> this._pluginCompiler ?? (this._pluginCompiler = this._plugin.Host.Plugins[this.Name]);

		/// <summary>Name of the compilation plugin</summary>
		public String Name => PluginConstants.Name;

		/// <summary>Creating a compiler plugin wrapper instance</summary>
		/// <param name="plugin">Current plugin</param>
		public CompilerPlugin(Plugin plugin)
		=> this._plugin = plugin;

		/// <summary>Create a window for editing compilation code</summary>
		/// <param name="methodName">Name of the class used in the compilation code</param>
		/// <param name="onSave">Event called when the code in the window is saved</param>
		public void CreateCompilerWindow(String methodName, EventHandler<DataEventArgs> onSave)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin ID={PluginConstants.Name} not installed");

			if(!(this._plugin.Host is IHostWindows hostWindows))
				throw new InvalidOperationException("To create compiler window plugin should be launched from UI Flatbed");

			IWindow wnd = hostWindows.Windows.CreateWindow(PluginConstants.Name,
				PluginConstants.Windows.DocumentCompiler,
				false,
				new
				{
					CallerPluginId = self.ID,
					ClassName = methodName,
					ArgumentsType = new Type[] { typeof(RuntimeItem) },
					ReturnType = typeof(void),
				});

			if(wnd != null && onSave != null)
				wnd.AddEventHandler(PluginConstants.Windows.SaveEventName, onSave);
		}

		/// <summary>Delete method</summary>
		/// <param name="methodName">Name of the method to delete</param>
		/// <returns>Result of deleting the method</returns>
		public Boolean DeleteMethod(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler != null && (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.DeleteMethod)
					.Invoke(self, methodName);
		}

		/// <summary>Checking for the existence of a dynamic method in the compilation plugin</summary>
		/// <param name="methodName">Name of the method in the compilation plugin</param>
		/// <returns>This method exists in the compilation plugin for this plugin</returns>
		public Boolean IsMethodExists(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler != null && (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.IsMethodExists)
					.Invoke(self, methodName);
		}

		/// <summary>Get a list of all methods</summary>
		/// <returns>List of all methods created for a specific plugin</returns>
		public String[] GetMethods()
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler == null
				? new String[] { }
				: (String[])pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.GetMethods)
					.Invoke(self);
		}

		/// <summary>Call dynamic code written in advance via a plugin</summary>
		/// <param name="methodName">Name of the class used in the compilation code</param>
		/// <param name="compilerArgs">Arguments passed to the compiled class</param>
		/// <returns>Result of method execution</returns>
		public Object InvokeDynamicMethod(String methodName, params Object[] compilerArgs)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin ID={PluginConstants.Name} not installed");

			return pluginCompiler.Type
				.GetMember<IPluginMethodInfo>(PluginConstants.Methods.InvokeDynamicMethod)
				.Invoke(self, methodName, compilerArgs);
		}
	}
}