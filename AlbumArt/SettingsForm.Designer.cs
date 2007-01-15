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
namespace AlbumArtDownloader
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxClose = new System.Windows.Forms.CheckBox();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxShowExistingArt = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.tabPageScriptManager = new System.Windows.Forms.TabPage();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.checkBoxShowFolderPictures = new System.Windows.Forms.CheckBox();
            this.listViewScripts = new System.Windows.Forms.ListView();
            this.clhScriptName = new System.Windows.Forms.ColumnHeader();
            this.splitContainerScriptManager = new System.Windows.Forms.SplitContainer();
            this.panelScriptManager = new System.Windows.Forms.Panel();
            this.labelScriptName = new System.Windows.Forms.Label();
            this.labelScriptVersion = new System.Windows.Forms.Label();
            this.labelScriptCreator = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabControlSettings.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageScriptManager.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.splitContainerScriptManager.Panel1.SuspendLayout();
            this.splitContainerScriptManager.Panel2.SuspendLayout();
            this.splitContainerScriptManager.SuspendLayout();
            this.panelScriptManager.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Thumbnail Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Use";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "threads";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Default Art Filename";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SaveFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(113, 145);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = global::AlbumArtDownloader.Properties.Settings.Default.SaveFileName;
            // 
            // checkBoxBold
            // 
            this.checkBoxBold.AutoSize = true;
            this.checkBoxBold.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ExactMatchBold;
            this.checkBoxBold.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ExactMatchBold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBold.Location = new System.Drawing.Point(8, 6);
            this.checkBoxBold.Name = "checkBoxBold";
            this.checkBoxBold.Size = new System.Drawing.Size(160, 17);
            this.checkBoxBold.TabIndex = 13;
            this.checkBoxBold.Text = "Show exact matches in Bold";
            this.checkBoxBold.UseVisualStyleBackColor = true;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThreadCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown3.Location = new System.Drawing.Point(37, 96);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown3.TabIndex = 10;
            this.numericUpDown3.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThreadCount;
            // 
            // checkBoxClose
            // 
            this.checkBoxClose.AutoSize = true;
            this.checkBoxClose.Checked = global::AlbumArtDownloader.Properties.Settings.Default.CloseAfterSaving;
            this.checkBoxClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "CloseAfterSaving", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxClose.Location = new System.Drawing.Point(8, 75);
            this.checkBoxClose.Name = "checkBoxClose";
            this.checkBoxClose.Size = new System.Drawing.Size(125, 17);
            this.checkBoxClose.TabIndex = 8;
            this.checkBoxClose.Text = "Close after saving art";
            this.checkBoxClose.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Location = new System.Drawing.Point(156, 121);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown2.TabIndex = 7;
            this.numericUpDown2.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailHeight;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(85, 121);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailWidth;
            // 
            // checkBoxShowExistingArt
            // 
            this.checkBoxShowExistingArt.AutoSize = true;
            this.checkBoxShowExistingArt.Checked = global::AlbumArtDownloader.Properties.Settings.Default.PreviewSavedArt;
            this.checkBoxShowExistingArt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowExistingArt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "PreviewSavedArt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowExistingArt.Location = new System.Drawing.Point(8, 29);
            this.checkBoxShowExistingArt.Name = "checkBoxShowExistingArt";
            this.checkBoxShowExistingArt.Size = new System.Drawing.Size(106, 17);
            this.checkBoxShowExistingArt.TabIndex = 3;
            this.checkBoxShowExistingArt.Text = "Show existing art";
            this.checkBoxShowExistingArt.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.AutoSize = true;
            this.button4.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button4.Location = new System.Drawing.Point(469, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(82, 27);
            this.button4.TabIndex = 20;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.AutoSize = true;
            this.button3.CausesValidation = false;
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(381, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 27);
            this.button3.TabIndex = 19;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabPageGeneral);
            this.tabControlSettings.Controls.Add(this.tabPageScriptManager);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(555, 421);
            this.tabControlSettings.TabIndex = 21;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.checkBoxShowFolderPictures);
            this.tabPageGeneral.Controls.Add(this.checkBoxBold);
            this.tabPageGeneral.Controls.Add(this.numericUpDown2);
            this.tabPageGeneral.Controls.Add(this.label7);
            this.tabPageGeneral.Controls.Add(this.label4);
            this.tabPageGeneral.Controls.Add(this.label3);
            this.tabPageGeneral.Controls.Add(this.checkBoxShowExistingArt);
            this.tabPageGeneral.Controls.Add(this.label2);
            this.tabPageGeneral.Controls.Add(this.checkBoxClose);
            this.tabPageGeneral.Controls.Add(this.textBox1);
            this.tabPageGeneral.Controls.Add(this.numericUpDown1);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Controls.Add(this.numericUpDown3);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(430, 395);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // tabPageScriptManager
            // 
            this.tabPageScriptManager.Controls.Add(this.splitContainerScriptManager);
            this.tabPageScriptManager.Location = new System.Drawing.Point(4, 22);
            this.tabPageScriptManager.Name = "tabPageScriptManager";
            this.tabPageScriptManager.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScriptManager.Size = new System.Drawing.Size(547, 395);
            this.tabPageScriptManager.TabIndex = 1;
            this.tabPageScriptManager.Text = "Script Manager";
            this.tabPageScriptManager.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.button3);
            this.panelButtons.Controls.Add(this.button4);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 421);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(555, 35);
            this.panelButtons.TabIndex = 22;
            // 
            // checkBoxShowFolderPictures
            // 
            this.checkBoxShowFolderPictures.AutoSize = true;
            this.checkBoxShowFolderPictures.Checked = global::AlbumArtDownloader.Properties.Settings.Default.PreviewSavedArt;
            this.checkBoxShowFolderPictures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowFolderPictures.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "PreviewSavedArt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowFolderPictures.Location = new System.Drawing.Point(8, 52);
            this.checkBoxShowFolderPictures.Name = "checkBoxShowFolderPictures";
            this.checkBoxShowFolderPictures.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowFolderPictures.TabIndex = 18;
            this.checkBoxShowFolderPictures.Text = "Show pictures in folder";
            this.checkBoxShowFolderPictures.UseVisualStyleBackColor = true;
            // 
            // listViewScripts
            // 
            this.listViewScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhScriptName});
            this.listViewScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewScripts.FullRowSelect = true;
            this.listViewScripts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewScripts.HideSelection = false;
            this.listViewScripts.Location = new System.Drawing.Point(0, 0);
            this.listViewScripts.MultiSelect = false;
            this.listViewScripts.Name = "listViewScripts";
            this.listViewScripts.Scrollable = false;
            this.listViewScripts.Size = new System.Drawing.Size(180, 389);
            this.listViewScripts.TabIndex = 0;
            this.listViewScripts.UseCompatibleStateImageBehavior = false;
            this.listViewScripts.View = System.Windows.Forms.View.Details;
            this.listViewScripts.SelectedIndexChanged += new System.EventHandler(this.listViewScripts_SelectedIndexChanged);
            this.listViewScripts.SizeChanged += new System.EventHandler(this.listViewScripts_SizeChanged);
            // 
            // clhScriptName
            // 
            this.clhScriptName.Text = "Name";
            this.clhScriptName.Width = 100;
            // 
            // splitContainerScriptManager
            // 
            this.splitContainerScriptManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerScriptManager.Location = new System.Drawing.Point(3, 3);
            this.splitContainerScriptManager.Name = "splitContainerScriptManager";
            // 
            // splitContainerScriptManager.Panel1
            // 
            this.splitContainerScriptManager.Panel1.Controls.Add(this.listViewScripts);
            // 
            // splitContainerScriptManager.Panel2
            // 
            this.splitContainerScriptManager.Panel2.Controls.Add(this.panelScriptManager);
            this.splitContainerScriptManager.Size = new System.Drawing.Size(541, 389);
            this.splitContainerScriptManager.SplitterDistance = 180;
            this.splitContainerScriptManager.TabIndex = 1;
            // 
            // panelScriptManager
            // 
            this.panelScriptManager.Controls.Add(this.labelScriptCreator);
            this.panelScriptManager.Controls.Add(this.labelScriptVersion);
            this.panelScriptManager.Controls.Add(this.labelScriptName);
            this.panelScriptManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScriptManager.Location = new System.Drawing.Point(0, 0);
            this.panelScriptManager.Name = "panelScriptManager";
            this.panelScriptManager.Size = new System.Drawing.Size(357, 389);
            this.panelScriptManager.TabIndex = 0;
            // 
            // labelScriptName
            // 
            this.labelScriptName.AutoSize = true;
            this.labelScriptName.Location = new System.Drawing.Point(3, 5);
            this.labelScriptName.Name = "labelScriptName";
            this.labelScriptName.Size = new System.Drawing.Size(38, 13);
            this.labelScriptName.TabIndex = 0;
            this.labelScriptName.Text = "Name:";
            // 
            // labelScriptVersion
            // 
            this.labelScriptVersion.AutoSize = true;
            this.labelScriptVersion.Location = new System.Drawing.Point(3, 23);
            this.labelScriptVersion.Name = "labelScriptVersion";
            this.labelScriptVersion.Size = new System.Drawing.Size(45, 13);
            this.labelScriptVersion.TabIndex = 1;
            this.labelScriptVersion.Text = "Version:";
            // 
            // labelScriptCreator
            // 
            this.labelScriptCreator.AutoSize = true;
            this.labelScriptCreator.Location = new System.Drawing.Point(3, 41);
            this.labelScriptCreator.Name = "labelScriptCreator";
            this.labelScriptCreator.Size = new System.Drawing.Size(44, 13);
            this.labelScriptCreator.TabIndex = 2;
            this.labelScriptCreator.Text = "Creator:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(555, 456);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.panelButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.Validated += new System.EventHandler(this.SettingsDlg_Validated);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageScriptManager.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.splitContainerScriptManager.Panel1.ResumeLayout(false);
            this.splitContainerScriptManager.Panel2.ResumeLayout(false);
            this.splitContainerScriptManager.ResumeLayout(false);
            this.panelScriptManager.ResumeLayout(false);
            this.panelScriptManager.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowExistingArt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox checkBoxClose;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxBold;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageScriptManager;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.CheckBox checkBoxShowFolderPictures;
        private System.Windows.Forms.ListView listViewScripts;
        private System.Windows.Forms.ColumnHeader clhScriptName;
        private System.Windows.Forms.SplitContainer splitContainerScriptManager;
        private System.Windows.Forms.Panel panelScriptManager;
        private System.Windows.Forms.Label labelScriptCreator;
        private System.Windows.Forms.Label labelScriptVersion;
        private System.Windows.Forms.Label labelScriptName;
    }
}