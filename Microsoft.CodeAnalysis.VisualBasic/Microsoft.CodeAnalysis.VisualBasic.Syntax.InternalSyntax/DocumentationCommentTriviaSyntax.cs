using System;
using System.Text;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DocumentationCommentTriviaSyntax : StructuredTriviaSyntax
	{
		internal readonly GreenNode _content;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> Content => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(_content);

		internal DocumentationCommentTriviaSyntax(SyntaxKind kind, GreenNode content)
			: base(kind)
		{
			base._slotCount = 1;
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
		}

		internal DocumentationCommentTriviaSyntax(SyntaxKind kind, GreenNode content, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
		}

		internal DocumentationCommentTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode content)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
		}

		internal DocumentationCommentTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_content = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_content);
		}

		static DocumentationCommentTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new DocumentationCommentTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DocumentationCommentTriviaSyntax), (ObjectReader r) => new DocumentationCommentTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DocumentationCommentTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _content;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DocumentationCommentTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _content);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DocumentationCommentTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _content);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDocumentationCommentTrivia(this);
		}

		internal string GetInteriorXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WriteInteriorXml((GreenNode)this, stringBuilder);
			return stringBuilder.ToString();
		}

		private static void WriteInteriorXml(GreenNode node, StringBuilder sb)
		{
			if (node == null)
			{
				return;
			}
			int slotCount = node.SlotCount;
			if (slotCount > 0)
			{
				int num = slotCount - 1;
				for (int i = 0; i <= num; i++)
				{
					WriteInteriorXml(node.GetSlot(i), sb);
				}
			}
			else
			{
				SyntaxToken obj = (SyntaxToken)node;
				WriteInteriorXml(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(obj.GetLeadingTrivia()), sb);
				WriteInteriorXml(obj, sb);
				WriteInteriorXml(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(obj.GetTrailingTrivia()), sb);
			}
		}

		private static void WriteInteriorXml(SyntaxToken node, StringBuilder sb)
		{
			if (node.Kind != SyntaxKind.DocumentationCommentLineBreakToken)
			{
				_ = node.Text;
				sb.Append(node.Text);
			}
		}

		private static void WriteInteriorXml(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> node, StringBuilder sb)
		{
			int num = node.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = node[i];
				if (visualBasicSyntaxNode.Kind != SyntaxKind.DocumentationCommentExteriorTrivia)
				{
					sb.Append(visualBasicSyntaxNode.ToString());
				}
			}
		}
	}
}
