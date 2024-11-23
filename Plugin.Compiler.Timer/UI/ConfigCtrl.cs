using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Plugin.Compiler.Timer.Runtime;
using Plugin.Compiler.Timer.Settings;

namespace Plugin.Compiler.Timer.UI
{
	public partial class ConfigCtrl : UserControl
	{
		private readonly Plugin _plugin;
		private RuntimeCollection _runtime;

		private TimerCompilerSettingsItem SelectedItem => lvTimers.SelectedItems.Count == 0 ? null : (TimerCompilerSettingsItem)lvTimers.SelectedItems[0].Tag;

		private String[] EmptySubItems => Array.ConvertAll<String, String>(new String[lvTimers.Columns.Count], delegate(String a) { return String.Empty; });

		public ConfigCtrl(Plugin plugin)
		{
			this._plugin = plugin;

			InitializeComponent();
			tsbnAdd.Enabled = tsbnRemove.Visible = tsbnExecute.Visible = this._plugin.Compiler.PluginInstance != null && this._plugin.Timers.PluginInstance != null;
			if(!tsbnAdd.Enabled)
			{
				String[] toolTip = new String[2];
				if(this._plugin.Compiler.PluginInstance == null)
					toolTip[0] = $"Plugin ID={this._plugin.Compiler.Name} is not installed";
				if(this._plugin.Timers.PluginInstance == null)
					toolTip[1] = $"Plugin ID={this._plugin.Timers.Name} is not installed";
				tsbnAdd.ToolTipText = String.Join(Environment.NewLine, toolTip).Trim();
				lblNoPlugins.Text = String.Join(Environment.NewLine, toolTip).Trim();
				lblNoPlugins.Visible = true;
			}

			this.DataBind();
		}

		private void DataBind()
			=> this.AddListItem(this._plugin.Settings.Data);

		private ListViewItem CreateListItem(TimerCompilerSettingsItem item)
			=> this.CreateListItem(item, this.EmptySubItems);

		private ListViewItem CreateListItem(TimerCompilerSettingsItem item, String[] emptySubItems)
		{
			ListViewItem result = new ListViewItem() { Tag = item };
			result.SubItems.AddRange(emptySubItems);
			this.ModifyListItem(result);

			return result;
		}

		private void ModifyListItem(ListViewItem listItem)
		{
			TimerCompilerSettingsItem settingsItem = (TimerCompilerSettingsItem)listItem.Tag;

			listItem.SubItems[colTimer.Index].Text = settingsItem.TimerName == null
				? Constant.NullText
				: settingsItem.TimerName;
			listItem.SubItems[colMethod.Index].Text = settingsItem.MethodName == null
				? Constant.NullText
				: settingsItem.MethodName;

			if(settingsItem.TimerName == null || settingsItem.MethodName == null)
				listItem.ForeColor = Color.Gray;
			else if(!this._plugin.Timers.IsTimerExists(settingsItem.TimerName))
				listItem.ForeColor = Color.Gray;
			else if(!this._plugin.Compiler.IsMethodExists(settingsItem.MethodName))
				listItem.ForeColor = Color.Gray;
			else
				listItem.ForeColor = Color.Empty;
		}

		private void AddListItem(IEnumerable<TimerCompilerSettingsItem> proxyItems)
		{
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();
			String[] subItems = this.EmptySubItems;

			foreach(TimerCompilerSettingsItem item in proxyItems)
				itemsToAdd.Add(this.CreateListItem(item, subItems));
			lvTimers.Items.AddRange(itemsToAdd.ToArray());

			/*ColumnHeaderAutoResizeStyle headerAutoResize = itemsToAdd.Count == 0
				? ColumnHeaderAutoResizeStyle.HeaderSize
				: ColumnHeaderAutoResizeStyle.ColumnContent;*/
			lvTimers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		private void tsbnAdd_Click(Object sender, EventArgs e)
		{
			TimerCompilerSettingsItem newItem = this._plugin.Settings.Data.NewMapping();
			ListViewItem listItem = this.CreateListItem(newItem);
			lvTimers.Items.Add(listItem);
			/*this._plugin.Settings.Data.Add(newItem);
			this._plugin.Settings.SaveSettings();*/
		}

		private void tsbnRemove_Click(Object sender, EventArgs e)
		{
			TimerCompilerSettingsItem item = this.SelectedItem;
			if(item != null)
			{
				String message = String.Format("Are you shure you want to remove mapping between timer {0} and method {1}? Method usage count {2:N0}",
					item.TimerName,
					item.MethodName,
					this._plugin.Settings.Data.GetMethodUsageCount(item));

				if(MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if(this._plugin.Settings.Data.Remove(item))
						this._plugin.Settings.SaveSettings();
					lvTimers.SelectedItems[0].Remove();
				}
			}
		}

		private void tsbnExecute_Click(Object sender, EventArgs e)
		{
			TimerCompilerSettingsItem item = this.SelectedItem;
			if(item != null)
			{
				if(this._runtime == null)
					this._runtime = new RuntimeCollection(this._plugin);

				tsbnExecute.Checked = this._runtime.Find(item) != null;
				if(tsbnExecute.Checked)
					this._runtime.Stop(item);
				else
					this._runtime.Start(item);
				tsbnExecute.Checked = !tsbnExecute.Checked;
			}
		}

		private void lvTimers_SelectedIndexChanged(Object sender, EventArgs e)
		{
			TimerCompilerSettingsItem item = this.SelectedItem;
			pgData.SelectedObject = item;
			splitMain.Panel2Collapsed = item == null;
			tsbnRemove.Enabled = tsbnExecute.Enabled = item != null;

			if(tsbnExecute.Enabled)
				tsbnExecute.Checked = this._runtime != null && this._runtime.Find(item) != null;
			else if(tsbnExecute.Checked)
				tsbnExecute.Checked = false;
		}

		private void pgData_PropertyValueChanged(Object s, PropertyValueChangedEventArgs e)
		{
			ListViewItem listItem = lvTimers.SelectedItems[0];

			TimerCompilerSettingsItem item = (TimerCompilerSettingsItem)listItem.Tag;

			this.ModifyListItem(listItem);
			if(String.IsNullOrEmpty(item.TimerName) || String.IsNullOrEmpty(item.MethodName))
				return;

			this._plugin.Settings.Data.AddOrUpdate(item);
			this._plugin.Settings.SaveSettings();
		}
	}
}