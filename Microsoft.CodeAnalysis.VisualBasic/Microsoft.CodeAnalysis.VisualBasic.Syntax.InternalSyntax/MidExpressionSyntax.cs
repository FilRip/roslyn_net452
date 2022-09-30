using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MidExpressionSyntax : ExpressionSyntax
	{
		internal readonly IdentifierTokenSyntax _mid;

		internal readonly ArgumentListSyntax _argumentList;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax Mid => _mid;

		internal ArgumentListSyntax ArgumentList => _argumentList;

		internal MidExpressionSyntax(SyntaxKind kind, IdentifierTokenSyntax mid, ArgumentListSyntax argumentList)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(mid);
			_mid = mid;
			AdjustFlagsAndWidth(argumentList);
			_argumentList = argumentList;
		}

		internal MidExpressionSyntax(SyntaxKind kind, IdentifierTokenSyntax mid, ArgumentListSyntax argumentList, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(mid);
			_mid = mid;
			AdjustFlagsAndWidth(argumentList);
			_argumentList = argumentList;
		}

		internal MidExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax mid, ArgumentListSyntax argumentList)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(mid);
			_mid = mid;
			AdjustFlagsAndWidth(argumentList);
			_argumentList = argumentList;
		}

		internal MidExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_mid = identifierTokenSyntax;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)reader.ReadValue();
			if (argumentListSyntax != null)
			{
				AdjustFlagsAndWidth(argumentListSyntax);
				_argumentList = argumentListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_mid);
			writer.WriteValue(_argumentList);
		}

		static MidExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MidExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MidExpressionSyntax), (ObjectReader r) => new MidExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MidExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _mid, 
				1 => _argumentList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MidExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _mid, _argumentList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MidExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _mid, _argumentList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMidExpression(this);
		}
	}
}
