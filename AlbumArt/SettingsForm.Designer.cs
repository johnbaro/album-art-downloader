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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.checkBoxAutoDownloadFullImage = new System.Windows.Forms.CheckBox();
            this.checkBoxShowSizeOverlay = new System.Windows.Forms.CheckBox();
            this.checkBoxShowFolderPictures = new System.Windows.Forms.CheckBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.numericUpDownThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            this.checkBoxShowExistingArt = new System.Windows.Forms.CheckBox();
            this.checkBoxClose = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.numericUpDownThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.tabPageScriptManager = new System.Windows.Forms.TabPage();
            this.splitContainerScriptManager = new System.Windows.Forms.SplitContainer();
            this.listViewScripts = new System.Windows.Forms.ListView();
            this.clhScriptName = new System.Windows.Forms.ColumnHeader();
            this.panelUpDown = new System.Windows.Forms.Panel();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.panelScriptManager = new System.Windows.Forms.Panel();
            this.labelScriptCreator = new System.Windows.Forms.Label();
            this.labelScriptVersion = new System.Windows.Forms.Label();
            this.labelScriptName = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.tabControlSettings.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.tabPageScriptManager.SuspendLayout();
            this.splitContainerScriptManager.Panel1.SuspendLayout();
            this.splitContainerScriptManager.Panel2.SuspendLayout();
            this.splitContainerScriptManager.SuspendLayout();
            this.panelUpDown.SuspendLayout();
            this.panelScriptManager.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Thumbnail Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Use";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "threads";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Default Art Filename";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.AutoSize = true;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(469, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(82, 27);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.CausesValidation = false;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(381, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(82, 27);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
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
            this.tabPageGeneral.Controls.Add(this.checkBoxAutoDownloadFullImage);
            this.tabPageGeneral.Controls.Add(this.checkBoxShowSizeOverlay);
            this.tabPageGeneral.Controls.Add(this.checkBoxShowFolderPictures);
            this.tabPageGeneral.Controls.Add(this.checkBoxBold);
            this.tabPageGeneral.Controls.Add(this.numericUpDownThumbnailHeight);
            this.tabPageGeneral.Controls.Add(this.label7);
            this.tabPageGeneral.Controls.Add(this.label4);
            this.tabPageGeneral.Controls.Add(this.label3);
            this.tabPageGeneral.Controls.Add(this.checkBoxShowExistingArt);
            this.tabPageGeneral.Controls.Add(this.label2);
            this.tabPageGeneral.Controls.Add(this.checkBoxClose);
            this.tabPageGeneral.Controls.Add(this.textBox1);
            this.tabPageGeneral.Controls.Add(this.numericUpDownThumbnailWidth);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Controls.Add(this.numericUpDown3);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(547, 395);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoDownloadFullImage
            // 
            this.checkBoxAutoDownloadFullImage.AutoSize = true;
            this.checkBoxAutoDownloadFullImage.Checked = global::AlbumArtDownloader.Properties.Settings.Default.AutoDownloadFullImage;
            this.checkBoxAutoDownloadFullImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoDownloadFullImage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "AutoDownloadFullImage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxAutoDownloadFullImage.Location = new System.Drawing.Point(8, 98);
            this.checkBoxAutoDownloadFullImage.Name = "checkBoxAutoDownloadFullImage";
            this.checkBoxAutoDownloadFullImage.Size = new System.Drawing.Size(333, 17);
            this.checkBoxAutoDownloadFullImage.TabIndex = 20;
            this.checkBoxAutoDownloadFullImage.Text = "Automatically download the full image to determine size if required";
            this.checkBoxAutoDownloadFullImage.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowSizeOverlay
            // 
            this.checkBoxShowSizeOverlay.AutoSize = true;
            this.checkBoxShowSizeOverlay.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowSizeOverlay;
            this.checkBoxShowSizeOverlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowSizeOverlay.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowSizeOverlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowSizeOverlay.Location = new System.Drawing.Point(8, 75);
            this.checkBoxShowSizeOverlay.Name = "checkBoxShowSizeOverlay";
            this.checkBoxShowSizeOverlay.Size = new System.Drawing.Size(115, 17);
            this.checkBoxShowSizeOverlay.TabIndex = 19;
            this.checkBoxShowSizeOverlay.Text = "Show Size Overlay";
            this.checkBoxShowSizeOverlay.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowFolderPictures
            // 
            this.checkBoxShowFolderPictures.AutoSize = true;
            this.checkBoxShowFolderPictures.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowFolderPictures;
            this.checkBoxShowFolderPictures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowFolderPictures.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowFolderPictures", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowFolderPictures.Location = new System.Drawing.Point(8, 52);
            this.checkBoxShowFolderPictures.Name = "checkBoxShowFolderPictures";
            this.checkBoxShowFolderPictures.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowFolderPictures.TabIndex = 18;
            this.checkBoxShowFolderPictures.Text = "Show pictures in folder";
            this.checkBoxShowFolderPictures.UseVisualStyleBackColor = true;
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
            // numericUpDownThumbnailHeight
            // 
            this.numericUpDownThumbnailHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailHeight.Location = new System.Drawing.Point(156, 167);
            this.numericUpDownThumbnailHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownThumbnailHeight.Name = "numericUpDownThumbnailHeight";
            this.numericUpDownThumbnailHeight.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownThumbnailHeight.TabIndex = 7;
            this.numericUpDownThumbnailHeight.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailHeight;
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
            // checkBoxClose
            // 
            this.checkBoxClose.AutoSize = true;
            this.checkBoxClose.Checked = global::AlbumArtDownloader.Properties.Settings.Default.CloseAfterSaving;
            this.checkBoxClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "CloseAfterSaving", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxClose.Location = new System.Drawing.Point(8, 121);
            this.checkBoxClose.Name = "checkBoxClose";
            this.checkBoxClose.Size = new System.Drawing.Size(125, 17);
            this.checkBoxClose.TabIndex = 8;
            this.checkBoxClose.Text = "Close after saving art";
            this.checkBoxClose.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SaveFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(113, 191);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = global::AlbumArtDownloader.Properties.Settings.Default.SaveFileName;
            // 
            // numericUpDownThumbnailWidth
            // 
            this.numericUpDownThumbnailWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailWidth.Location = new System.Drawing.Point(85, 167);
            this.numericUpDownThumbnailWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownThumbnailWidth.Name = "numericUpDownThumbnailWidth";
            this.numericUpDownThumbnailWidth.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownThumbnailWidth.TabIndex = 5;
            this.numericUpDownThumbnailWidth.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailWidth;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThreadCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown3.Location = new System.Drawing.Point(37, 142);
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
            // tabPageScriptManager
            // 
            this.tabPageScriptManager.Controls.Add(this.splitContainerScriptManager);
            this.tabPageScriptManager.Location = new System.Drawing.Point(4, 22);
            this.tabPageScriptManager.Name = "tabPageScriptManager";
            this.tabPageScriptManager.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScriptManager.Size = new System.Drawing.Size(547, 395);
            this.tabPageScriptManager.TabIndex = 1;
            this.tabPageScriptManager.Text = "Script Manager - NOT FUNCTIONAL";
            this.tabPageScriptManager.UseVisualStyleBackColor = true;
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
            this.splitContainerScriptManager.Panel1.Controls.Add(this.panelUpDown);
            // 
            // splitContainerScriptManager.Panel2
            // 
            this.splitContainerScriptManager.Panel2.Controls.Add(this.panelScriptManager);
            this.splitContainerScriptManager.Size = new System.Drawing.Size(541, 389);
            this.splitContainerScriptManager.SplitterDistance = 180;
            this.splitContainerScriptManager.TabIndex = 1;
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
            this.listViewScripts.Size = new System.Drawing.Size(145, 389);
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
            // panelUpDown
            // 
            this.panelUpDown.Controls.Add(this.buttonDown);
            this.panelUpDown.Controls.Add(this.buttonUp);
            this.panelUpDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelUpDown.Location = new System.Drawing.Point(145, 0);
            this.panelUpDown.Name = "panelUpDown";
            this.panelUpDown.Size = new System.Drawing.Size(35, 389);
            this.panelUpDown.TabIndex = 6;
            // 
            // buttonDown
            // 
            this.buttonDown.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveDown;
            this.buttonDown.Location = new System.Drawing.Point(3, 74);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(29, 65);
            this.buttonDown.TabIndex = 1;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveUp;
            this.buttonUp.Location = new System.Drawing.Point(3, 3);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(29, 65);
            this.buttonUp.TabIndex = 0;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
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
            // labelScriptCreator
            // 
            this.labelScriptCreator.AutoSize = true;
            this.labelScriptCreator.Location = new System.Drawing.Point(3, 41);
            this.labelScriptCreator.Name = "labelScriptCreator";
            this.labelScriptCreator.Size = new System.Drawing.Size(44, 13);
            this.labelScriptCreator.TabIndex = 2;
            this.labelScriptCreator.Text = "Creator:";
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
            // labelScriptName
            // 
            this.labelScriptName.AutoSize = true;
            this.labelScriptName.Location = new System.Drawing.Point(3, 5);
            this.labelScriptName.Name = "labelScriptName";
            this.labelScriptName.Size = new System.Drawing.Size(38, 13);
            this.labelScriptName.TabIndex = 0;
            this.labelScriptName.Text = "Name:";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonCancel);
            this.panelButtons.Controls.Add(this.buttonOK);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 421);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(555, 35);
            this.panelButtons.TabIndex = 22;
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
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.tabPageScriptManager.ResumeLayout(false);
            this.splitContainerScriptManager.Panel1.ResumeLayout(false);
            this.splitContainerScriptManager.Panel2.ResumeLayout(false);
            this.splitContainerScriptManager.ResumeLayout(false);
            this.panelUpDown.ResumeLayout(false);
            this.panelScriptManager.ResumeLayout(false);
            this.panelScriptManager.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowExistingArt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownThumbnailWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownThumbnailHeight;
        private System.Windows.Forms.CheckBox checkBoxClose;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxBold;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
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
        private System.Windows.Forms.CheckBox checkBoxShowSizeOverlay;
        private System.Windows.Forms.CheckBox checkBoxAutoDownloadFullImage;
        private System.Windows.Forms.Panel panelUpDown;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonUp;
    }
}