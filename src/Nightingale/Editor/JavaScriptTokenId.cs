using ActiproSoftware.Text.Lexing.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Editor
{
    public class JavaScriptTokenId : TokenIdProviderBase
    {
        public const int Whitespace = 1;
        public const int LineTerminator = 2;
        public const int OpenParenthesis = 3;
        public const int CloseParenthesis = 4;
        public const int OpenCurlyBrace = 5;
        public const int CloseCurlyBrace = 6;
        public const int OpenSquareBrace = 7;
        public const int CloseSquareBrace = 8;
        public const int Punctuation = 9;
        public const int Null = 10;

        public const int Boolean = 11;
        public const int Keyword = 12;
        public const int Identifier = 13;
        public const int Operator = 14;
        public const int RealNumber = 15;
        public const int IntegerNumber = 16;
        public const int HexNumber = 17;
        public const int PrimaryStringText = 18;
        public const int PrimaryStringStartDelimiter = 19;
        public const int PrimaryStringEndDelimiter = 20;

        public const int PrimaryStringEscapedCharacter = 21;
        public const int AlternateStringText = 22;
        public const int AlternateStringStartDelimiter = 23;
        public const int AlternateStringEndDelimiter = 24;
        public const int AlternateStringEscapedCharacter = 25;
        public const int SingleLineCommentText = 26;
        public const int SingleLineCommentStartDelimiter = 27;
        public const int SingleLineCommentEndDelimiter = 28;
        public const int MultiLineCommentText = 29;
        public const int MultiLineCommentStartDelimiter = 30;

        public const int MultiLineCommentEndDelimiter = 31;
        public const int MultiLineCommentLineTerminator = 32;
        public const int RegExpLiteralText = 33;
        public const int RegExpLiteralStartDelimiter = 34;
        public const int RegExpLiteralEndDelimiter = 35;
        public const int RegExpLiteralEscapedCharacter = 36;

        public override int MaxId => 42;

        public override int MinId => 1;

        public override bool ContainsId(int id) => (id >= MinId) && (id <= MaxId);

        public override string GetDescription(int id)
        {
            FieldInfo[] fields = GetFields();

            for (int index = 0; (index < fields.Length); index = (index + 1))
            {
                FieldInfo field = fields[index];

                if (id.Equals(field.GetValue(null)))
                {
                    object descriptionAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                    return descriptionAttr != null ? ((DescriptionAttribute)descriptionAttr).Description : field.Name;
                }
            }

            return null;
        }

        public override string GetKey(int id)
        {
            FieldInfo[] fields = GetFields();

            for (int index = 0; index < fields.Length; index += 1)
            {
                FieldInfo field = fields[index];

                if (id.Equals(field.GetValue(null)))
                {
                    return field.Name;
                }
            }

            return null;
        }

        private static FieldInfo[] GetFields()
        {
            return typeof(JavaScriptTokenId).GetTypeInfo().DeclaredFields
                .Where(f => f.IsPublic && f.IsStatic)
                .ToArray();
        }

        /// <summary>
        /// Specifies a description for a property or event.
        /// </summary>
        private class DescriptionAttribute : Attribute
        {

            private string descriptionValue;

            /// <summary>
            /// Initializes a new instance of the <c>DescriptionAttribute</c> class.
            /// </summary>
            /// <param name="descriptionValue">The description stored in this attribute.</param>
            public DescriptionAttribute(string descriptionValue)
            {
                this.descriptionValue = descriptionValue;
            }

            /// <summary>
            /// Gets the description stored in this attribute.
            /// </summary>
            /// <value>The description stored in this attribute.</value>
            public string Description
            {
                get
                {
                    return this.descriptionValue;
                }
            }
        }
    }
}
