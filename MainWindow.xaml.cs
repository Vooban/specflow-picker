//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Media;
using System.Windows.Shapes;

namespace Pickler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fonts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Xml;

    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.AddIn;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Folding;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Local Working Variable
        /// </summary>
        private string currentworkingDir = string.Empty;

        /// <summary>
        /// Local Working Variable
        /// </summary>
        private string currentFileName = string.Empty;

        /// <summary>
        /// Local Working Variable
        /// </summary>
        private bool initialLoad = true;

        /// <summary>
        /// Local Working Variable
        /// </summary>
        private ConfigurationFile configuationSettings;

        /// <summary>
        /// Local Working Variable
        /// </summary>
        private FoldingManager foldMgr;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            string[] args = App.Parms;

            this.initialLoad = true;
            this.LoadConfig();

            this.initialLoad = false;

            if ((args != null) && (args.Length >= 1))
            {
                if (this.LoadFileView(args[0].ToString()) == true)
                {
                    this.MoveMruNode(args[0].ToString());
                }
            }
        }

        #region "Event Handlers"

        /// <summary>
        /// Handles the KeyUp event of the Edit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void Edit_KeyUp(object sender, KeyEventArgs e)
        {
            TextEditor editor = (TextEditor)((TabItem)tabCntl.Items[tabCntl.SelectedIndex]).Content;
            if (e.Key == Key.OemPipe)
            {
                var currCaret = editor.CaretOffset;
                this.ReformatFile(editor);
                var nextIndex = editor.Text.IndexOf("\n", currCaret);
                editor.CaretOffset = nextIndex - 1;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the Edit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Edit_TextChanged(object sender, EventArgs e)
        {
            //var tabItem = ((TabItem) tabCntl.Items[tabCntl.SelectedIndex]);
            var editor = (TextEditor)sender;
            var tabItem = (TabItem)editor.Parent;

            if (editor.Text.Length > 0)
            {
                editor.Tag = true;
                mSave.IsEnabled = true;
                tabItem.Header = string.Format("{0}*", tabItem.Header.ToString().Replace("*", ""));
                this.ApplySyntax(editor);
            }
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveConfig();
        }

        /// <summary>
        /// Handles the LocationChanged event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!this.initialLoad)
            {
                if (this.configuationSettings != null)
                {
                    this.configuationSettings.FormLeft = Convert.ToInt32(this.Left);
                    this.configuationSettings.FormTop = Convert.ToInt32(this.Top);
                }
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.initialLoad)
            {
                if (this.configuationSettings != null)
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        this.configuationSettings.FormHeight = Convert.ToInt32(this.Height);
                        this.configuationSettings.FormWidth = Convert.ToInt32(this.Width);
                    }

                    this.configuationSettings.FormState = this.WindowState.ToString();
                }
            }
        }

        #endregion

        #region "Helper Functions"

        /// <summary>
        /// Adds the blank line after table.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>List of string content with spaces inserted after tables if it didn't already have them.</returns>
        private List<string> AddBlankLineAfterTable(List<string> input)
        {
            var ret = new List<string>();
            var lineIsTable = false;
            foreach (string inputLine in input)
            {
                if (inputLine.Trim().StartsWith(@"|"))
                {
                    lineIsTable = true;
                }
                else if (lineIsTable)
                {
                    if (!string.IsNullOrWhiteSpace(inputLine.Trim()))
                    {
                        if (input.Count != ret.Count)
                        {
                            ret.Add(string.Empty);
                        }
                    }

                    lineIsTable = false;
                }

                ret.Add(inputLine);
            }

            return ret;
        }

        /// <summary>
        /// Applies the MRU settings.
        /// </summary>
        private void ApplyMruSettings()
        {
            mru1.Header = string.Format("_1 - {0}", this.configuationSettings.MruItem1.FilePath);
            mru2.Header = string.Format("_2 - {0}", this.configuationSettings.MruItem2.FilePath);
            mru3.Header = string.Format("_3 - {0}", this.configuationSettings.MruItem3.FilePath);
            mru4.Header = string.Format("_4 - {0}", this.configuationSettings.MruItem4.FilePath);

            mru1.Visibility = Visibility.Visible;
            mru1.Height = mNew.Height;
            mru2.Visibility = Visibility.Visible;
            mru2.Height = mNew.Height;
            mru3.Visibility = Visibility.Visible;
            mru3.Height = mNew.Height;
            mru4.Visibility = Visibility.Visible;
            mru4.Height = mNew.Height;

            if (mru1.Header.ToString().EndsWith("..."))
            {
                mru1.Visibility = Visibility.Hidden;
                mru1.Height = 0;
            }

            if (mru2.Header.ToString().EndsWith("..."))
            {
                mru2.Visibility = Visibility.Hidden;
                mru2.Height = 0;
            }

            if (mru3.Header.ToString().EndsWith("..."))
            {
                mru3.Visibility = Visibility.Hidden;
                mru3.Height = 0;
            }

            if (mru4.Header.ToString().EndsWith("..."))
            {
                mru4.Visibility = Visibility.Hidden;
                mru4.Height = 0;
            }

            mSep3.Visibility = Visibility.Hidden;
            mSep3.Height = 0;
            if (mru1.IsVisible || mru2.IsVisible || mru3.IsVisible || mru4.IsVisible)
            {
                mSep3.Visibility = Visibility.Visible;
                mSep3.Height = mNew.Height;
            }
        }

        /// <summary>
        /// Removes the folding.
        /// </summary>
        private void RemoveFolding()
        {
            FoldingManager.Uninstall(this.foldMgr);
        }

        /// <summary>
        /// Adds the folding.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void ApplyFolding(TextEditor editor)
        {
            if (foldMgr != null) FoldingManager.Uninstall(foldMgr);
            this.foldMgr = FoldingManager.Install(editor.TextArea);

            var startFold = -1;
            var endFold = -1;
            var folding = false;
            var currentLine = 0;
            var folds = new List<NewFolding>();
            var titles = new List<string>();
            var scenarioText = string.Empty;
            DocumentLine lastNonEmptyLine = editor.Document.Lines.FirstOrDefault();
            foreach (DocumentLine item in editor.Document.Lines)
            {
                string lineText = editor.Text.Substring(item.Offset, item.Length);
                if (!string.IsNullOrWhiteSpace(lineText))
                {
                    lastNonEmptyLine = item;
                }
                else
                {
                    if (folding)
                    {
                        if (lastNonEmptyLine != null) endFold = lastNonEmptyLine.EndOffset;
                        folds.Add(new NewFolding(startFold, endFold));
                        folding = false;
                    }
                }

                if (lineText.StartsWith(@"Scenario:") || lineText.StartsWith(@"Scenario Outline:") ||
                    folding && lineText.StartsWith(@"@"))
                {
                    startFold = item.Offset;
                    titles.Add(lineText);
                    folding = true;
                }
            }

            if (folding)
            {
                if (lastNonEmptyLine != null) endFold = lastNonEmptyLine.EndOffset;
                folds.Add(new NewFolding(startFold, endFold));
            }

            for (int i = 0; i < folds.Count; i++)
            {
                folds[i].Name = titles[i];
            }

            this.foldMgr.UpdateFoldings(folds, 0);
        }

        /// <summary>
        /// Applies the syntax.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void ApplySyntax(TextEditor editor)
        {
            var pathXml =
                System.IO.Path.Combine(
                    Path.GetDirectoryName(
                        Process.GetCurrentProcess().MainModule.FileName),
                        Properties.Settings.Default["SyntaxFile"].ToString());

            using (XmlTextReader reader = new XmlTextReader(pathXml))
            {
                editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        /// <summary>
        /// Creates the context menu for the editor windows.
        /// </summary>
        /// <returns>ContextMenu for binding to the editors.</returns>
        private ContextMenu CreateContextMenu()
        {
            var editContextMenu = new ContextMenu();

            editContextMenu.Items.Clear();

            editContextMenu = new ContextMenu();
            var mi = new MenuItem();
            mi.Header = "Cut";
            mi.Command = ApplicationCommands.Cut;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/cut.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Copy";
            mi.Command = ApplicationCommands.Copy;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/copy.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Paste";
            mi.Command = ApplicationCommands.Paste;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/paste.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            var sep = new Separator();
            editContextMenu.Items.Add(sep);

            mi = new MenuItem();
            mi.Header = "Reformat File";
            mi.Command = PicklerCommand.ReformatOpenFile;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/reformat.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Toggle Whitespace";
            mi.Command = PicklerCommand.ToggleWhitespace;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/whitespace.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Toggle Word Wrap";
            mi.Command = PicklerCommand.ToggleWordWrap;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/wordwrap.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            sep = new Separator();
            editContextMenu.Items.Add(sep);

            mi = new MenuItem();
            mi.Header = "Collapse All";
            mi.Command = PicklerCommand.FoldAll;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/fold.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "Expand All";
            mi.Command = PicklerCommand.UnfoldAll;
            mi.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("images/app-icons/unfold.png", UriKind.Relative)) };
            editContextMenu.Items.Add(mi);

            return editContextMenu;
        }

        /// <summary>
        /// Loads the editor config.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void LoadEditorConfig(TextEditor editor)
        {
            editor.Options.AllowScrollBelowDocument = true;
            editor.Options.IndentationSize = this.configuationSettings.TabWidth;
            editor.FontFamily = this.configuationSettings.EditorTypeface.FontFamily;
            editor.FontSize = this.configuationSettings.EditorFontSize;
            editor.FontWeight = this.configuationSettings.EditorTypeface.Weight;
            editor.FontStyle = this.configuationSettings.EditorTypeface.Style;
            editor.ShowLineNumbers = true;
            editor.LineNumbersForeground = this.configuationSettings.LineNumberForeBrush;
            editor.Background = this.configuationSettings.LineNumberBackBrush;
            FontSizeConverter fsc = new FontSizeConverter();
        }

        /// <summary>
        /// Files the is already open.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The index of the tab the file is currently open in or -1 if it is not open.</returns>
        private int FileIsAlreadyOpen(string fileName)
        {
            var ret = -1;

            if (tabCntl.Items.Count > 0)
            {
                foreach (TabItem t in tabCntl.Items)
                {
                    if ((t.ToolTip.ToString() == fileName) && (t.Header.ToString() == System.IO.Path.GetFileName(fileName)))
                    {
                        tabCntl.SelectedItem = t;
                        return tabCntl.SelectedIndex;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Loads the file view.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Flag inidcating whether the file load was successful or not.</returns>
        private bool LoadFileView(string fileName)
        {
            var newFile = false;
            try
            {
                var localFileName = fileName;
                if (localFileName == string.Empty)
                {
                    localFileName = "Unnamed";
                    newFile = true;
                }

                if (this.FileIsAlreadyOpen(fileName) >= 0)
                {
                    return true;
                }
                else
                {
                    this.currentFileName = System.IO.Path.GetFileName(localFileName);
                    this.currentworkingDir = System.IO.Path.GetDirectoryName(localFileName);
                    var newTabPage = new TabItem()
                    {
                        ToolTip = localFileName,
                        Visibility = System.Windows.Visibility.Visible,
                        Header = this.currentFileName
                    };

                    tabCntl.Items.Add(newTabPage);
                    tabCntl.SelectedItem = newTabPage;
                    tabCntl.Visibility = System.Windows.Visibility.Visible;

                    var newEditor = new TextEditor { Visibility = System.Windows.Visibility.Visible };
                    newEditor.Options.ShowSpaces = false;
                    newEditor.Options.ShowTabs = false;
                    newEditor.Options.CutCopyWholeLine = true;
                    newEditor.Options.ConvertTabsToSpaces = true;
                    newEditor.Options.EnableHyperlinks = false;
                    newEditor.Options.EnableEmailHyperlinks = false;
                    newEditor.Options.EnableTextDragDrop = false;
                    newEditor.Options.EnableRectangularSelection = true;
                    newEditor.ShowLineNumbers = true;

                    newEditor.Tag = false;
                    newEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    newEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    this.LoadEditorConfig(newEditor);
                    this.ApplySyntax(newEditor);

                    if (!newFile)
                    {
                        using (var featureStream = new StreamReader(fileName))
                        {
                            newEditor.Text = featureStream.ReadToEnd();
                        }

                        this.ReformatFile(newEditor);
                    }

                    this.ApplyFolding(newEditor);
                    newEditor.IsModified = false;
                    newEditor.TextChanged += new EventHandler(this.Edit_TextChanged);
                    newEditor.KeyUp += new KeyEventHandler(this.Edit_KeyUp);
                    newEditor.ContextMenu = this.CreateContextMenu();
                    newTabPage.Content = newEditor;

                    CaretReferencesRenderer cfr = new CaretReferencesRenderer(newEditor);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Loads the config.
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                var filePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                filePath = System.IO.Path.Combine(filePath, Globals.ConfigFile);
                this.configuationSettings = new ConfigurationFile(filePath);

                if (this.configuationSettings.FormState == "Maximized")
                {
                    this.WindowState = WindowState.Maximized;
                }
                else if (this.configuationSettings.FormState == "Minimized")
                {
                    WindowState = WindowState.Minimized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }

                this.Left = this.configuationSettings.FormLeft;
                this.Top = this.configuationSettings.FormTop;
                this.Width = this.configuationSettings.FormWidth;
                this.Height = this.configuationSettings.FormHeight;

                this.ApplyMruSettings();
            }
            catch (System.Exception exp)
            {
                MessageBox.Show(
                    this,
                    (string)this.Resources["ConfigErrorLoadingMessage"] + exp.Message,
                    (string)this.Resources["ConfigurationErrorHeading"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// Moves the MRU node.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void MoveMruNode(string fileName)
        {
            var finf = new FileInfo(fileName);
            var mru1 = this.configuationSettings.MruItem1;
            var mru2 = this.configuationSettings.MruItem2;
            var mru3 = this.configuationSettings.MruItem3;
            var mru4 = this.configuationSettings.MruItem4;

            if (mru1.FilePath == finf.FullName)
            {
                this.MoveMruNode(1);
            }
            else if (mru2.FilePath == finf.FullName)
            {
                this.MoveMruNode(2);
            }
            else if (mru3.FilePath == finf.FullName)
            {
                this.MoveMruNode(3);
            }
            else if (mru4.FilePath == finf.FullName)
            {
                this.MoveMruNode(4);
            }
            else
            {
                // Copy 3 data to 4 node
                mru4.Name = mru3.Name;
                mru4.FilePath = mru3.FilePath;

                // Copy 2 data to 3 node
                mru3.Name = mru2.Name;
                mru3.FilePath = mru2.FilePath;

                // Copy 1 data to 2 node
                mru2.Name = mru1.Name;
                mru2.FilePath = mru1.FilePath;

                // Copy saved data to 1
                mru1.Name = finf.Name;
                mru1.FilePath = finf.FullName;
            }

            this.ApplyMruSettings();
            this.configuationSettings.Save();
        }

        /// <summary>
        /// Moves the MRU node.
        /// </summary>
        /// <param name="nodeNDX">The node NDX.</param>
        private void MoveMruNode(int nodeNDX)
        {
            MruEntry temp;
            switch (nodeNDX)
            {
                case 2:
                    // Save Item
                    temp = this.configuationSettings.MruItem2;

                    // Copy 1 data to 2 node
                    this.configuationSettings.MruItem2 = this.configuationSettings.MruItem1;

                    // Copy saved data to 1
                    this.configuationSettings.MruItem1 = temp;

                    break;
                case 3:

                    // Save Item
                    temp = this.configuationSettings.MruItem3;

                    // Copy 2 data to 3 node
                    this.configuationSettings.MruItem3 = this.configuationSettings.MruItem2;

                    // Copy 1 data to 2 node
                    this.configuationSettings.MruItem2 = this.configuationSettings.MruItem1;

                    // Copy saved data to 1
                    this.configuationSettings.MruItem1 = temp;

                    break;
                case 4:

                    // Save Item
                    temp = this.configuationSettings.MruItem4;

                    // Copy 3 data to 4 node
                    this.configuationSettings.MruItem4 = this.configuationSettings.MruItem3;

                    // Copy 2 data to 3 node
                    this.configuationSettings.MruItem3 = this.configuationSettings.MruItem2;

                    // Copy 1 data to 2 node
                    this.configuationSettings.MruItem2 = this.configuationSettings.MruItem1;

                    // Copy saved data to 1
                    this.configuationSettings.MruItem1 = temp;

                    break;
            }
        }

        /// <summary>
        /// Folds the file.
        /// </summary>
        private void FoldFile()
        {
            foreach (FoldingSection fldSec in this.foldMgr.AllFoldings)
            {
                fldSec.IsFolded = true;
            }
        }

        /// <summary>
        /// Unfolds the file file.
        /// </summary>
        private void UnfoldFile()
        {
            foreach (FoldingSection foldSec in this.foldMgr.AllFoldings)
            {
                foldSec.IsFolded = false;
            }
        }

        /// <summary>
        /// Reformats the file.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void ReformatFile(TextEditor editor)
        {
            try
            {
                var input = (
                    from DocumentLine item in editor.Document.Lines
                    select this.RemoveWikiJunk(editor.Text.Substring(item.Offset, item.Length))).ToList();

                for (var i = 0; i < input.Count; i++)
                {
                    if (input[i].Trim().StartsWith("|"))
                    {
                        for (var j = i + 1; j <= input.Count; j++)
                        {
                            if (j == input.Count || input[j].Trim().StartsWith("|") == false)
                            {
                                var pipeTable = new List<string>();
                                for (int k = i; k < j; k++)
                                {
                                    var escapedBarLine = input[k].Trim().Replace(@"\|", @"~~");
                                    pipeTable.Add(escapedBarLine);
                                }

                                pipeTable = this.ReformatPipeTable(pipeTable, i + 1);

                                for (int k = i; k < j; k++)
                                {
                                    input[k] = pipeTable[0].Replace(@"~~", @"\|");
                                    pipeTable.RemoveAt(0);
                                }

                                i = j;
                                j = input.Count;
                            }
                        }
                    }
                }

                //input = this.AddBlankLineAfterTable(input);
                var inputText = new StringBuilder(string.Empty);
                for (int ndx=0; ndx<=input.Count-1; ndx++)
                {
                    if (ndx < input.Count - 1)
                    {
                        inputText.AppendLine(input[ndx]);
                    }
                    else
                    {
                        inputText.Append(input[ndx]);
                    }
                }

                editor.Text = inputText.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("FORMATTING EXCEPTION:\n\n{0}\n-----\nPress OK to continue", ex.ToString()),
                    @"Reformatting Error",
                    MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Reformats the pipe table.
        /// </summary>
        /// <param name="pipeTable">The pipe table.</param>
        /// <param name="startingLineFromMainFile">The starting line from main file.</param>
        /// <returns>List of strings with the pipe(|) characters aligned.</returns>
        private List<string> ReformatPipeTable(List<string> pipeTable, int startingLineFromMainFile)
        {
            var output = new List<string>();
            var data = new Dictionary<int, string>();
            var maxWidth = new Dictionary<int, int>();

            if (pipeTable.Any(row => row.StartsWith("|") == false || row.EndsWith("|") == false))
            {
                statusCurrent.Content = @"All table rows must begin and end with a |pipe| character.";
                return pipeTable;
            }

            var pipeCount = 0;
            foreach (string lne in pipeTable)
            {
                var lclCnt = lne.Count(f => f == '|');
                if ((lclCnt != pipeCount) && (pipeCount > 0))
                {
                    return pipeTable;
                }
                else
                {
                    pipeCount = lclCnt;
                }
            }

            // Number of "columns" is pipeCount - 1
            var colCount = pipeCount - 1;
            for (var i = 0; i < colCount; i++)
            {
                maxWidth.Add(i, 0);
            }

            // Iterate through all "row"s
            foreach (var s in pipeTable)
            {
                // Process:
                for (var i = 0; i < colCount; i++)
                {
                    // List<int> to store maximum field lengths for each "column" (using .Trim()'d data)
                    var value = s.Trim('|').Split('|')[i].Trim();
                    maxWidth[i] = Math.Max(maxWidth[i], value.Length);
                }
            }

            // Rewrite each "row" with all data padded to the max field length of that column with leading and trailing space for legibility
            foreach (var s in pipeTable)
            {
                var values = s.Trim('|').Split('|');

                if (values.Length == colCount)
                {
                    for (var i = 0; i < colCount; i++)
                    {
                        var temp = values[i].Trim() + new string(' ', maxWidth[i]);
                        values[i] = temp.Substring(0, maxWidth[i]);
                    }

                    output.Add(string.Format("| {0} |", string.Join(" | ", values)));
                }
                else
                {
                    output.Add(s);
                }
            }

            return output;
        }

        /// <summary>
        /// Removes the MRU node.
        /// </summary>
        /// <param name="fileName">The file name in the node.</param>
        private void RemoveMruNode(string fileName)
        {
            var finf = new FileInfo(fileName);
            var mru1 = this.configuationSettings.MruItem1;
            var mru2 = this.configuationSettings.MruItem2;
            var mru3 = this.configuationSettings.MruItem3;
            var mru4 = this.configuationSettings.MruItem4;

            if (mru1.FilePath == finf.FullName)
            {
                this.RemoveMruNode(1);
            }
            else if (mru2.FilePath == finf.FullName)
            {
                this.RemoveMruNode(2);
            }
            else if (mru3.FilePath == finf.FullName)
            {
                this.RemoveMruNode(3);
            }
            else if (mru4.FilePath == finf.FullName)
            {
                this.RemoveMruNode(4);
            }

            this.ApplyMruSettings();
            this.configuationSettings.Save();
        }

        /// <summary>
        /// Removes the MRU node.
        /// </summary>
        /// <param name="nodeNDX">The node NDX.</param>
        private void RemoveMruNode(int nodeNDX)
        {
            switch (nodeNDX)
            {
                case 1:

                    // Copy 2 data to 1 node
                    this.configuationSettings.MruItem1 = this.configuationSettings.MruItem2;

                    // Copy 3 data to 2 node
                    this.configuationSettings.MruItem2 = this.configuationSettings.MruItem3;

                    // Copy 4 data to 3 node
                    this.configuationSettings.MruItem3 = this.configuationSettings.MruItem4;

                    // Clear out 4
                    this.configuationSettings.MruItem4 = new MruEntry();

                    break;
                case 2:

                    // Copy 3 data to 2 node
                    this.configuationSettings.MruItem2 = this.configuationSettings.MruItem3;

                    // Copy 4 data to 3 node
                    this.configuationSettings.MruItem3 = this.configuationSettings.MruItem4;

                    // Clear out 4
                    this.configuationSettings.MruItem4 = new MruEntry();

                    break;
                case 3:

                    // Copy 4 data to 3 node
                    this.configuationSettings.MruItem3 = this.configuationSettings.MruItem4;

                    // Clear out 4
                    this.configuationSettings.MruItem4 = new MruEntry();

                    break;
                case 4:

                    // Clear out 4
                    this.configuationSettings.MruItem4 = new MruEntry();

                    break;
            }
        }

        /// <summary>
        /// Removes the wiki junk.
        /// </summary>
        /// <param name="data">The data line passed to remove wiki junk.</param>
        /// <returns>String with wiki markup and formatting tags removed.</returns>
        private string RemoveWikiJunk(string data)
        {
            data = data.Trim();
            data = data.Replace(@"\\", string.Empty).Replace(@"&nbsp;", @" ").Replace(@"\#", @"#").Replace(@"\!", @"!").Replace(@"||", @"|").Replace(@"/|", @"|");

            var pattern = @"\{color:#\w*\}|\{color\}";
            var rgx = new Regex(pattern);
            data = rgx.Replace(data, string.Empty);

            pattern = "^/-.*/-$";
            rgx = new Regex(pattern);
            data = rgx.Replace(data, string.Empty);

            return data;
        }

        /// <summary>
        /// Saves the config.
        /// </summary>
        private void SaveConfig()
        {
            this.Cursor = Cursors.Wait;
            this.configuationSettings.Save();
            this.Cursor = null;
        }

        /// <summary>
        /// Saves the file as.
        /// </summary>
        private void SaveFileAs()
        {
            var sfa = new SaveFileDialog();
            sfa.Filter = @"Feature Files (*.feature)|*.feature|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sfa.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (!string.IsNullOrEmpty(this.currentworkingDir))
            {
                sfa.InitialDirectory = this.currentworkingDir;
                sfa.FileName = this.currentFileName;
            }

            if (sfa.ShowDialog() == true)
            {
                var filePage = (TabItem)tabCntl.SelectedItem;
                var editor = (TextEditor)filePage.Content;
                this.SaveFile(sfa.FileName, editor);
                filePage.Header = Path.GetFileName(sfa.FileName);
            }
        }

        /// <summary>
        /// Verifies the lose changes.
        /// </summary>
        /// <returns>Flag indicating whether program should loses changes before continuing.</returns>
        private bool VerifyLoseChanges()
        {
            if (tabCntl.Items.Count > 0)
            {
                var editor = (TextEditor)((TabItem)tabCntl.Items[tabCntl.SelectedIndex]).Content;
                if ((editor != null) && (editor.CanUndo && (mSave.Visibility == Visibility.Visible)))
                {
                    var boxResult =
                        MessageBox.Show(
                            this,
                            @"File has changed.  Do you wish to save before proceeding",
                            @"Save Changes",
                            MessageBoxButton.YesNo);
                    if (boxResult == MessageBoxResult.Yes)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region "Command Helpers"

        /// <summary>
        /// Exits the app.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Creates the new file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CreateNewFile(object sender, RoutedEventArgs e)
        {
            this.LoadFileView(string.Empty);
        }

        /// <summary>
        /// Existings the open file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExistingOpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = this.configuationSettings.Filters;
            if (Directory.Exists(this.currentworkingDir))
            {
                ofd.InitialDirectory = this.currentworkingDir;
            }
            else
            {
                ofd.InitialDirectory = "C:\\";
            }

            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == true)
            {
                if ((ofd.FileName != null) && File.Exists(ofd.FileName))
                {
                    this.LoadFileView(ofd.FileName);
                }

                this.MoveMruNode(ofd.FileName);
                this.ApplyMruSettings();
            }            
        }

        /// <summary>
        /// Saves all editors.
        /// </summary>
        private void SaveAllEditors()
        {
            foreach (TabItem ti in tabCntl.Items)
            {
                SaveFile(ti);
            }
        }

        private string SaveFile(TabItem filePage)
        {
            var fileEditor = (TextEditor)filePage.Content;
            var fileName = filePage.ToolTip.ToString();

            this.ReformatFile(fileEditor);
            this.ApplySyntax(fileEditor);
            this.ApplyFolding(fileEditor);
            if (string.IsNullOrEmpty(fileName) || fileName == "Unnamed")
            {
                var sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == true)
                {
                    fileName = sfd.FileName;
                    filePage.ToolTip = fileName;
                }
                else
                    return fileName;
            }
            
            filePage.Header = Path.GetFileName(fileName);
            fileEditor.Tag = false;

            this.SaveFile(fileName, fileEditor);

            return fileName;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            var filePage = (TabItem)tabCntl.SelectedItem;

            this.currentFileName = SaveFile(filePage);
            this.MoveMruNode(this.currentFileName);

            ((TextEditor)filePage.Content).Tag = false;
            mSave.IsEnabled = false;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="editor">The editor.</param>
        private void SaveFile(string filePath, TextEditor editor)
        {
            TextWriter tw = new StreamWriter(filePath, false);
            foreach (DocumentLine item in editor.Document.Lines)
            {
                tw.WriteLine(editor.Text.Substring(item.Offset, item.Length));
            }

            tw.Close();
        }

        /// <summary>
        /// Saves the file as.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveFileAs(object sender, RoutedEventArgs e)
        {
            var filePage = (TabItem)tabCntl.SelectedItem;
            var fileEditor = (TextEditor)filePage.Content;
            this.ReformatFile(fileEditor);
            this.ApplySyntax(fileEditor);
            this.ApplyFolding(fileEditor);
            this.SaveFileAs();
            fileEditor.Tag = false;
            mSave.IsEnabled = false;
        }

        /// <summary>
        /// Saves all files.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveAllFiles(object sender, RoutedEventArgs e)
        {
            var holdSelected = tabCntl.SelectedIndex;
            this.SaveAllEditors();
            tabCntl.SelectedIndex = holdSelected;
        }

        /// <summary>
        /// Closes the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseCurrentFile(object sender, RoutedEventArgs e)
        {
            if (this.VerifyLoseChanges())
            {
                Cursor = Cursors.Wait;
                var currentTab = tabCntl.SelectedItem;
                tabCntl.Items.Remove(currentTab);
                statusCurrent.Content = @"Closing File.";
                if (tabCntl.Items.Count == 0)
                {
                    this.currentFileName = string.Empty;
                    tabCntl.Visibility = Visibility.Visible;
                }

                Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Closes all file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseAllFile(object sender, RoutedEventArgs e)
        {
            while (tabCntl.Items.Count > 0)
            {
                tabCntl.SelectedIndex = 0;
                this.CloseCurrentFile(sender, e);
            }
        }

        /// <summary>
        /// Selects the MR u1.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectMRU1(object sender, RoutedEventArgs e)
        {
            var startNdx = mru1.Header.ToString().IndexOf(":") - 1;
            var nodePath = mru1.Header.ToString().Substring(startNdx);
            if (File.Exists(nodePath))
            {
                this.LoadFileView(nodePath);
                this.MoveMruNode(nodePath);
            }
            else
            {
                this.RemoveMruNode(nodePath);
            }
        }

        /// <summary>
        /// Selects the MR u2.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectMRU2(object sender, RoutedEventArgs e)
        {
            var startNdx = mru2.Header.ToString().IndexOf(":") - 1;
            var nodePath = mru2.Header.ToString().Substring(startNdx);
            if (File.Exists(nodePath))
            {
                this.LoadFileView(nodePath);
                this.MoveMruNode(nodePath);
            }
            else
            {
                this.RemoveMruNode(nodePath);
            }
        }

        /// <summary>
        /// Selects the MR u3.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectMRU3(object sender, RoutedEventArgs e)
        {
            var startNdx = mru3.Header.ToString().IndexOf(":") - 1;
            var nodePath = mru3.Header.ToString().Substring(startNdx);
            if (File.Exists(nodePath))
            {
                this.LoadFileView(nodePath);
                this.MoveMruNode(nodePath);
            }
            else
            {
                this.RemoveMruNode(nodePath);
            }
        }

        /// <summary>
        /// Selects the MR u4.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectMRU4(object sender, RoutedEventArgs e)
        {
            var startNdx = mru4.Header.ToString().IndexOf(":") - 1;
            var nodePath = mru4.Header.ToString().Substring(startNdx);
            if (File.Exists(nodePath))
            {
                this.LoadFileView(nodePath);
                this.MoveMruNode(nodePath);
            }
            else
            {
                this.RemoveMruNode(nodePath);
            }
        }

        /// <summary>
        /// Expands all folds.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExpandAllFolds(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;
            this.FoldFile();
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Collapses all folds.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CollapseAllFolds(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;
            this.UnfoldFile();
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Reformats the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ReformatFile(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;
            var currentOffset = currentEditor.CaretOffset;
            this.ReformatFile(currentEditor);
            this.RemoveFolding();
            this.ApplyFolding(currentEditor);
            currentEditor.CaretOffset = currentOffset < currentEditor.Text.Length ? currentOffset : 0; ;
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Reformats all files.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ReformatAllFiles(object sender, RoutedEventArgs e)
        {
            var currentTI = (TabItem)tabCntl.SelectedItem;
            var currentDOC = (TextEditor)currentTI.Content;
            var currentCRT = currentDOC.CaretOffset;
            foreach (TabItem ti in tabCntl.Items)
            {
                tabCntl.SelectedItem = ti;
                this.ReformatFile(sender, e);
            }

            tabCntl.SelectedItem = currentTI;
            currentDOC.CaretOffset = currentCRT < currentDOC.Text.Length ? currentCRT : 0; ;
        }

        /// <summary>
        /// Toggles the space.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleSpace(object sender, RoutedEventArgs e)
        {
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;            
            currentEditor.TextArea.Options.ShowTabs = !currentEditor.TextArea.Options.ShowTabs;
            currentEditor.TextArea.Options.ShowSpaces = !currentEditor.TextArea.Options.ShowSpaces;
            currentEditor.TextArea.Options.ShowBoxForControlCharacters = !currentEditor.TextArea.Options.ShowBoxForControlCharacters;
        }

        /// <summary>
        /// Toggles the wrap.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleWrap(object sender, RoutedEventArgs e)
        {
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;
            currentEditor.WordWrap = !currentEditor.WordWrap;
        }

        /// <summary>
        /// Changes the font.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangeFont(object sender, RoutedEventArgs e)
        {
            var currentTab = (TabItem)tabCntl.SelectedItem;
            var currentEditor = (TextEditor)currentTab.Content;
            System.Windows.Forms.FontDialog fdlg = new System.Windows.Forms.FontDialog();
            fdlg.AllowVectorFonts = false;
            fdlg.AllowVerticalFonts = false;

            var ft = new FontTranslate(
                currentEditor.FontFamily,
                currentEditor.FontSize,
                currentEditor.FontStyle,
                currentEditor.FontWeight);

            fdlg.Font = ft.Font;

            if (fdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ft = new FontTranslate(fdlg.Font);
                currentEditor.FontSize = ft.FontSize;
                currentEditor.FontFamily = ft.FontFamily;
                currentEditor.FontWeight = ft.FontWeight;
                currentEditor.FontStyle = ft.FontStyle;

                this.configuationSettings.EditorTypeface = ft.Typeface;
                this.configuationSettings.EditorFontSize = (float)ft.FontSize;
                this.configuationSettings.SaveFont();
            }
        }

        /// <summary>
        /// Abouts the open.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AboutOpen(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            Nullable<bool> dialogResult = aboutWindow.ShowDialog();
        }

        #endregion

        #region "Command Handlers"

        /// <summary>
        /// Determines whether this instance [can fold unfold] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanFoldUnfold(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Expands the folds.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExpandFolds(object sender, ExecutedRoutedEventArgs e)
        {
            this.ExpandAllFolds(sender, e);
        }

        /// <summary>
        /// Collapses the folds.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CollapseFolds(object sender, ExecutedRoutedEventArgs e)
        {
            this.CollapseAllFolds(sender, e);
        }

        /// <summary>
        /// Determines whether this instance can reformat the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanReformat(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Reformats the open file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ReformatOpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            this.ReformatFile(sender, e);
        }

        /// <summary>
        /// Reformats all.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ReformatAll(object sender, ExecutedRoutedEventArgs e)
        {
            this.ReformatAllFiles(sender, e);
        }

        /// <summary>
        /// Determines whether this instance can save the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Saves the open file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void SaveOpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveFile(sender, e);
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void SaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveAllFiles(sender, e);
        }

        /// <summary>
        /// Saves the open editor as a new file name.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveFileAs(sender, e);
        }

        /// <summary>
        /// Determines whether this instance can close the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Closes the file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CloseFile(object sender, ExecutedRoutedEventArgs e)
        {
            this.CloseCurrentFile(sender, e);
        }

        /// <summary>
        /// Closes all.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CloseAll(object sender, ExecutedRoutedEventArgs e)
        {
            this.CloseAllFile(sender, e);
        }

        /// <summary>
        /// Determines whether this instance can toggle the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanToggle(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Toggles the whitespace.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleWhitespace(object sender, ExecutedRoutedEventArgs e)
        {
            this.ToggleSpace(sender, e);
        }

        /// <summary>
        /// Toggles the word wrap.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleWordWrap(object sender, ExecutedRoutedEventArgs e)
        {
            this.ToggleWrap(sender, e);
        }

        /// <summary>
        /// Determines whether this instance [can select font] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanSelectFont(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.AnyFileOpen();
            e.Handled = true;
        }

        /// <summary>
        /// Selects the font.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void SelectFont(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeFont(sender, e);
        }

        /// <summary>
        /// Anies the file open.
        /// </summary>
        /// <returns>Flag indicating if ant file is open.</returns>
        private bool AnyFileOpen()
        {
            return this.tabCntl.Items.Count > 0;
        }

        /// <summary>
        /// MRUs the visible.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void MRUVisible(object sender, CanExecuteRoutedEventArgs e)
        {
            var mruNumber = Convert.ToInt32(e.Parameter.ToString());
            switch (mruNumber)
            {
                case 1:
                    e.CanExecute = mru1.IsVisible;
                    break;
                case 2:
                    e.CanExecute = mru2.IsVisible;
                    break;
                case 3:
                    e.CanExecute = mru3.IsVisible;
                    break;
                case 4:
                    e.CanExecute = mru4.IsVisible;
                    break;
                default:
                    e.CanExecute = false;
                    break;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Selects the MRU.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void SelectMru(object sender, ExecutedRoutedEventArgs e)
        {
            var mruNumber = Convert.ToInt32(e.Parameter.ToString());
            switch (mruNumber)
            {
                case 1:
                    this.SelectMRU1(sender, e);
                    break;
                case 2:
                    this.SelectMRU2(sender, e);
                    break;
                case 3:
                    this.SelectMRU3(sender, e);
                    break;
                case 4:
                    this.SelectMRU4(sender, e);
                    break;
            }
        }

        /// <summary>
        /// Opens the existing file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OpenExistingFile(object sender, ExecutedRoutedEventArgs e)
        {
            this.ExistingOpenFile(sender, e);
        }

        /// <summary>
        /// Opens the new file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OpenNewFile(object sender, ExecutedRoutedEventArgs e)
        {
            this.CreateNewFile(sender, e);
        }

        /// <summary>
        /// Shows the about.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ShowAbout(object sender, ExecutedRoutedEventArgs e)
        {
            this.AboutOpen(sender, e);
        }

        #endregion

        private void tabCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var editor = (TextEditor)((TabItem)tabCntl.Items[tabCntl.SelectedIndex]).Content;
            mSave.IsEnabled = editor!= null && editor.Tag != null && (bool) editor.Tag;
        }


    }
}
