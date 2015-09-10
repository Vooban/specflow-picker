// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)


namespace ICSharpCode.AvalonEdit.AddIn
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    /// <summary>
    /// Highlights expressions (references to expression under current caret).
    /// </summary>
    public class ExpressionHighlightRenderer : IBackgroundRenderer
    {
        List<TextSegment> renderedSegments;
        Pen borderPen;
        Brush backgroundBrush;
        TextView textView;
        Color borderColor = Color.FromArgb(120, 30, 130, 255);
        Color fillColor = Color.FromArgb(90, 30, 130, 255);
        readonly int borderThickness = 1;
        readonly int cornerRadius = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionHighlightRenderer"/> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        public ExpressionHighlightRenderer(TextView textView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            this.textView = textView;
            this.borderPen = new Pen(new SolidColorBrush(this.borderColor), this.borderThickness);
            this.backgroundBrush = new SolidColorBrush(this.fillColor);
            this.borderPen.Freeze();
            this.backgroundBrush.Freeze();
            this.textView.BackgroundRenderers.Add(this);
        }

        /// <summary>
        /// Gets the layer.
        /// </summary>
        public KnownLayer Layer
        {
            get
            {
                return KnownLayer.Selection;
            }
        }

        /// <summary>
        /// Sets the highlight.
        /// </summary>
        /// <param name="renderedSegments">The rendered segments.</param>
        public void SetHighlight(List<TextSegment> renderedSegments)
        {
            if (this.renderedSegments != renderedSegments)
            {
                this.renderedSegments = renderedSegments;
                this.textView.InvalidateLayer(this.Layer);
            }
        }

        /// <summary>
        /// Clears the highlight.
        /// </summary>
        public void ClearHighlight()
        {
            this.SetHighlight(null);
        }
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this.renderedSegments == null)
            {
                return;
            }

            BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();
            builder.CornerRadius = this.cornerRadius;
            builder.AlignToMiddleOfPixels = true;
            foreach (var segment in this.renderedSegments)
            {
                builder.AddSegment(textView, segment);
                builder.CloseFigure();
            }

            Geometry geometry = builder.CreateGeometry();
            if (geometry != null)
            {
                drawingContext.DrawGeometry(this.backgroundBrush, this.borderPen, geometry);
            }
        }
    }
}
