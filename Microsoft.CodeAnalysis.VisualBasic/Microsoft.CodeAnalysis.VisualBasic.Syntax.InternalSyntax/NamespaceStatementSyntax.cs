using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NamespaceStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly KeywordSyntax _namespaceKeyword;

		internal readonly NameSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax NamespaceKeyword => _namespaceKeyword;

		internal NameSyntax Name => _name;

		internal NamespaceStatementSyntax(SyntaxKind kind, KeywordSyntax namespaceKeyword, NameSyntax name)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(namespaceKeyword);
			_namespaceKeyword = namespaceKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal NamespaceStatementSyntax(SyntaxKind kind, KeywordSyntax namespaceKeyword, NameSyntax name, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(namespaceKeyword);
			_namespaceKeyword = namespaceKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal NamespaceStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax namespaceKeyword, NameSyntax name)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(namespaceKeyword);
			_namespaceKeyword = namespaceKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal NamespaceStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_namespaceKeyword = keywordSyntax;
			}
			NameSyntax nameSyntax = (NameSyntax)reader.ReadValue();
			if (nameSyntax != null)
			{
				AdjustFlagsAndWidth(nameSyntax);
				_name = nameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_namespaceKeyword);
			writer.WriteValue(_name);
		}

		static NamespaceStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new NamespaceStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NamespaceStatementSyntax), (ObjectReader r) => new NamespaceStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _namespaceKeyword, 
				1 => _name, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NamespaceStatementSyntax(base.Kind, newErrors, GetAnnotations(), _namespaceKeyword, _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NamespaceStatementSyntax(base.Kind, GetDiagnostics(), annotations, _namespaceKeyword, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNamespaceStatement(this);
		}
	}
}
