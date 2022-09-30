using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class VariableNameEqualsSyntax : VisualBasicSyntaxNode
	{
		internal readonly ModifiedIdentifierSyntax _identifier;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly PunctuationSyntax _equalsToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ModifiedIdentifierSyntax Identifier => _identifier;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal VariableNameEqualsSyntax(SyntaxKind kind, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal VariableNameEqualsSyntax(SyntaxKind kind, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal VariableNameEqualsSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
		}

		internal VariableNameEqualsSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)reader.ReadValue();
			if (modifiedIdentifierSyntax != null)
			{
				AdjustFlagsAndWidth(modifiedIdentifierSyntax);
				_identifier = modifiedIdentifierSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
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
			writer.WriteValue(_asClause);
			writer.WriteValue(_equalsToken);
		}

		static VariableNameEqualsSyntax()
		{
			CreateInstance = (ObjectReader o) => new VariableNameEqualsSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(VariableNameEqualsSyntax), (ObjectReader r) => new VariableNameEqualsSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _asClause, 
				2 => _equalsToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new VariableNameEqualsSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _asClause, _equalsToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new VariableNameEqualsSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _asClause, _equalsToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitVariableNameEquals(this);
		}
	}
}
