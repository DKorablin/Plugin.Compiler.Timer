using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.UI
{
	public class TimersNameEditor : UITypeEditor
	{
		private IWindowsFormsEditorService _editorService;

		public IEnumerable<String> GetValues(ITypeDescriptorContext context)
		{
			TimerCompilerSettingsItem item = (TimerCompilerSettingsItem)context.Instance;
			TimerCompilerSettingsCollection collection = item.GetOwner();
			return collection.Plugin.Timers.GetTimersName();
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown; //base.GetEditStyle(context);

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			this._editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			// use a list box
			ListBox lb = new ListBox
			{
				SelectionMode = SelectionMode.One,
				DisplayMember = "Key",
				ValueMember = "Value"
			};
			lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

			//context.Instance
			foreach(String item in this.GetValues(context))
			{
				Int32 index = lb.Items.Add(item);
				if(item.Equals(value))
					lb.SelectedIndex = index;
			}

			// show this model stuff
			this._editorService.DropDownControl(lb);
			return lb.SelectedItem == null
				? value // no selection, return the passed-in value as is
				: (String)lb.SelectedItem;
		}

		private void OnListBoxSelectedValueChanged(Object sender, EventArgs e)// close the drop down as soon as something is clicked
			=> this._editorService.CloseDropDown();
	}
}