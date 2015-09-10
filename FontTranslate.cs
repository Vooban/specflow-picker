//-----------------------------------------------------------------------
// <copyright file="FontTranslate.cs" company="StrikeByte">
//     StrikeByte Software, Inc. 2011-
// </copyright>
//-----------------------------------------------------------------------
namespace System.Fonts
{
    using System;
    using System.Drawing;
    using System.Windows.Media;

    /// <summary>
    /// This class is used to work with translating fonts information for use with .Net 4.0
    /// </summary>
    public class FontTranslate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontTranslate"/> class.
        /// </summary>
        /// <param name="fontObject">The font object.</param>
        public FontTranslate(Font fontObject)
        {
            this.Font = fontObject;
            this.FillFamilyStyleTypeface();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTranslate"/> class.
        /// </summary>
        /// <param name="typefaceObject">The typeface object.</param>
        /// <param name="size">The fonts size.</param>
        public FontTranslate(
            Typeface typefaceObject,
            double size)
        {
            Typeface = typefaceObject;
            this.FontFamily = Typeface.FontFamily;
            this.FontStyle = Typeface.Style;
            this.FontWeight = Typeface.Weight;
            this.FontSize = size;

            this.FillFont();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTranslate"/> class.
        /// </summary>
        /// <param name="familyObject">The fonts family object.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="styleObject">The fonts style object.</param>
        /// <param name="weightObject">The fonts weight object.</param>
        public FontTranslate(
            System.Windows.Media.FontFamily familyObject,
            double size,
            Windows.FontStyle styleObject,
            Windows.FontWeight weightObject)
        {
            this.FontFamily = familyObject;
            this.FontStyle = styleObject;
            this.FontSize = size;
            this.Typeface = new Typeface(familyObject, styleObject, weightObject, Windows.FontStretches.Normal);

            this.FillFont();
        }
        
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font to translate.
        /// </value>
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public System.Windows.Media.FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public Windows.FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public Windows.FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the typeface.
        /// </summary>
        /// <value>
        /// The typeface.
        /// </value>
        public Typeface Typeface { get; set; }

        /// <summary>
        /// Fills the family style typeface.
        /// </summary>
        private void FillFamilyStyleTypeface()
        {
            FontFamilyConverter ffc = new FontFamilyConverter();
            this.FontFamily = (System.Windows.Media.FontFamily)ffc.ConvertFromString(Font.FontFamily.Name);

            this.FontSize = (double)Font.Size;

            this.FontWeight = Windows.FontWeights.Normal;
            if (Font.Bold == true)
            {
                this.FontWeight = Windows.FontWeights.Bold;
            }

            this.FontStyle = Windows.FontStyles.Normal;
            if (Font.Italic == true)
            {
                this.FontStyle = Windows.FontStyles.Italic;
            }

            this.Typeface = new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, Windows.FontStretches.Normal);
        }

        /// <summary>
        /// Fills the font.
        /// </summary>
        private void FillFont()
        {
            int fontStyleBold = 0;
            if (this.FontWeight == Windows.FontWeights.Bold)
            {
                fontStyleBold = Convert.ToInt32(System.Drawing.FontStyle.Bold);
            }

            int fontStyleItalic = 0;
            if (this.FontStyle == Windows.FontStyles.Italic)
            {
                fontStyleItalic = Convert.ToInt32(System.Drawing.FontStyle.Italic);
            } 

            var fntStyle = (System.Drawing.FontStyle)(fontStyleBold & fontStyleItalic);

            this.Font = new Font(
                new FontFamilyConverter().ConvertToString(this.FontFamily),
                (float)this.FontSize,
                fntStyle);
        }
    }
}
