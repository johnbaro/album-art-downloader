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
                foreach (Script s in theparent.a.scripts)
                {
                    listViewScripts.Items.Add(s.Name);
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
                lock (theparent.a.scripts)
                {
                    foreach (Script s in theparent.a.scripts)
                    {
                        if (listViewScripts.SelectedItems[0].Text == s.Name)
                        {
                            labelScriptName.Text = String.Format("Name: {0}", s.Name);
                            labelScriptVersion.Text = String.Format("Version: {0}", s.Version);
                            labelScriptCreator.Text = String.Format("Creator: {0}", s.Creator);
                        }
                    }
                }
            }
        }


    }
}