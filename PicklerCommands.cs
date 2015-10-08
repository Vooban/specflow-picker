//-----------------------------------------------------------------------
// <copyright file="PicklerCommands.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace Pickler
{
    using System.Windows.Input;

    /// <summary>
    /// Used for helping with the command controllers.
    /// </summary>
    public static class PicklerCommand
    {
        /// <summary>
        /// WPFCommand for Collapsing Folds
        /// </summary>
        public static readonly RoutedUICommand FoldAll = new RoutedUICommand("Expand Folds", "ExpandFolds", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Expanding Folds.
        /// </summary>
        public static readonly RoutedUICommand UnfoldAll = new RoutedUICommand("Collapse Folds", "CollapseFolds", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Reformatting the current open file.
        /// </summary>
        public static readonly RoutedUICommand ReformatOpenFile = new RoutedUICommand("Reformat File", "ReformatOpenFile", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Reformatting all open files.
        /// </summary>
        public static readonly RoutedUICommand ReformatAll = new RoutedUICommand("Reformat All", "ReformatAll", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Saving the current open file.
        /// </summary>
        public static readonly RoutedUICommand SaveOpenFile = new RoutedUICommand("Save File", "SaveOpenFile", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for saving all open editors new or existing.
        /// </summary>
        public static readonly RoutedUICommand SaveAll = new RoutedUICommand("Save All", "SaveAll", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Saving the current window to new name
        /// </summary>
        public static readonly RoutedUICommand SaveAs = new RoutedUICommand("Save As", "SaveAs", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Closing the current editor
        /// </summary>
        public static readonly RoutedUICommand CloseFile = new RoutedUICommand("Close File", "CloseFile", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Closing All Open Files
        /// </summary>
        public static readonly RoutedUICommand CloseAll = new RoutedUICommand("Close All", "CloseAll", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Opening a New File
        /// </summary>
        public static readonly RoutedUICommand OpenNewFile = new RoutedUICommand("New File", "OpenNewFile", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Opening an existing file
        /// </summary>
        public static readonly RoutedUICommand OpenExistingFile = new RoutedUICommand("Open File", "OpenExistingFile", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Selecting a Most Recently Used entry.
        /// </summary>
        public static readonly RoutedUICommand SelectMru = new RoutedUICommand("Select MRU", "SelectMru", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Toggling Whitespace display in the editor
        /// </summary>
        public static readonly RoutedUICommand ToggleWhitespace = new RoutedUICommand("Toggle WhiteSpace", "ToggleWhiteSpace", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Toggling Word Wrap in editor.
        /// </summary>
        public static readonly RoutedUICommand ToggleWordWrap = new RoutedUICommand("Toggle Wrap", "ToggleWordWrap", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Changing the font
        /// </summary>
        public static readonly RoutedUICommand ChangeFont = new RoutedUICommand("Change Font", "ChangeFont", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Changing the font
        /// </summary>
        public static readonly RoutedUICommand Find = new RoutedUICommand("Find", "Find", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for Changing the font
        /// </summary>
        public static readonly RoutedUICommand Replace = new RoutedUICommand("Replace", "Replace", typeof(MainWindow));

        /// <summary>
        /// WPFCommand for About Windows
        /// </summary>
        public static readonly RoutedUICommand About = new RoutedUICommand("About", "About", typeof(MainWindow));
    }
}
