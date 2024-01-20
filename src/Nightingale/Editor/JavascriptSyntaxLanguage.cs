using ActiproSoftware.Text.Implementation;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.IntelliPrompt.Implementation;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Outlining;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Outlining.Implementation;
using System.Threading.Tasks;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Analysis.Implementation;

namespace Nightingale.Editor
{
    public class JavascriptSyntaxLanguage : SyntaxLanguage
    {
        public JavascriptSyntaxLanguage() : base("Javascript")
        {
            this.RegisterDelimiterAutoCompleter(new DelimiterAutoCompleter()
            {
                CanCompleteSquareBraces = true,
                CanCompleteCurlyBraces = true,
                OpenCurlyBraceTokenId = JavaScriptTokenId.OpenCurlyBrace,
                OpenSquareBraceTokenId = JavaScriptTokenId.OpenSquareBrace
            });

            this.RegisterIndentProvider(new JavaScriptIndentProvider());

            // Register an outliner, which tells the document's outlining manager that
            //   this language supports automatic outlining, and helps drive outlining updates
            this.RegisterService<IOutliner>(new TokenOutliner<JavascriptOutliningSource>());

            // Register a built-in service that automatically provides quick info tips 
            //   when hovering over collapsed outlining nodes
            this.RegisterService(new CollapsedRegionQuickInfoProvider());
        }

        public async Task Initialize()
        {
            // Initialize this language from a language definition
            await LangDefLoader.InitializeLanguageFromResourceStream(this, "JavaScript.langdef");
        }
    }
}
