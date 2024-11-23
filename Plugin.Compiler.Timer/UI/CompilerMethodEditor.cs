using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.UI
{
	public class CompilerMethodEditor : UITypeEditor
	{
		private IWindowsFormsEditorService _editorService;

		public IEnumerable<String> GetValues(TimerCompilerSettingsItem item)
		{
			TimerCompilerSettingsCollection collection = item.GetOwner();
			return collection.Plugin.Compiler.GetMethods();
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown; //base.GetEditStyle(context);

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			_editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			// use a list box
			ListBox lb = new ListBox
			{
				SelectionMode = SelectionMode.One,
				DisplayMember = "Key",
				ValueMember = "Value",
			};
			lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

			TimerCompilerSettingsItem container = (TimerCompilerSettingsItem)context.Instance;
			foreach(String item in this.GetValues(container))
			{
				Int32 index = lb.Items.Add(item);
				if(item.Equals(value))
					lb.SelectedIndex = index;
			}

			Int32 newItemIndex = lb.Items.Add("Add...");

			// show this model stuff
			this._editorService.DropDownControl(lb);

			String result = (String)lb.SelectedItem;
			if(result == null)
				return value;

			result = lb.SelectedIndex == newItemIndex
				? container.GetOwner().GetUniqueMethodName()
				: result;
			container.GetOwner().Plugin.Compiler.CreateCompilerWindow(result, null);

			return result;
		}

		private void OnListBoxSelectedValueChanged(Object sender, EventArgs e)// close the drop down as soon as something is clicked
			=> _editorService.CloseDropDown();
	}
}