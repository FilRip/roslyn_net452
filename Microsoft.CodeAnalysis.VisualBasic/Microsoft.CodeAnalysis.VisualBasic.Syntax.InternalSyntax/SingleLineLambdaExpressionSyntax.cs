using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SingleLineLambdaExpressionSyntax : LambdaExpressionSyntax
	{
		internal readonly VisualBasicSyntaxNode _body;

		internal static Func<ObjectReader, object> CreateInstance;

		internal VisualBasicSyntaxNode Body => _body;

		internal SingleLineLambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
			: base(kind, subOrFunctionHeader)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(body);
			_body = body;
		}

		internal SingleLineLambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body, ISyntaxFactoryContext context)
			: base(kind, subOrFunctionHeader)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(body);
			_body = body;
		}

		internal SingleLineLambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
			: base(kind, errors, annotations, subOrFunctionHeader)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(body);
			_body = body;
		}

		internal SingleLineLambdaExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)reader.ReadValue();
			if (visualBasicSyntaxNode != null)
			{
				AdjustFlagsAndWidth(visualBasicSyntaxNode);
				_body = visualBasicSyntaxNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_body);
		}

		static SingleLineLambdaExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new SingleLineLambdaExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SingleLineLambdaExpressionSyntax), (ObjectReader r) => new SingleLineLambdaExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionHeader, 
				1 => _body, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SingleLineLambdaExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _subOrFunctionHeader, _body);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SingleLineLambdaExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _subOrFunctionHeader, _body);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSingleLineLambdaExpression(this);
		}
	}
}
