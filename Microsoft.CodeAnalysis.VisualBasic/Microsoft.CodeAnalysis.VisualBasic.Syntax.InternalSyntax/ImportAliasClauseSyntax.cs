using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ImportAliasClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly PunctuationSyntax _equalsToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal ImportAliasClauseSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal ImportAliasClauseSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal ImportAliasClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal ImportAliasClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
			writer.WriteValue(_equalsToken);
		}

		static ImportAliasClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new ImportAliasClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ImportAliasClauseSyntax), (ObjectReader r) => new ImportAliasClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportAliasClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _equalsToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ImportAliasClauseSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _equalsToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ImportAliasClauseSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _equalsToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitImportAliasClause(this);
		}
	}
}
