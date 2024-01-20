using ActiproSoftware.Text.Implementation;
using System.Threading.Tasks;

namespace Nightingale.Editor
{
    public class XmlSyntaxLanguage : SyntaxLanguage
    {
        public XmlSyntaxLanguage() : base("Xml")
        {

        }

        public async Task Initialize()
        {
            await LangDefLoader.InitializeLanguageFromResourceStream(this, "Xml.langdef");
        }
    }
}
