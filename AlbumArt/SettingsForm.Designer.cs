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
            this.buttonClose = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.buttonSizeOverlayColorBackground = new System.Windows.Forms.Button();
            this.labelSizeOverlayColorBackground = new System.Windows.Forms.Label();
            this.pictureBoxPreviewSizeOverlayColor = new System.Windows.Forms.PictureBox();
            this.buttonSizeOverlayColorForeground = new System.Windows.Forms.Button();
            this.labelSizeOverlayColorForeground = new System.Windows.Forms.Label();
            this.tabPageScriptManager = new System.Windows.Forms.TabPage();
            this.splitContainerScriptManager = new System.Windows.Forms.SplitContainer();
            this.listScripts = new System.Windows.Forms.CheckedListBox();
            this.panelUpDown = new System.Windows.Forms.Panel();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.panelScriptManager = new System.Windows.Forms.Panel();
            this.labelScriptCreator = new System.Windows.Forms.Label();
            this.labelScriptVersion = new System.Windows.Forms.Label();
            this.labelScriptName = new System.Windows.Forms.Label();
            this.clhScriptName = new System.Windows.Forms.ColumnHeader();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.colorDialogSizeOverlay = new System.Windows.Forms.ColorDialog();
            this.groupBoxSizeOverlay = new System.Windows.Forms.GroupBox();
            this.groupBoxGeneralSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoDownloadFullImage = new System.Windows.Forms.CheckBox();
            this.checkBoxShowFolderPictures = new System.Windows.Forms.CheckBox();
            this.numericUpDownThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.numericUpDownThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            this.checkBoxClose = new System.Windows.Forms.CheckBox();
            this.checkBoxShowExistingArt = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSizeOverlayColor2 = new System.Windows.Forms.CheckBox();
            this.checkBoxShowSizeOverlay = new System.Windows.Forms.CheckBox();
            this.textBoxSizeOverlayColorBackground = new System.Windows.Forms.TextBox();
            this.textBoxSizeOverlayColorForeground = new System.Windows.Forms.TextBox();
            this.tabControlSettings.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreviewSizeOverlayColor)).BeginInit();
            this.tabPageScriptManager.SuspendLayout();
            this.splitContainerScriptManager.Panel1.SuspendLayout();
            this.splitContainerScriptManager.Panel2.SuspendLayout();
            this.splitContainerScriptManager.SuspendLayout();
            this.panelUpDown.SuspendLayout();
            this.panelScriptManager.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.groupBoxSizeOverlay.SuspendLayout();
            this.groupBoxGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Thumbnail Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Use";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "threads";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 189);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Default Art Filename";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.AutoSize = true;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(469, 3);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 27);
            this.buttonClose.TabIndex = 20;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
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
            this.tabPageGeneral.Controls.Add(this.groupBoxGeneralSettings);
            this.tabPageGeneral.Controls.Add(this.groupBoxSizeOverlay);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(547, 395);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // buttonSizeOverlayColorBackground
            // 
            this.buttonSizeOverlayColorBackground.Location = new System.Drawing.Point(161, 60);
            this.buttonSizeOverlayColorBackground.Name = "buttonSizeOverlayColorBackground";
            this.buttonSizeOverlayColorBackground.Size = new System.Drawing.Size(29, 23);
            this.buttonSizeOverlayColorBackground.TabIndex = 27;
            this.buttonSizeOverlayColorBackground.Text = "...";
            this.buttonSizeOverlayColorBackground.UseVisualStyleBackColor = true;
            this.buttonSizeOverlayColorBackground.Click += new System.EventHandler(this.buttonSizeOverlayColorBackground_Click);
            // 
            // labelSizeOverlayColorBackground
            // 
            this.labelSizeOverlayColorBackground.AutoSize = true;
            this.labelSizeOverlayColorBackground.Location = new System.Drawing.Point(3, 65);
            this.labelSizeOverlayColorBackground.Name = "labelSizeOverlayColorBackground";
            this.labelSizeOverlayColorBackground.Size = new System.Drawing.Size(102, 13);
            this.labelSizeOverlayColorBackground.TabIndex = 25;
            this.labelSizeOverlayColorBackground.Text = "Size Overlay Color 2";
            // 
            // pictureBoxPreviewSizeOverlayColor
            // 
            this.pictureBoxPreviewSizeOverlayColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxPreviewSizeOverlayColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxPreviewSizeOverlayColor.DataBindings.Add(new System.Windows.Forms.Binding("Tag", global::AlbumArtDownloader.Properties.Settings.Default, "SizeOverlayPreviewColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.pictureBoxPreviewSizeOverlayColor.Location = new System.Drawing.Point(196, 34);
            this.pictureBoxPreviewSizeOverlayColor.Name = "pictureBoxPreviewSizeOverlayColor";
            this.pictureBoxPreviewSizeOverlayColor.Size = new System.Drawing.Size(90, 48);
            this.pictureBoxPreviewSizeOverlayColor.TabIndex = 24;
            this.pictureBoxPreviewSizeOverlayColor.TabStop = false;
            this.pictureBoxPreviewSizeOverlayColor.Tag = global::AlbumArtDownloader.Properties.Settings.Default.SizeOverlayPreviewColor;
            this.pictureBoxPreviewSizeOverlayColor.Click += new System.EventHandler(this.pictureBoxPreviewSizeOverlayColor_Click);
            // 
            // buttonSizeOverlayColorForeground
            // 
            this.buttonSizeOverlayColorForeground.Location = new System.Drawing.Point(161, 34);
            this.buttonSizeOverlayColorForeground.Name = "buttonSizeOverlayColorForeground";
            this.buttonSizeOverlayColorForeground.Size = new System.Drawing.Size(29, 23);
            this.buttonSizeOverlayColorForeground.TabIndex = 23;
            this.buttonSizeOverlayColorForeground.Text = "...";
            this.buttonSizeOverlayColorForeground.UseVisualStyleBackColor = true;
            this.buttonSizeOverlayColorForeground.Click += new System.EventHandler(this.buttonSizeOverlayColorForeground_Click);
            // 
            // labelSizeOverlayColorForeground
            // 
            this.labelSizeOverlayColorForeground.AutoSize = true;
            this.labelSizeOverlayColorForeground.Location = new System.Drawing.Point(3, 39);
            this.labelSizeOverlayColorForeground.Name = "labelSizeOverlayColorForeground";
            this.labelSizeOverlayColorForeground.Size = new System.Drawing.Size(102, 13);
            this.labelSizeOverlayColorForeground.TabIndex = 21;
            this.labelSizeOverlayColorForeground.Text = "Size Overlay Color 1";
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
            // splitContainerScriptManager
            // 
            this.splitContainerScriptManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerScriptManager.Location = new System.Drawing.Point(3, 3);
            this.splitContainerScriptManager.Name = "splitContainerScriptManager";
            // 
            // splitContainerScriptManager.Panel1
            // 
            this.splitContainerScriptManager.Panel1.Controls.Add(this.listScripts);
            this.splitContainerScriptManager.Panel1.Controls.Add(this.panelUpDown);
            // 
            // splitContainerScriptManager.Panel2
            // 
            this.splitContainerScriptManager.Panel2.Controls.Add(this.panelScriptManager);
            this.splitContainerScriptManager.Size = new System.Drawing.Size(541, 389);
            this.splitContainerScriptManager.SplitterDistance = 180;
            this.splitContainerScriptManager.TabIndex = 1;
            // 
            // listScripts
            // 
            this.listScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listScripts.Location = new System.Drawing.Point(0, 0);
            this.listScripts.Name = "listScripts";
            this.listScripts.Size = new System.Drawing.Size(145, 379);
            this.listScripts.TabIndex = 0;
            this.listScripts.SelectedIndexChanged += new System.EventHandler(this.listScripts_SelectedIndexChanged);
            this.listScripts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listScripts_ItemCheck);
            this.listScripts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listScripts_MouseDown);
            this.listScripts.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listScripts_KeyUp);
            this.listScripts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listScripts_KeyDown);
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
            this.panelScriptManager.Visible = false;
            // 
            // labelScriptCreator
            // 
            this.labelScriptCreator.AutoSize = true;
            this.labelScriptCreator.Location = new System.Drawing.Point(3, 39);
            this.labelScriptCreator.Name = "labelScriptCreator";
            this.labelScriptCreator.Size = new System.Drawing.Size(44, 13);
            this.labelScriptCreator.TabIndex = 2;
            this.labelScriptCreator.Text = "Creator:";
            // 
            // labelScriptVersion
            // 
            this.labelScriptVersion.AutoSize = true;
            this.labelScriptVersion.Location = new System.Drawing.Point(3, 21);
            this.labelScriptVersion.Name = "labelScriptVersion";
            this.labelScriptVersion.Size = new System.Drawing.Size(45, 13);
            this.labelScriptVersion.TabIndex = 1;
            this.labelScriptVersion.Text = "Version:";
            // 
            // labelScriptName
            // 
            this.labelScriptName.AutoSize = true;
            this.labelScriptName.Location = new System.Drawing.Point(3, 3);
            this.labelScriptName.Name = "labelScriptName";
            this.labelScriptName.Size = new System.Drawing.Size(38, 13);
            this.labelScriptName.TabIndex = 0;
            this.labelScriptName.Text = "Name:";
            // 
            // clhScriptName
            // 
            this.clhScriptName.Text = "Name";
            this.clhScriptName.Width = 100;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonClose);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 421);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(555, 35);
            this.panelButtons.TabIndex = 22;
            // 
            // colorDialogSizeOverlay
            // 
            this.colorDialogSizeOverlay.AnyColor = true;
            // 
            // groupBoxSizeOverlay
            // 
            this.groupBoxSizeOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSizeOverlay.Controls.Add(this.checkBoxUseSizeOverlayColor2);
            this.groupBoxSizeOverlay.Controls.Add(this.checkBoxShowSizeOverlay);
            this.groupBoxSizeOverlay.Controls.Add(this.buttonSizeOverlayColorBackground);
            this.groupBoxSizeOverlay.Controls.Add(this.labelSizeOverlayColorForeground);
            this.groupBoxSizeOverlay.Controls.Add(this.textBoxSizeOverlayColorBackground);
            this.groupBoxSizeOverlay.Controls.Add(this.textBoxSizeOverlayColorForeground);
            this.groupBoxSizeOverlay.Controls.Add(this.labelSizeOverlayColorBackground);
            this.groupBoxSizeOverlay.Controls.Add(this.buttonSizeOverlayColorForeground);
            this.groupBoxSizeOverlay.Controls.Add(this.pictureBoxPreviewSizeOverlayColor);
            this.groupBoxSizeOverlay.Location = new System.Drawing.Point(6, 225);
            this.groupBoxSizeOverlay.Name = "groupBoxSizeOverlay";
            this.groupBoxSizeOverlay.Size = new System.Drawing.Size(535, 110);
            this.groupBoxSizeOverlay.TabIndex = 28;
            this.groupBoxSizeOverlay.TabStop = false;
            this.groupBoxSizeOverlay.Text = "Size Overlay";
            // 
            // groupBoxGeneralSettings
            // 
            this.groupBoxGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxBold);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDown3);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxAutoDownloadFullImage);
            this.groupBoxGeneralSettings.Controls.Add(this.label1);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxShowFolderPictures);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDownThumbnailWidth);
            this.groupBoxGeneralSettings.Controls.Add(this.textBox1);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDownThumbnailHeight);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxClose);
            this.groupBoxGeneralSettings.Controls.Add(this.label7);
            this.groupBoxGeneralSettings.Controls.Add(this.label2);
            this.groupBoxGeneralSettings.Controls.Add(this.label4);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxShowExistingArt);
            this.groupBoxGeneralSettings.Controls.Add(this.label3);
            this.groupBoxGeneralSettings.Location = new System.Drawing.Point(6, 6);
            this.groupBoxGeneralSettings.Name = "groupBoxGeneralSettings";
            this.groupBoxGeneralSettings.Size = new System.Drawing.Size(535, 213);
            this.groupBoxGeneralSettings.TabIndex = 29;
            this.groupBoxGeneralSettings.TabStop = false;
            this.groupBoxGeneralSettings.Text = "General Settings";
            // 
            // checkBoxBold
            // 
            this.checkBoxBold.AutoSize = true;
            this.checkBoxBold.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ExactMatchBold;
            this.checkBoxBold.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ExactMatchBold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBold.Location = new System.Drawing.Point(6, 19);
            this.checkBoxBold.Name = "checkBoxBold";
            this.checkBoxBold.Size = new System.Drawing.Size(160, 17);
            this.checkBoxBold.TabIndex = 13;
            this.checkBoxBold.Text = "Show exact matches in Bold";
            this.checkBoxBold.UseVisualStyleBackColor = true;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThreadCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown3.Location = new System.Drawing.Point(35, 134);
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
            // checkBoxAutoDownloadFullImage
            // 
            this.checkBoxAutoDownloadFullImage.AutoSize = true;
            this.checkBoxAutoDownloadFullImage.Checked = global::AlbumArtDownloader.Properties.Settings.Default.AutoDownloadFullImage;
            this.checkBoxAutoDownloadFullImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoDownloadFullImage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "AutoDownloadFullImage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxAutoDownloadFullImage.Location = new System.Drawing.Point(6, 88);
            this.checkBoxAutoDownloadFullImage.Name = "checkBoxAutoDownloadFullImage";
            this.checkBoxAutoDownloadFullImage.Size = new System.Drawing.Size(333, 17);
            this.checkBoxAutoDownloadFullImage.TabIndex = 20;
            this.checkBoxAutoDownloadFullImage.Text = "Automatically download the full image to determine size if required";
            this.checkBoxAutoDownloadFullImage.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowFolderPictures
            // 
            this.checkBoxShowFolderPictures.AutoSize = true;
            this.checkBoxShowFolderPictures.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowFolderPictures;
            this.checkBoxShowFolderPictures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowFolderPictures.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowFolderPictures", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowFolderPictures.Location = new System.Drawing.Point(6, 65);
            this.checkBoxShowFolderPictures.Name = "checkBoxShowFolderPictures";
            this.checkBoxShowFolderPictures.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowFolderPictures.TabIndex = 18;
            this.checkBoxShowFolderPictures.Text = "Show pictures in folder";
            this.checkBoxShowFolderPictures.UseVisualStyleBackColor = true;
            // 
            // numericUpDownThumbnailWidth
            // 
            this.numericUpDownThumbnailWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailWidth.Location = new System.Drawing.Point(83, 160);
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
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SaveFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(111, 186);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = global::AlbumArtDownloader.Properties.Settings.Default.SaveFileName;
            // 
            // numericUpDownThumbnailHeight
            // 
            this.numericUpDownThumbnailHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailHeight.Location = new System.Drawing.Point(154, 160);
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
            // checkBoxClose
            // 
            this.checkBoxClose.AutoSize = true;
            this.checkBoxClose.Checked = global::AlbumArtDownloader.Properties.Settings.Default.CloseAfterSaving;
            this.checkBoxClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "CloseAfterSaving", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxClose.Location = new System.Drawing.Point(6, 111);
            this.checkBoxClose.Name = "checkBoxClose";
            this.checkBoxClose.Size = new System.Drawing.Size(125, 17);
            this.checkBoxClose.TabIndex = 8;
            this.checkBoxClose.Text = "Close after saving art";
            this.checkBoxClose.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowExistingArt
            // 
            this.checkBoxShowExistingArt.AutoSize = true;
            this.checkBoxShowExistingArt.Checked = global::AlbumArtDownloader.Properties.Settings.Default.PreviewSavedArt;
            this.checkBoxShowExistingArt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowExistingArt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "PreviewSavedArt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowExistingArt.Location = new System.Drawing.Point(6, 42);
            this.checkBoxShowExistingArt.Name = "checkBoxShowExistingArt";
            this.checkBoxShowExistingArt.Size = new System.Drawing.Size(106, 17);
            this.checkBoxShowExistingArt.TabIndex = 3;
            this.checkBoxShowExistingArt.Text = "Show existing art";
            this.checkBoxShowExistingArt.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseSizeOverlayColor2
            // 
            this.checkBoxUseSizeOverlayColor2.AutoSize = true;
            this.checkBoxUseSizeOverlayColor2.Checked = global::AlbumArtDownloader.Properties.Settings.Default.UseSizeOverlayColor2;
            this.checkBoxUseSizeOverlayColor2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseSizeOverlayColor2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "UseSizeOverlayColor2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxUseSizeOverlayColor2.Location = new System.Drawing.Point(6, 88);
            this.checkBoxUseSizeOverlayColor2.Name = "checkBoxUseSizeOverlayColor2";
            this.checkBoxUseSizeOverlayColor2.Size = new System.Drawing.Size(143, 17);
            this.checkBoxUseSizeOverlayColor2.TabIndex = 28;
            this.checkBoxUseSizeOverlayColor2.Text = "Use Size Overlay Color 2";
            this.checkBoxUseSizeOverlayColor2.UseVisualStyleBackColor = true;
            this.checkBoxUseSizeOverlayColor2.CheckedChanged += new System.EventHandler(this.checkBoxUseSizeOverlayColor2_CheckedChanged);
            // 
            // checkBoxShowSizeOverlay
            // 
            this.checkBoxShowSizeOverlay.AutoSize = true;
            this.checkBoxShowSizeOverlay.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowSizeOverlay;
            this.checkBoxShowSizeOverlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowSizeOverlay.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowSizeOverlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowSizeOverlay.Location = new System.Drawing.Point(6, 19);
            this.checkBoxShowSizeOverlay.Name = "checkBoxShowSizeOverlay";
            this.checkBoxShowSizeOverlay.Size = new System.Drawing.Size(115, 17);
            this.checkBoxShowSizeOverlay.TabIndex = 19;
            this.checkBoxShowSizeOverlay.Text = "Show Size Overlay";
            this.checkBoxShowSizeOverlay.UseVisualStyleBackColor = true;
            // 
            // textBoxSizeOverlayColorBackground
            // 
            this.textBoxSizeOverlayColorBackground.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SizeOverlayColorBackground", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxSizeOverlayColorBackground.Location = new System.Drawing.Point(110, 62);
            this.textBoxSizeOverlayColorBackground.Name = "textBoxSizeOverlayColorBackground";
            this.textBoxSizeOverlayColorBackground.Size = new System.Drawing.Size(45, 20);
            this.textBoxSizeOverlayColorBackground.TabIndex = 26;
            this.textBoxSizeOverlayColorBackground.Text = global::AlbumArtDownloader.Properties.Settings.Default.SizeOverlayColorBackground;
            this.textBoxSizeOverlayColorBackground.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSizeOverlayColorBackground.TextChanged += new System.EventHandler(this.textBoxSizeOverlayColor_TextChanged);
            // 
            // textBoxSizeOverlayColorForeground
            // 
            this.textBoxSizeOverlayColorForeground.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SizeOverlayColorForeground", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxSizeOverlayColorForeground.Location = new System.Drawing.Point(110, 36);
            this.textBoxSizeOverlayColorForeground.Name = "textBoxSizeOverlayColorForeground";
            this.textBoxSizeOverlayColorForeground.Size = new System.Drawing.Size(45, 20);
            this.textBoxSizeOverlayColorForeground.TabIndex = 22;
            this.textBoxSizeOverlayColorForeground.Text = global::AlbumArtDownloader.Properties.Settings.Default.SizeOverlayColorForeground;
            this.textBoxSizeOverlayColorForeground.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSizeOverlayColorForeground.TextChanged += new System.EventHandler(this.textBoxSizeOverlayColor_TextChanged);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(555, 456);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.panelButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Validated += new System.EventHandler(this.SettingsDlg_Validated);
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreviewSizeOverlayColor)).EndInit();
            this.tabPageScriptManager.ResumeLayout(false);
            this.splitContainerScriptManager.Panel1.ResumeLayout(false);
            this.splitContainerScriptManager.Panel2.ResumeLayout(false);
            this.splitContainerScriptManager.ResumeLayout(false);
            this.panelUpDown.ResumeLayout(false);
            this.panelScriptManager.ResumeLayout(false);
            this.panelScriptManager.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.groupBoxSizeOverlay.ResumeLayout(false);
            this.groupBoxSizeOverlay.PerformLayout();
            this.groupBoxGeneralSettings.ResumeLayout(false);
            this.groupBoxGeneralSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).EndInit();
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
		private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageScriptManager;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.CheckBox checkBoxShowFolderPictures;
        private System.Windows.Forms.CheckedListBox listScripts;
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
        private System.Windows.Forms.ColorDialog colorDialogSizeOverlay;
        private System.Windows.Forms.Label labelSizeOverlayColorForeground;
        private System.Windows.Forms.Button buttonSizeOverlayColorForeground;
        private System.Windows.Forms.TextBox textBoxSizeOverlayColorForeground;
        private System.Windows.Forms.PictureBox pictureBoxPreviewSizeOverlayColor;
        private System.Windows.Forms.Button buttonSizeOverlayColorBackground;
        private System.Windows.Forms.TextBox textBoxSizeOverlayColorBackground;
        private System.Windows.Forms.Label labelSizeOverlayColorBackground;
        private System.Windows.Forms.GroupBox groupBoxSizeOverlay;
        private System.Windows.Forms.CheckBox checkBoxUseSizeOverlayColor2;
        private System.Windows.Forms.GroupBox groupBoxGeneralSettings;
    }
}