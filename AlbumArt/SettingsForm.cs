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

            ListViewItem listViewItem;

            lock (theparent.a.scripts)
            {
                foreach (Script s in theparent.a.scripts)
                {
                    listViewItem = listViewScripts.Items.Add(s.Name);
                    listViewItem.Tag = s;
                }
            }
            listViewScripts.Columns[0].Width = listViewScripts.Width - 4;
        }
        
        private void SettingsDlg_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void listViewScripts_SizeChanged(object sender, EventArgs e)
        {
            listViewScripts.Columns[0].Width = listViewScripts.Width - 4;
        }

        private void listViewScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewScripts.SelectedItems.Count > 0)
            {
                panelScriptManager.Visible = true;
                labelScriptName.Text = String.Format("Name: {0}", ((Script)listViewScripts.SelectedItems[0].Tag).Name);
                labelScriptVersion.Text = String.Format("Name: {0}", ((Script)listViewScripts.SelectedItems[0].Tag).Version);
                labelScriptCreator.Text = String.Format("Name: {0}", ((Script)listViewScripts.SelectedItems[0].Tag).Creator);
                checkBoxScriptEnabled.Checked = ((Script)listViewScripts.SelectedItems[0].Tag).Enabled;
            }
            else
            {
                panelScriptManager.Visible = false;
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listViewScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listViewScripts, true);
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listViewScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listViewScripts, false);
            }
        }

        private void MoveListViewItem(ref ListView lv, bool moveUp)
        {
            int selIdx;
            ListViewItem lvi;

            selIdx = lv.SelectedItems[0].Index;
            if (moveUp)
            {
                // ignore moveup of row(0)
                if (selIdx == 0)
                    return;

                lvi = (ListViewItem)lv.SelectedItems[0].Clone();
                lv.SelectedItems[0].Remove();
                lv.Items.Insert(selIdx - 1, lvi);
                
                lv.Items[selIdx - 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
            else
            {
                // ignore movedown of last item
                if (selIdx == lv.Items.Count - 1)
                    return;

                lvi = (ListViewItem)lv.SelectedItems[0].Clone();
                lv.SelectedItems[0].Remove();
                lv.Items.Insert(selIdx + 1, lvi);

                lv.Items[selIdx + 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            lock (theparent.a.scripts)
            {
                for (int i = 0; i < listViewScripts.Items.Count; i++)
                {
                    ((Script)listViewScripts.Items[i].Tag).SortPosition = i;
                }

                theparent.UpdateScriptOrder(theparent.a.scripts);
            }
        }

        private void checkBoxScriptEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (listViewScripts.SelectedItems.Count > 0)
            {
                lock (theparent.a.scripts)
                {
                    ((Script)listViewScripts.SelectedItems[0].Tag).Enabled = checkBoxScriptEnabled.Checked;
                }

            }
        }

    }
}