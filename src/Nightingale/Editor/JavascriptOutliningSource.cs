using ActiproSoftware.Text;
using ActiproSoftware.Text.Lexing;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Outlining;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Outlining.Implementation;

namespace Nightingale.Editor
{
    public class JavascriptOutliningSource : TokenOutliningSourceBase
    {

        private static OutliningNodeDefinition curlyBraceDefinition;
        private static OutliningNodeDefinition multiLineCommentDefinition;
        private static OutliningNodeDefinition squareBraceDefinition;

        /// <summary>
        /// Initializes the <c>JavascriptOutliningSource</c> class.
        /// </summary>
        static JavascriptOutliningSource()
        {
            // Create the outlining node definitions that will be used by this outlining source to
            //   tell the document's outlining manager how to create new outlining nodes...
            //   Each definition can indicate options such as: 
            //   1) Whether the node is an implementation and will be collapsed when "Collapse to Definitions" is clicked
            //   2) The default collapsed content for the node that appears in the in-line collapsed node box
            //   3) If the node should be collapsed by default when loading a file, such as for #region type nodes
            //   4) If the node is collapsible... when false, no UI appears for the node in the margin

            squareBraceDefinition = new OutliningNodeDefinition("SquareBrace");
            squareBraceDefinition.IsImplementation = true;

            curlyBraceDefinition = new OutliningNodeDefinition("CurlyBrace");
            curlyBraceDefinition.IsImplementation = true;

            multiLineCommentDefinition = new OutliningNodeDefinition("MultiLineComment");
            multiLineCommentDefinition.DefaultCollapsedContent = "/**/";
            multiLineCommentDefinition.IsImplementation = true;
        }

        /// <summary>
        /// Initializes a new instance of the <c>JavascriptOutliningSource</c> class.
        /// </summary>
        /// <param name="snapshot">The <see cref="ITextSnapshot"/> to use for this outlining source.</param>
        public JavascriptOutliningSource(ITextSnapshot snapshot) : base(snapshot) { }

        /// <summary>
        /// Returns information about the action to take when incrementally updating automatic outlining for a particular token.
        /// </summary>
        /// <param name="token">The <see cref="IToken"/> to examine.</param>
        /// <param name="definition">
        /// If the node action indicated is a start or end, an <see cref="IOutliningNodeDefinition"/> describing the related
        /// node must be returned.
        /// </param>
        /// <returns>
        /// An <see cref="OutliningNodeAction"/> indicating the action to take for the token.
        /// </returns>
        protected override OutliningNodeAction GetNodeActionForToken(IToken token, out IOutliningNodeDefinition definition)
        {
            switch (token.Key)
            {
                case "MultiLineCommentStartDelimiter":
                    definition = multiLineCommentDefinition;
                    return OutliningNodeAction.Start;
                case "MultiLineCommentEndDelimiter":
                    definition = multiLineCommentDefinition;
                    return OutliningNodeAction.End;
                case "OpenCurlyBrace":
                    definition = curlyBraceDefinition;
                    return OutliningNodeAction.Start;
                case "CloseCurlyBrace":
                    definition = curlyBraceDefinition;
                    return OutliningNodeAction.End;
                case "OpenSquareBrace":
                    definition = squareBraceDefinition;
                    return OutliningNodeAction.Start;
                case "CloseSquareBrace":
                    definition = squareBraceDefinition;
                    return OutliningNodeAction.End;
                default:
                    definition = null;
                    return OutliningNodeAction.None;
            }
        }
    }
}
