using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.ViewModels
{
    public class EditorViewModel
    {
        public string Text { get; set; }

        public SyntaxType Syntax { get; set; }

    }

    public enum SyntaxType
    {
        Plain,
        Json,
        Xml,
        Html
    }
}
