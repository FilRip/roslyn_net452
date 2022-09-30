using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlDocumentSyntax : XmlNodeSyntax
	{
		internal XmlDeclarationSyntax _declaration;

		internal SyntaxNode _precedingMisc;

		internal XmlNodeSyntax _root;

		internal SyntaxNode _followingMisc;

		public XmlDeclarationSyntax Declaration => GetRedAtZero(ref _declaration);

		public SyntaxList<XmlNodeSyntax> PrecedingMisc
		{
			get
			{
				SyntaxNode red = GetRed(ref _precedingMisc, 1);
				return new SyntaxList<XmlNodeSyntax>(red);
			}
		}

		public XmlNodeSyntax Root => GetRed(ref _root, 2);

		public SyntaxList<XmlNodeSyntax> FollowingMisc
		{
			get
			{
				SyntaxNode red = GetRed(ref _followingMisc, 3);
				return new SyntaxList<XmlNodeSyntax>(red);
			}
		}

		internal XmlDocumentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlDocumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlDeclarationSyntax declaration, SyntaxNode precedingMisc, XmlNodeSyntax root, SyntaxNode followingMisc)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)declaration.Green, precedingMisc?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)root.Green, followingMisc?.Green), null, 0)
		{
		}

		public XmlDocumentSyntax WithDeclaration(XmlDeclarationSyntax declaration)
		{
			return Update(declaration, PrecedingMisc, Root, FollowingMisc);
		}

		public XmlDocumentSyntax WithPrecedingMisc(SyntaxList<XmlNodeSyntax> precedingMisc)
		{
			return Update(Declaration, precedingMisc, Root, FollowingMisc);
		}

		public XmlDocumentSyntax AddPrecedingMisc(params XmlNodeSyntax[] items)
		{
			return WithPrecedingMisc(PrecedingMisc.AddRange(items));
		}

		public XmlDocumentSyntax WithRoot(XmlNodeSyntax root)
		{
			return Update(Declaration, PrecedingMisc, root, FollowingMisc);
		}

		public XmlDocumentSyntax WithFollowingMisc(SyntaxList<XmlNodeSyntax> followingMisc)
		{
			return Update(Declaration, PrecedingMisc, Root, followingMisc);
		}

		public XmlDocumentSyntax AddFollowingMisc(params XmlNodeSyntax[] items)
		{
			return WithFollowingMisc(FollowingMisc.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
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

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Declaration, 
				1 => GetRed(ref _precedingMisc, 1), 
				2 => Root, 
				3 => GetRed(ref _followingMisc, 3), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlDocument(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlDocument(this);
		}

		public XmlDocumentSyntax Update(XmlDeclarationSyntax declaration, SyntaxList<XmlNodeSyntax> precedingMisc, XmlNodeSyntax root, SyntaxList<XmlNodeSyntax> followingMisc)
		{
			if (declaration != Declaration || precedingMisc != PrecedingMisc || root != Root || followingMisc != FollowingMisc)
			{
				XmlDocumentSyntax xmlDocumentSyntax = SyntaxFactory.XmlDocument(declaration, precedingMisc, root, followingMisc);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlDocumentSyntax, annotations);
				}
				return xmlDocumentSyntax;
			}
			return this;
		}
	}
}
