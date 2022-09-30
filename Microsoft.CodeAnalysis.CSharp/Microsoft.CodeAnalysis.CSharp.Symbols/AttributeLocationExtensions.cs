using System.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class AttributeLocationExtensions
    {
        internal static string ToDisplayString(this AttributeLocation locations)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int num = 1; num < 1024; num <<= 1)
            {
                if (((uint)locations & (uint)(short)num) != 0)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    switch ((short)num)
                    {
                        case 1:
                            stringBuilder.Append("assembly");
                            break;
                        case 2:
                            stringBuilder.Append("module");
                            break;
                        case 4:
                            stringBuilder.Append("type");
                            break;
                        case 8:
                            stringBuilder.Append("method");
                            break;
                        case 16:
                            stringBuilder.Append("field");
                            break;
                        case 32:
                            stringBuilder.Append("property");
                            break;
                        case 64:
                            stringBuilder.Append("event");
                            break;
                        case 256:
                            stringBuilder.Append("return");
                            break;
                        case 128:
                            stringBuilder.Append("param");
                            break;
                        case 512:
                            stringBuilder.Append("typevar");
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(num);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        internal static AttributeLocation ToAttributeLocation(this SyntaxToken token)
        {
            return ToAttributeLocation(token.ValueText);
        }

        internal static AttributeLocation ToAttributeLocation(this Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken token)
        {
            return ToAttributeLocation(token.ValueText);
        }

        private static AttributeLocation ToAttributeLocation(string text)
        {
            return text switch
            {
                "assembly" => AttributeLocation.Assembly,
                "module" => AttributeLocation.Module,
                "type" => AttributeLocation.Type,
                "return" => AttributeLocation.Return,
                "method" => AttributeLocation.Method,
                "field" => AttributeLocation.Field,
                "event" => AttributeLocation.Event,
                "param" => AttributeLocation.Parameter,
                "property" => AttributeLocation.Property,
                "typevar" => AttributeLocation.TypeParameter,
                _ => AttributeLocation.None,
            };
        }
    }
}
