using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ImplementsClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _implementsKeyword;

		internal readonly GreenNode _interfaceMembers;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ImplementsKeyword => _implementsKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<QualifiedNameSyntax> InterfaceMembers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<QualifiedNameSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QualifiedNameSyntax>(_interfaceMembers));

		internal ImplementsClauseSyntax(SyntaxKind kind, KeywordSyntax implementsKeyword, GreenNode interfaceMembers)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (interfaceMembers != null)
			{
				AdjustFlagsAndWidth(interfaceMembers);
				_interfaceMembers = interfaceMembers;
			}
		}

		internal ImplementsClauseSyntax(SyntaxKind kind, KeywordSyntax implementsKeyword, GreenNode interfaceMembers, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (interfaceMembers != null)
			{
				AdjustFlagsAndWidth(interfaceMembers);
				_interfaceMembers = interfaceMembers;
			}
		}

		internal ImplementsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax implementsKeyword, GreenNode interfaceMembers)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (interfaceMembers != null)
			{
				AdjustFlagsAndWidth(interfaceMembers);
				_interfaceMembers = interfaceMembers;
			}
		}

		internal ImplementsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_implementsKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_interfaceMembers = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_implementsKeyword);
			writer.WriteValue(_interfaceMembers);
		}

		static ImplementsClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new ImplementsClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ImplementsClauseSyntax), (ObjectReader r) => new ImplementsClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _implementsKeyword, 
				1 => _interfaceMembers, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ImplementsClauseSyntax(base.Kind, newErrors, GetAnnotations(), _implementsKeyword, _interfaceMembers);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ImplementsClauseSyntax(base.Kind, GetDiagnostics(), annotations, _implementsKeyword, _interfaceMembers);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitImplementsClause(this);
		}
	}
}
