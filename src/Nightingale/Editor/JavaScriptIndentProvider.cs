using ActiproSoftware.Text;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Implementation;
using System;

namespace Nightingale.Editor
{
    public class JavaScriptIndentProvider : DelimiterIndentProvider
    {

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // OBJECT
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <c>SimpleIndentProvider</c> class.
        /// </summary>
        public JavaScriptIndentProvider()
        {
            // Initialize
            this.CloseCurlyBraceTokenId = JavaScriptTokenId.CloseCurlyBrace;
            this.OpenCurlyBraceTokenId = JavaScriptTokenId.OpenCurlyBrace;
            this.CloseSquareBraceTokenId = JavaScriptTokenId.CloseSquareBrace;
            this.OpenSquareBraceTokenId = JavaScriptTokenId.OpenSquareBrace;
            this.CanAutoIndentSquareBraces = true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // PUBLIC PROCEDURES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the ideal amount of indent, always in columns, for the line containing the snapshot offset.
        /// </summary>
        /// <param name="snapshotOffset">The <see cref="TextSnapshotOffset"/> whose line should be examined.</param>
        /// <param name="defaultAmount">The default indent amount, which is the amount used in <c>Block</c> mode.</param>
        /// <returns>The ideal amount of indent, always in columns, for the line containing the snapshot offset.</returns>
        /// <remarks>
        /// This method is called when the <see cref="IndentMode"/> is <c>Smart</c>.
        /// The containing <see cref="ITextDocument"/> is accessible via the snapshot range's <see cref="ITextSnapshot"/>.
        /// </remarks>
        public override int GetIndentAmount(TextSnapshotOffset snapshotOffset, int defaultAmount)
        {
            // If the snapshot offset is deleted, return the default amount
            if (snapshotOffset.IsDeleted)
                return defaultAmount;

            // Get the ICodeDocument from the snapshot
            ICodeDocument document = snapshotOffset.Snapshot.Document as ICodeDocument;
            if (document == null)
                return defaultAmount;

            // Get the tab size
            int tabSize = document.TabSize;

            // Get a reader
            ITextSnapshotReader reader = snapshotOffset.Snapshot.GetReader(snapshotOffset.Offset);
            if (reader == null)
                return defaultAmount;

            // Get the indentation base line index
            int indentationBaseLineIndex = Math.Max(0, snapshotOffset.Line.Index - 1);

            // Ensure we are at the start of the current token
            if (!reader.IsAtTokenStart)
                reader.GoToCurrentTokenStart();

            // If finding indentation for an open curly brace, move back a token
            bool isForOpenBrace = reader.Token.Id == JavaScriptTokenId.OpenCurlyBrace || reader.Token.Id == JavaScriptTokenId.OpenSquareBrace;
            if (isForOpenBrace)
                reader.GoToPreviousToken();

            // Loop backwards
            bool keywordFoundAfterStatement = false;
            bool statementFound = false;
            while (true)
            {
                switch (reader.Token.Id)
                {
                    case JavaScriptTokenId.OpenSquareBrace:
                        {
                            return reader.SnapshotLine.IndentAmount + tabSize;
                        }
                    case JavaScriptTokenId.OpenCurlyBrace:
                        {
                            // Indent from this open curly brace
                            return reader.SnapshotLine.IndentAmount + tabSize;
                        }
                    case JavaScriptTokenId.CloseCurlyBrace:
                        // Return the indent level of the matching {
                        reader.GoToPreviousMatchingTokenById(JavaScriptTokenId.CloseCurlyBrace, JavaScriptTokenId.OpenCurlyBrace);
                        return reader.SnapshotLine.IndentAmount;
                    case JavaScriptTokenId.CloseSquareBrace:
                        reader.GoToPreviousMatchingTokenById(JavaScriptTokenId.CloseSquareBrace, JavaScriptTokenId.OpenSquareBrace);
                        return reader.SnapshotLine.IndentAmount;
                    //case JavaScriptTokenId.CloseParenthesis:
                    //case JavaScriptTokenId.SemiColon:
                    //    if (!statementFound)
                    //    {
                    //        // Flag that a statement was found
                    //        statementFound = true;

                    //        if (!keywordFoundAfterStatement)
                    //        {
                    //            // Use this line as indentation base
                    //            indentationBaseLineIndex = reader.SnapshotLine.Index;
                    //        }
                    //    }
                    //    break;
                    default:
                        if ((!keywordFoundAfterStatement) && (!statementFound) && (reader.Offset < snapshotOffset.Offset))
                            //&& (reader.Token.Id >= SimpleTokenId.Function) && (reader.Token.Id <= SimpleTokenId.Var))
                        {
                            // Flag that a keyword was found
                            keywordFoundAfterStatement = true;

                            // Use this line as indentation base
                            indentationBaseLineIndex = reader.SnapshotLine.Index;
                        }
                        break;
                }

                // Go to the previous token
                if (!reader.GoToPreviousToken())
                    break;
            }

            // Indent a level if on the statement after the keyword
            return reader.Snapshot.Lines[indentationBaseLineIndex].IndentAmount + (keywordFoundAfterStatement && isForOpenBrace ? tabSize : 0);
        }

        /// <summary>
        /// Gets the <see cref="IndentMode"/> that specifies the mode by which to indent text.
        /// </summary>
        /// <value>The <see cref="IndentMode"/> that specifies the mode by which to indent text.</value>
        public override IndentMode Mode => IndentMode.Smart;

        /// <summary>
        /// Occurs after a text change occurs to an <see cref="IEditorDocument"/> that uses this language.
        /// </summary>
        /// <param name="editor">The <see cref="SyntaxEditor"/> whose <see cref="IEditorDocument"/> is changed.</param>
        /// <param name="e">The <c>EditorSnapshotChangedEventArgs</c> that contains the event data.</param>
        protected override void OnDocumentTextChanged(SyntaxEditor editor, EditorSnapshotChangedEventArgs e)
        {
            // If the user is typing a '}' character...
            if (e.TypedText == "}" || e.TypedText == "]")
            {
                // Ensure the '}' is the first non-whitespace character on the line
                ITextSnapshotLine startLine = e.ChangedSnapshotRange.StartLine;
                if (startLine.FirstNonWhitespaceCharacterOffset != e.ChangedSnapshotRange.StartOffset)
                    return;

                // Get the indent amount of the previous line
                int previousLineIndex = Math.Max(0, startLine.Index - 1);
                int previousLineIndentAmount = startLine.Snapshot.Lines[previousLineIndex].IndentAmount;

                // The new indent should be a tab stop out
                int indentAmount = Math.Max(0, this.GetIndentAmount(new TextSnapshotOffset(e.ChangedSnapshotRange.Snapshot, e.ChangedSnapshotRange.StartOffset), previousLineIndentAmount));
                startLine.IndentAmount = indentAmount;
            }

            // Call the base method
            base.OnDocumentTextChanged(editor, e);
        }

        /// <summary>
        /// Occurs before a text change occurs to an <see cref="IEditorDocument"/> that uses this language.
        /// </summary>
        /// <param name="editor">The <see cref="SyntaxEditor"/> whose <see cref="IEditorDocument"/> that is changing.</param>
        /// <param name="e">The <c>EditorSnapshotChangingEventArgs</c> that contains the event data.</param>
        protected override void OnDocumentTextChanging(SyntaxEditor editor, EditorSnapshotChangingEventArgs e)
        {
            // Call the base method
            base.OnDocumentTextChanging(editor, e);
        }

    }
}
