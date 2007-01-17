/*  This file is part of Album Art Downloader.
 *  CoverDownloader is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  CoverDownloader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CoverDownloader; if not, write to the Free Software             
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA  */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AlbumArtDownloader
{
    public partial class SettingsForm : Form
    {
        MainForm theparent;

        public SettingsForm(MainForm parent)
        {
            theparent = parent;
            InitializeComponent();

            lock (theparent.a.scripts)
            {
				mAllowCheck = true;
                foreach (Script s in theparent.a.scripts)
                {
                    listScripts.Items.Add(s, s.Enabled);
                }
				mAllowCheck = false;
            }

			listScripts_SelectedIndexChanged(null, EventArgs.Empty); //Set up panel visibility and button enabling appropriately
        }
        
        private void SettingsDlg_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void listScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listScripts.SelectedItems.Count > 0)
            {
                panelScriptManager.Visible = true;
                labelScriptName.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Name);
                labelScriptVersion.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Version);
                labelScriptCreator.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Creator);
                
				int selIdx = listScripts.SelectedIndex;
				buttonUp.Enabled = selIdx > 0;
				buttonDown.Enabled = selIdx < listScripts.Items.Count - 1;
            }
            else
            {
                panelScriptManager.Visible = false;
				buttonUp.Enabled = false;
				buttonDown.Enabled = false;
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listScripts, true);
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listScripts, false);
            }
        }

        private void MoveListViewItem(ref CheckedListBox lv, bool moveUp)
        {
            int selIdx;
			int newIdx;

			selIdx = lv.SelectedIndex;
            if (moveUp)
            {
                // ignore moveup of row(0)
                if (selIdx == 0)
                    return;

				newIdx = selIdx - 1;

            }
            else
            {
                // ignore movedown of last item
                if (selIdx == lv.Items.Count - 1)
                    return;

				newIdx = selIdx + 1;
            }

			Script s = (Script)lv.SelectedItem;
			lv.Items.RemoveAt(selIdx);
			lv.Items.Insert(newIdx, s);
			mAllowCheck = true;
			lv.SetItemChecked(newIdx, s.Enabled);
			mAllowCheck = false;
			lv.SelectedIndex = newIdx;

			lv.Refresh();
			lv.Focus();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            lock (theparent.a.scripts)
            {
                for (int i = 0; i < listScripts.Items.Count; i++)
                {
                    ((Script)listScripts.Items[i]).SortPosition = i;
                }

                theparent.UpdateScriptOrder(theparent.a.scripts);
            }
        }

		private bool mAllowCheck; //Only allow check state to change programatically.
		private void listScripts_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (!mAllowCheck)
			{
				//Prevent the check change
				e.NewValue = e.CurrentValue;
				return;
			}
			lock (theparent.a.scripts)
			{
				((Script)listScripts.Items[e.Index]).Enabled = e.NewValue == CheckState.Checked;
			}
		}

		private void listScripts_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X < (SystemInformation.MenuCheckSize.Width + SystemInformation.Border3DSize.Width))
			{
				//They clicked directly on the checkbox, so check it.
				int idx = listScripts.IndexFromPoint(e.Location);
				if (idx >= 0)
				{
					mAllowCheck = true;
					listScripts.SetItemChecked(idx, !listScripts.GetItemChecked(idx));
					mAllowCheck = false;
				}
			}
		}

		private void listScripts_KeyDown(object sender, KeyEventArgs e)
		{
			mAllowCheck = true; //Allow checking by keypress (space, generally)
		}

		private void listScripts_KeyUp(object sender, KeyEventArgs e)
		{
			mAllowCheck = false; //Reset allow checking after keypress
		}
    }
}