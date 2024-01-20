using ActiproSoftware.Text.Implementation;
using System.Threading.Tasks;

namespace Nightingale.Editor
{
    public class HtmlSyntaxLanguage : SyntaxLanguage
    {
        public HtmlSyntaxLanguage() : base("Html")
        {

        }

        public async Task Initialize()
        {
            await LangDefLoader.InitializeLanguageFromResourceStream(this, "Html.langdef");
        }
    }
}
