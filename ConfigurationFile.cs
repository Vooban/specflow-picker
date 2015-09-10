//-----------------------------------------------------------------------
// <copyright file="ConfigurationFile.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace Pickler
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Abstraction class for the configuration file to localize file access and ease usage.
    /// </summary>
    public class ConfigurationFile
    {
        /// <summary>
        /// Local varible used for manipulating the files.
        /// </summary>
        private string fileName = string.Empty;   

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFile"/> class.
        /// </summary>
        /// <param name="configurationFileName">Name of the configuration file.</param>
        public ConfigurationFile(string configurationFileName)
        {
            this.fileName = configurationFileName;
            this.LoadFromConfigurationFile();
        }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>
        /// The editor.
        /// </value>
        public string Editor { get; set; }

        /// <summary>
        /// Gets or sets the syntax file.
        /// </summary>
        /// <value>
        /// The syntax file.
        /// </value>
        public string SyntaxFile { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public string Filters { get; set; }

        /// <summary>
        /// Gets or sets the editor font.
        /// </summary>
        /// <value>
        /// The editor font.
        /// </value>
        public Typeface EditorTypeface { get; set; }

        /// <summary>
        /// Gets or sets the editor font size.
        /// </summary>
        /// <value>
        /// The editor font size.
        /// </value>
        public float EditorFontSize { get; set; }

        /// <summary>
        /// Gets or sets the state of the form.
        /// </summary>
        /// <value>
        /// The state of the form.
        /// </value>
        public string FormState { get; set; }

        /// <summary>
        /// Gets or sets the form left.
        /// </summary>
        /// <value>
        /// The form left.
        /// </value>
        public int FormLeft { get; set; }

        /// <summary>
        /// Gets or sets the form top.
        /// </summary>
        /// <value>
        /// The form top.
        /// </value>
        public int FormTop { get; set; }

        /// <summary>
        /// Gets or sets the width of the form.
        /// </summary>
        /// <value>
        /// The width of the form.
        /// </value>
        public int FormWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the form.
        /// </summary>
        /// <value>
        /// The height of the form.
        /// </value>
        public int FormHeight { get; set; }

        /// <summary>
        /// Gets or sets the MRU item1.
        /// </summary>
        /// <value>
        /// The MRU item1.
        /// </value>
        public MruEntry MruItem1 { get; set; }

        /// <summary>
        /// Gets or sets the MRU item2.
        /// </summary>
        /// <value>
        /// The MRU item2.
        /// </value>
        public MruEntry MruItem2 { get; set; }

        /// <summary>
        /// Gets or sets the MRU item3.
        /// </summary>
        /// <value>
        /// The MRU item3.
        /// </value>
        public MruEntry MruItem3 { get; set; }

        /// <summary>
        /// Gets or sets the MRU item4.
        /// </summary>
        /// <value>
        /// The MRU item4.
        /// </value>
        public MruEntry MruItem4 { get; set; }

        /// <summary>
        /// Gets or sets the width of the tab.
        /// </summary>
        /// <value>
        /// The width of the tab.
        /// </value>
        public int TabWidth { get; set; }

        /// <summary>
        /// Gets or sets the color of the line number back.
        /// </summary>
        /// <value>
        /// The color of the line number back.
        /// </value>
        public Brush LineNumberBackBrush { get; set; }

        /// <summary>
        /// Gets or sets the color of the line number fore.
        /// </summary>
        /// <value>
        /// The color of the line number fore.
        /// </value>
        public Brush LineNumberForeBrush { get; set; }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            var configXmlDoc = new XmlDocument();
            configXmlDoc.Load(this.fileName);

            var configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormState));
            configItem.InnerText = this.FormState;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.Editor));
            configItem.InnerText = this.Editor;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormLeft));
            configItem.InnerText = this.FormLeft.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormTop));
            configItem.InnerText = this.FormTop.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormWidth));
            configItem.InnerText = this.FormWidth.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormHeight));
            configItem.InnerText = this.FormHeight.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Name));
            configItem.InnerText = this.EditorTypeface.FontFamily.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Size));
            configItem.InnerText = this.EditorFontSize.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Italic));
            configItem.InnerText = (this.EditorTypeface.Style == FontStyles.Italic).ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Bold));
            configItem.InnerText = (this.EditorTypeface.Weight == FontWeights.Bold).ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Underline));
            configItem.InnerText = false.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem1));
            configItem.Attributes[Tags.Path].InnerText = this.MruItem1.FilePath;
            configItem.Attributes[Tags.Name].InnerText = Path.GetFileName(this.MruItem1.FilePath);

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem2));
            configItem.Attributes[Tags.Path].InnerText = this.MruItem2.FilePath;
            configItem.Attributes[Tags.Name].InnerText = Path.GetFileName(this.MruItem2.FilePath);

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem3));
            configItem.Attributes[Tags.Path].InnerText = this.MruItem3.FilePath;
            configItem.Attributes[Tags.Name].InnerText = Path.GetFileName(this.MruItem3.FilePath);

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem4));
            configItem.Attributes[Tags.Path].InnerText = this.MruItem4.FilePath;
            configItem.Attributes[Tags.Name].InnerText = Path.GetFileName(this.MruItem4.FilePath);

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.LineNumberForeColor));
            configItem.InnerText = ((SolidColorBrush)this.LineNumberForeBrush).Color.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.LineNumberBackColor));
            configItem.InnerText = ((SolidColorBrush)this.LineNumberBackBrush).Color.ToString();

            // Save File to Disk
            configXmlDoc.Save(this.fileName);
            configXmlDoc = null; /* Dispose? */
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void SaveFont()
        {
            var configXmlDoc = new XmlDocument();
            configXmlDoc.Load(this.fileName);

            var configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Name));
            configItem.InnerText = this.EditorTypeface.FontFamily.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Size));
            configItem.InnerText = this.EditorFontSize.ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Italic));
            configItem.InnerText = (this.EditorTypeface.Style == FontStyles.Italic).ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Bold));
            configItem.InnerText = (this.EditorTypeface.Weight == FontWeights.Bold).ToString();

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Underline));
            configItem.InnerText = false.ToString();

            // Save File to Disk
            configXmlDoc.Save(this.fileName);
        }

        /// <summary>
        /// Gets the file filters.
        /// </summary>
        /// <param name="filterNodes">The filter nodes.</param>
        /// <returns>
        /// File filter string.
        /// </returns>
        private static string GetFileFilters(XmlNodeList filterNodes)
        {
            try
            {
                var retFilters = string.Empty;
                if (filterNodes != null)
                {
                    foreach (XmlNode filterItem in filterNodes)
                    {
                        if (retFilters == string.Empty)
                        {
                            retFilters = filterItem.InnerText;
                        }
                        else
                        {
                            retFilters = retFilters + "|" + filterItem.InnerText;
                        }
                    }
                }

                return retFilters;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Loads object from configuration file.
        /// </summary>
        private void LoadFromConfigurationFile()
        {
            // Create config file if it is missing
            if (!System.IO.File.Exists(this.fileName))
            {
                this.CreateDefaultConfigFile();
            }

            var configXmlDoc = new XmlDocument();
            configXmlDoc.Load(this.fileName);

            XmlNode configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormState));
            this.FormState = (configItem != null) ? configItem.InnerText : string.Empty;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.Editor));
            this.Editor = (configItem != null) ? configItem.InnerText : string.Empty;

            this.Filters = GetFileFilters(configXmlDoc.SelectNodes(string.Format(@"{0}/{1}/{2}", Tags.Root, Tags.Filters, Tags.Filter)));

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormLeft));
            this.FormLeft = (configItem != null) ? Convert.ToInt32(configItem.InnerText) : 0;
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormTop));
            this.FormTop = (configItem != null) ? Convert.ToInt32(configItem.InnerText) : 0;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormWidth));
            this.FormWidth = (configItem != null) ? Convert.ToInt32(configItem.InnerText) : 0;
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.FormHeight));
            this.FormHeight = (configItem != null) ? Convert.ToInt32(configItem.InnerText) : 0;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem1));
            this.MruItem1 = new MruEntry((configItem != null) ? configItem.Attributes[Tags.Path].InnerText : @"...");
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem2));
            this.MruItem2 = new MruEntry((configItem != null) ? configItem.Attributes[Tags.Path].InnerText : @"...");
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem3));
            this.MruItem3 = new MruEntry((configItem != null) ? configItem.Attributes[Tags.Path].InnerText : @"...");
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.MruItem4));
            this.MruItem4 = new MruEntry((configItem != null) ? configItem.Attributes[Tags.Path].InnerText : @"...");

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.TabWidth));
            this.TabWidth = (configItem != null) ? Convert.ToInt32(configItem.InnerText) : 0;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}", Tags.Root, Tags.Syntaxfile));
            this.SyntaxFile = (configItem != null) ? configItem.InnerText : string.Empty;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Size));
            this.EditorFontSize = (configItem != null) ? float.Parse(configItem.InnerText) : 0.0F;

            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Name));
            var fontName = (configItem != null) ? configItem.InnerText : string.Empty;
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Bold));
            var fontBold = (configItem != null) ? (configItem.InnerText == "True") : false;
            configItem = configXmlDoc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}", Tags.Root, Tags.Fonts, Tags.FileFont, Tags.Italic));
            var fontItalic = (configItem != null) ? (configItem.InnerText == "True") : false;

            FontWeight fontStyleBold = FontWeights.Normal;
            if (fontBold)
            {
                fontStyleBold = FontWeights.Bold;
            }

            FontStyle fontStyleItalic = FontStyles.Normal;
            if (fontItalic)
            {
                fontStyleItalic = FontStyles.Italic;
            }

            FontFamily fontFamilyName = new FontFamily(fontName);
            this.EditorTypeface = new Typeface(fontFamilyName, fontStyleItalic, fontStyleBold, FontStretches.Normal);

            var bc = new BrushConverter(); 
            configItem = configXmlDoc.SelectSingleNode(Tags.LineNumberForeColor);
            this.LineNumberForeBrush = (configItem != null) ? (Brush)bc.ConvertFromString(configItem.InnerText) : Brushes.Black;

            configItem = configXmlDoc.SelectSingleNode(Tags.LineNumberBackColor);
            this.LineNumberBackBrush = (configItem != null) ? (Brush)bc.ConvertFromString(configItem.InnerText) : Brushes.White;

            configXmlDoc = null;
        }

        /// <summary>
        /// Creates the default config file.
        /// </summary>
        /// <returns>Success Flag</returns>
        private bool CreateDefaultConfigFile()
        {
            try
            {
                var f = new FileInfo(this.fileName);
                var w = f.CreateText();
                w.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                w.WriteLine("<" + Tags.Root + ">");
                w.WriteLine("	<" + Tags.Editor + " />");
                w.WriteLine("	<" + Tags.Syntaxfile + ">Gherkin.syn</" + Tags.Syntaxfile + ">");
                w.WriteLine("	<" + Tags.Filters + ">");
                w.WriteLine("		<" + Tags.Filter + ">Feature File (*.feature)|*.feature</" + Tags.Filter + ">");
                w.WriteLine("		<" + Tags.Filter + ">All files (*.*)|*.*</" + Tags.Filter + ">");
                w.WriteLine("	</" + Tags.Filters + ">");
                w.WriteLine("	<" + Tags.Fonts + ">");
                w.WriteLine("		<" + Tags.FileFont + ">");
                w.WriteLine("			<" + Tags.Name + ">Consolas</" + Tags.Name + ">");
                w.WriteLine("			<" + Tags.Bold + ">False</" + Tags.Bold + ">");
                w.WriteLine("			<" + Tags.Italic + ">False</" + Tags.Italic + ">");
                w.WriteLine("			<" + Tags.Size + ">12</" + Tags.Size + ">");
                w.WriteLine("			<" + Tags.Underline + ">False</" + Tags.Underline + ">");
                w.WriteLine("		</" + Tags.FileFont + ">");
                w.WriteLine("	</" + Tags.Fonts + ">");
                w.WriteLine("	<" + Tags.LineNumberForeColor + ">Black</" + Tags.LineNumberForeColor + ">");
                w.WriteLine("	<" + Tags.LineNumberBackColor + ">White</" + Tags.LineNumberBackColor + ">");
                w.WriteLine("	<" + Tags.FormState + ">Normal</" + Tags.FormState + ">");
                w.WriteLine("	<" + Tags.FormLeft + ">50</" + Tags.FormLeft + ">");
                w.WriteLine("	<" + Tags.FormTop + ">50</" + Tags.FormTop + ">");
                w.WriteLine("	<" + Tags.FormWidth + ">800</" + Tags.FormWidth + ">");
                w.WriteLine("	<" + Tags.FormHeight + ">600</" + Tags.FormHeight + ">");
                w.WriteLine("	<" + Tags.MruItem1 + " name=\"...\" path=\"...\" />");
                w.WriteLine("	<" + Tags.MruItem2 + " name=\"...\" path=\"...\" />");
                w.WriteLine("	<" + Tags.MruItem3 + " name=\"...\" path=\"...\" />");
                w.WriteLine("	<" + Tags.MruItem4 + " name=\"...\" path=\"...\" />");
                w.WriteLine("	<" + Tags.TabWidth + ">1</" + Tags.TabWidth + ">");
                w.WriteLine("</" + Tags.Root + ">");
                w.Close();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}