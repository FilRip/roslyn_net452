using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SyncLockStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _syncLockKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SyncLockKeyword => _syncLockKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal SyncLockStatementSyntax(SyntaxKind kind, KeywordSyntax syncLockKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(syncLockKeyword);
			_syncLockKeyword = syncLockKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SyncLockStatementSyntax(SyntaxKind kind, KeywordSyntax syncLockKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(syncLockKeyword);
			_syncLockKeyword = syncLockKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SyncLockStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax syncLockKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(syncLockKeyword);
			_syncLockKeyword = syncLockKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SyncLockStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_syncLockKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_syncLockKeyword);
			writer.WriteValue(_expression);
		}

		static SyncLockStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new SyncLockStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SyncLockStatementSyntax), (ObjectReader r) => new SyncLockStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _syncLockKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SyncLockStatementSyntax(base.Kind, newErrors, GetAnnotations(), _syncLockKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SyncLockStatementSyntax(base.Kind, GetDiagnostics(), annotations, _syncLockKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSyncLockStatement(this);
		}
	}
}
