// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
namespace ICSharpCode.AvalonEdit.AddIn
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Documents;
    using System.Windows.Threading;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.AddIn;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Editing;
    using ICSharpCode.AvalonEdit.Rendering;

    /// <summary>
    /// In the code editor, highlights all references to the expression under the caret (for better code readability).
    /// </summary>
    public class CaretReferencesRenderer
    {
        /// <summary>
        /// Delays the Resolve check so that it does not get called too often when user holds an arrow.
        /// </summary>
        DispatcherTimer delayMoveTimer;
        const int delayMoveMs = 100;

        /// <summary>
        /// Delays the Find references (and highlight) after the caret stays at one point for a while.
        /// </summary>
        DispatcherTimer delayTimer;
        const int delayMs = 800;

        /// <summary>
        /// Maximum time for Find references. After this time it gets cancelled and no highlight is displayed.
        /// Useful for very large files.
        /// </summary>
        const int findReferencesTimeoutMs = 200;

        TextEditor editor;
        ExpressionHighlightRenderer highlightRenderer;
        string lastExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaretReferencesRenderer"/> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        public CaretReferencesRenderer(TextEditor editor)
        {
            this.editor = editor;
            this.highlightRenderer = new ExpressionHighlightRenderer(this.editor.TextArea.TextView);
            this.delayTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMs) };
            this.delayTimer.Stop();
            this.delayTimer.Tick += this.TimerTick;
            this.delayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMoveMs) };
            this.delayMoveTimer.Stop();
            this.delayMoveTimer.Tick += this.TimerMoveTick;
            this.editor.TextArea.Caret.PositionChanged += this.CaretPositionChanged;
        }

        public void ClearHighlight()
        {
            this.highlightRenderer.ClearHighlight();
        }

        /// <summary>
        /// In the current document, highlights all references to the expression
        /// which is currently under the caret (local variable, class, property).
        /// This gets called on every caret position change, so quite often.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CaretPositionChanged(object sender, EventArgs e)
        {
            this.Restart(this.delayMoveTimer);
        }

        void TimerTick(object sender, EventArgs e)
        {
            this.delayTimer.Stop();

            var segmentsToBeHighlighted = this.FindReferencesInCurrentFile();
            this.highlightRenderer.SetHighlight(segmentsToBeHighlighted);
        }

        void TimerMoveTick(object sender, EventArgs e)
        {
            this.delayMoveTimer.Stop();
            this.delayTimer.Stop();

            var expressionResult = this.GetExpressionAtCaret();
            this.highlightRenderer.ClearHighlight();

            // caret is over symbol and that symbol is different from the last time
            if (this.lastExpression != expressionResult)
            {
                this.lastExpression = expressionResult;
            }

            this.delayTimer.Start();
        }

        /// <summary>
        /// Resolves the current expression under caret.
        /// This gets called on every caret position change, so quite often.
        /// </summary>
        /// <returns></returns>
        string GetExpressionAtCaret()
        {
            var expression = string.Empty;
            if (this.editor.SelectedText.Trim() != string.Empty)
            {
                expression = this.editor.SelectedText.Trim();
            }
            else
            {
                if ((this.editor.TextArea.Caret.Offset == 0) ||
                    (this.editor.TextArea.Caret.Offset >= this.editor.Text.Length - 1))
                {
                    return expression;
                }

                string prevChar = this.editor.Text.Substring(this.editor.TextArea.Caret.Offset - 1, 1);
                string nextChar = this.editor.Text.Substring(this.editor.TextArea.Caret.Offset, 1);
                if (prevChar == " " || prevChar == "\r" || prevChar == "\n" ||
                    nextChar == " " || nextChar == "\r" || nextChar == "\n")
                {
                    return expression;
                }

                int startOfWord = TextUtilities.GetNextCaretPosition(
                    this.editor.TextArea.Document,
                    this.editor.TextArea.Caret.Offset,
                    LogicalDirection.Backward,
                    CaretPositioningMode.WordBorder);

                int endOfWord = TextUtilities.GetNextCaretPosition(
                    this.editor.TextArea.Document,
                    startOfWord,
                    LogicalDirection.Forward,
                    CaretPositioningMode.WordBorder);

                expression = this.editor.Text.Substring(startOfWord, endOfWord - startOfWord);
            }

            return expression;
        }

        /// <summary>
        /// Finds references to resolved expression in the current file.
        /// </summary>
        /// <returns></returns>
        List<TextSegment> FindReferencesInCurrentFile()
        {
            var segments = this.FindReferencesLocal();
            if (segments == null || segments.Count == 0)
            {
                return null;
            }

            return segments;
        }

        List<TextSegment> FindReferencesLocal()
        {
            List<TextSegment> segs = new List<TextSegment>();
            if (this.lastExpression.Trim().Length > 0)
            {
                Regex rex = new Regex(Regex.Escape(this.lastExpression));

                MatchCollection matchlist = rex.Matches(this.editor.Text);
                foreach (Match m in matchlist)
                {
                    TextSegment t = new TextSegment();
                    t.StartOffset = m.Index;
                    t.Length = m.Length;
                    t.EndOffset = m.Index + m.Length;
                    segs.Add(t);
                }
            }

            return segs;
        }

        /// <summary>
        /// Restarts a timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        void Restart(DispatcherTimer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
