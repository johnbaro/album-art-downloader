﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlbumArtDownloader.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public double PanelWidth {
            get {
                return ((double)(this["PanelWidth"]));
            }
            set {
                this["PanelWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("77")]
        public double ThumbSize {
            get {
                return ((double)(this["ThumbSize"]));
            }
            set {
                this["ThumbSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%default%")]
        public string DefaultSavePath {
            get {
                return ((string)(this["DefaultSavePath"]));
            }
            set {
                this["DefaultSavePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<SortDescription xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <PropertyName>ResultName</PropertyName>
  <Direction>Ascending</Direction>
</SortDescription>")]
        public global::System.ComponentModel.SortDescription ResultsSorting {
            get {
                return ((global::System.ComponentModel.SortDescription)(this["ResultsSorting"]));
            }
            set {
                this["ResultsSorting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OpenResultsInNewWindow {
            get {
                return ((bool)(this["OpenResultsInNewWindow"]));
            }
            set {
                this["OpenResultsInNewWindow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int MinimumImageSize {
            get {
                return ((int)(this["MinimumImageSize"]));
            }
            set {
                this["MinimumImageSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection DefaultSavePathHistory {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["DefaultSavePathHistory"]));
            }
            set {
                this["DefaultSavePathHistory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Right")]
        public global::AlbumArtDownloader.Controls.InformationLocation InformationLocation {
            get {
                return ((global::AlbumArtDownloader.Controls.InformationLocation)(this["InformationLocation"]));
            }
            set {
                this["InformationLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("900")]
        public int MaximumImageSize {
            get {
                return ((int)(this["MaximumImageSize"]));
            }
            set {
                this["MaximumImageSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Never")]
        public global::AlbumArtDownloader.AutoDownloadFullSizeImages AutoDownloadFullSizeImages {
            get {
                return ((global::AlbumArtDownloader.AutoDownloadFullSizeImages)(this["AutoDownloadFullSizeImages"]));
            }
            set {
                this["AutoDownloadFullSizeImages"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseMinimumImageSize {
            get {
                return ((bool)(this["UseMinimumImageSize"]));
            }
            set {
                this["UseMinimumImageSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseMaximumImageSize {
            get {
                return ((bool)(this["UseMaximumImageSize"]));
            }
            set {
                this["UseMaximumImageSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoClose {
            get {
                return ((bool)(this["AutoClose"]));
            }
            set {
                this["AutoClose"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ApplicationVersion {
            get {
                return ((string)(this["ApplicationVersion"]));
            }
            set {
                this["ApplicationVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Net.NetworkCredential ProxyCredentials {
            get {
                return ((global::System.Net.NetworkCredential)(this["ProxyCredentials"]));
            }
            set {
                this["ProxyCredentials"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%default%")]
        public string FileBrowseRoot {
            get {
                return ((string)(this["FileBrowseRoot"]));
            }
            set {
                this["FileBrowseRoot"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FileBrowseSubfolders {
            get {
                return ((bool)(this["FileBrowseSubfolders"]));
            }
            set {
                this["FileBrowseSubfolders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Folder%preset%.%extension%|Cover%preset%.%extension%")]
        public string FileBrowseImagePath {
            get {
                return ((string)(this["FileBrowseImagePath"]));
            }
            set {
                this["FileBrowseImagePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection FileBrowseImagePathHistory {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FileBrowseImagePathHistory"]));
            }
            set {
                this["FileBrowseImagePathHistory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int NumberOfWindowsForQueue {
            get {
                return ((int)(this["NumberOfWindowsForQueue"]));
            }
            set {
                this["NumberOfWindowsForQueue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        public int EnqueueWarning {
            get {
                return ((int)(this["EnqueueWarning"]));
            }
            set {
                this["EnqueueWarning"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool FileBrowseUsePathPattern {
            get {
                return ((bool)(this["FileBrowseUsePathPattern"]));
            }
            set {
                this["FileBrowseUsePathPattern"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\%artist%\\%album%\\*")]
        public string FileBrowsePathPattern {
            get {
                return ((string)(this["FileBrowsePathPattern"]));
            }
            set {
                this["FileBrowsePathPattern"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public global::AlbumArtDownloader.Controls.Grouping ResultsGrouping {
            get {
                return ((global::AlbumArtDownloader.Controls.Grouping)(this["ResultsGrouping"]));
            }
            set {
                this["ResultsGrouping"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection FileBrowseFilePathPatternHistory {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FileBrowseFilePathPatternHistory"]));
            }
            set {
                this["FileBrowseFilePathPatternHistory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<GridSettings xmlns:xsi=\"http://www.w3.o" +
            "rg/2001/XMLSchema-instance\"\r\n                        xmlns:xsd=\"http://www.w3.or" +
            "g/2001/XMLSchema\"/>")]
        public global::AlbumArtDownloader.Controls.SortableListView.GridSettings FileBrowseResultsGrid {
            get {
                return ((global::AlbumArtDownloader.Controls.SortableListView.GridSettings)(this["FileBrowseResultsGrid"]));
            }
            set {
                this["FileBrowseResultsGrid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Visible")]
        public global::System.Windows.Visibility DonationsMenuItemVisibility {
            get {
                return ((global::System.Windows.Visibility)(this["DonationsMenuItemVisibility"]));
            }
            set {
                this["DonationsMenuItemVisibility"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<ArrayOfPreset xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Preset><Name>Default</Name><Value></Value></Preset>
  <Preset><Name>Front</Name><Value>-Front</Value></Preset>
  <Preset><Name>Back</Name><Value>-Back</Value></Preset>
  <Preset><Name>Inlay</Name><Value>-Inlay</Value></Preset>
  <Preset><Name>CD</Name><Value>-CD</Value></Preset>
</ArrayOfPreset>")]
        public AlbumArtDownloader.Preset[] Presets {
            get {
                return ((AlbumArtDownloader.Preset[])(this["Presets"]));
            }
            set {
                this["Presets"] = value;
            }
        }
    }
}
