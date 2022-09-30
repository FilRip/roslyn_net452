using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class TreeDumper
    {
        private readonly StringBuilder _sb;

        protected TreeDumper()
        {
            _sb = new StringBuilder();
        }

        public static string DumpCompact(TreeDumperNode root)
        {
            return new TreeDumper().DoDumpCompact(root);
        }

        protected string DoDumpCompact(TreeDumperNode root)
        {
            DoDumpCompact(root, string.Empty);
            return _sb.ToString();
        }

        private void DoDumpCompact(TreeDumperNode node, string indent)
        {
            _sb.Append(node.Text);
            if (node.Value != null)
            {
                _sb.AppendFormat(": {0}", DumperString(node.Value));
            }
            _sb.AppendLine();
            List<TreeDumperNode> list = node.Children.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                TreeDumperNode treeDumperNode = list[i];
                if (treeDumperNode != null)
                {
                    _sb.Append(indent);
                    _sb.Append((i == list.Count - 1) ? '└' : '├');
                    _sb.Append('─');
                    DoDumpCompact(treeDumperNode, indent + ((i == list.Count - 1) ? "  " : "│ "));
                }
            }
        }

        public static string DumpXML(TreeDumperNode root, string? indent = null)
        {
            TreeDumper treeDumper = new TreeDumper();
            treeDumper.DoDumpXML(root, string.Empty, indent ?? string.Empty);
            return treeDumper._sb.ToString();
        }

        private void DoDumpXML(TreeDumperNode node, string indent, string relativeIndent)
        {
            if (node.Children.All((TreeDumperNode child) => child == null))
            {
                _sb.Append(indent);
                if (node.Value != null)
                {
                    _sb.AppendFormat("<{0}>{1}</{0}>", node.Text, DumperString(node.Value));
                }
                else
                {
                    _sb.AppendFormat("<{0} />", node.Text);
                }
                _sb.AppendLine();
                return;
            }
            _sb.Append(indent);
            _sb.AppendFormat("<{0}>", node.Text);
            _sb.AppendLine();
            if (node.Value != null)
            {
                _sb.Append(indent);
                _sb.AppendFormat("{0}", DumperString(node.Value));
                _sb.AppendLine();
            }
            string indent2 = indent + relativeIndent;
            foreach (TreeDumperNode child in node.Children)
            {
                if (child != null)
                {
                    DoDumpXML(child, indent2, relativeIndent);
                }
            }
            _sb.Append(indent);
            _sb.AppendFormat("</{0}>", node.Text);
            _sb.AppendLine();
        }

        private static bool IsDefaultImmutableArray(object o)
        {
            System.Reflection.TypeInfo typeInfo = o.GetType().GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
            {
                object obj = typeInfo?.GetDeclaredMethod("get_IsDefault")?.Invoke(o, new object[0]);
                bool flag = default(bool);
                int num;
                if (obj is bool)
                {
                    flag = (bool)obj;
                    num = 1;
                }
                else
                {
                    num = 0;
                }
                return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
            }
            return false;
        }

        protected virtual string DumperString(object o)
        {
            if (o == null)
            {
                return "(null)";
            }
            if (o is string result)
            {
                return result;
            }
            if (IsDefaultImmutableArray(o))
            {
                return "(null)";
            }
            if (o is IEnumerable source)
            {
                return string.Format("{{{0}}}", string.Join(", ", source.Cast<object>().Select(DumperString).ToArray()));
            }
            if (o is ISymbol symbol)
            {
                return symbol.ToDisplayString(SymbolDisplayFormat.TestFormat);
            }
            return o.ToString() ?? "";
        }
    }
}
