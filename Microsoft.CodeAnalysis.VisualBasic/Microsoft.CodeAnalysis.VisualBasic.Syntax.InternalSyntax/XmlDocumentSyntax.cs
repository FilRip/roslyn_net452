using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlDocumentSyntax : XmlNodeSyntax
	{
		internal readonly XmlDeclarationSyntax _declaration;

		internal readonly GreenNode _precedingMisc;

		internal readonly XmlNodeSyntax _root;

		internal readonly GreenNode _followingMisc;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlDeclarationSyntax Declaration => _declaration;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> PrecedingMisc => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(_precedingMisc);

		internal XmlNodeSyntax Root => _root;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> FollowingMisc => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(_followingMisc);

		internal XmlDocumentSyntax(SyntaxKind kind, XmlDeclarationSyntax declaration, GreenNode precedingMisc, XmlNodeSyntax root, GreenNode followingMisc)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(declaration);
			_declaration = declaration;
			if (precedingMisc != null)
			{
				AdjustFlagsAndWidth(precedingMisc);
				_precedingMisc = precedingMisc;
			}
			AdjustFlagsAndWidth(root);
			_root = root;
			if (followingMisc != null)
			{
				AdjustFlagsAndWidth(followingMisc);
				_followingMisc = followingMisc;
			}
		}

		internal XmlDocumentSyntax(SyntaxKind kind, XmlDeclarationSyntax declaration, GreenNode precedingMisc, XmlNodeSyntax root, GreenNode followingMisc, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(declaration);
			_declaration = declaration;
			if (precedingMisc != null)
			{
				AdjustFlagsAndWidth(precedingMisc);
				_precedingMisc = precedingMisc;
			}
			AdjustFlagsAndWidth(root);
			_root = root;
			if (followingMisc != null)
			{
				AdjustFlagsAndWidth(followingMisc);
				_followingMisc = followingMisc;
			}
		}

		internal XmlDocumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlDeclarationSyntax declaration, GreenNode precedingMisc, XmlNodeSyntax root, GreenNode followingMisc)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(declaration);
			_declaration = declaration;
			if (precedingMisc != null)
			{
				AdjustFlagsAndWidth(precedingMisc);
				_precedingMisc = precedingMisc;
			}
			AdjustFlagsAndWidth(root);
			_root = root;
			if (followingMisc != null)
			{
				AdjustFlagsAndWidth(followingMisc);
				_followingMisc = followingMisc;
			}
		}

		internal XmlDocumentSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			XmlDeclarationSyntax xmlDeclarationSyntax = (XmlDeclarationSyntax)reader.ReadValue();
			if (xmlDeclarationSyntax != null)
			{
				AdjustFlagsAndWidth(xmlDeclarationSyntax);
				_declaration = xmlDeclarationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_precedingMisc = greenNode;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)reader.ReadValue();
			if (xmlNodeSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNodeSyntax);
				_root = xmlNodeSyntax;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_followingMisc = greenNode2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_declaration);
			writer.WriteValue(_precedingMisc);
			writer.WriteValue(_root);
			writer.WriteValue(_followingMisc);
		}

		static XmlDocumentSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlDocumentSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlDocumentSyntax), (ObjectReader r) => new XmlDocumentSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _declaration, 
				1 => _precedingMisc, 
				2 => _root, 
				3 => _followingMisc, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlDocumentSyntax(base.Kind, newErrors, GetAnnotations(), _declaration, _precedingMisc, _root, _followingMisc);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlDocumentSyntax(base.Kind, GetDiagnostics(), annotations, _declaration, _precedingMisc, _root, _followingMisc);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlDocument(this);
		}
	}
}
