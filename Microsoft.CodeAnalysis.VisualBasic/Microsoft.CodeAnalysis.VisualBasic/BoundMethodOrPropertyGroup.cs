using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundMethodOrPropertyGroup : BoundExpression
	{
		private readonly BoundExpression _ReceiverOpt;

		private readonly QualificationKind _QualificationKind;

		internal string MemberName => base.Kind switch
		{
			BoundKind.MethodGroup => ((BoundMethodGroup)this).Methods[0].Name, 
			BoundKind.PropertyGroup => ((BoundPropertyGroup)this).Properties[0].Name, 
			_ => throw ExceptionUtilities.UnexpectedValue(base.Kind), 
		};

		internal TypeSymbol ContainerOfFirstInGroup => base.Kind switch
		{
			BoundKind.MethodGroup => ((BoundMethodGroup)this).Methods[0].ContainingType, 
			BoundKind.PropertyGroup => ((BoundPropertyGroup)this).Properties[0].ContainingType, 
			_ => throw ExceptionUtilities.UnexpectedValue(base.Kind), 
		};

		protected override ImmutableArray<BoundNode> Children
		{
			get
			{
				if (ReceiverOpt != null)
				{
					return ImmutableArray.Create((BoundNode)ReceiverOpt);
				}
				return ImmutableArray<BoundNode>.Empty;
			}
		}

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public QualificationKind QualificationKind => _QualificationKind;

		protected BoundMethodOrPropertyGroup(BoundKind kind, SyntaxNode syntax, BoundExpression receiverOpt, QualificationKind qualificationKind, bool hasErrors = false)
			: base(kind, syntax, null, hasErrors)
		{
			_ReceiverOpt = receiverOpt;
			_QualificationKind = qualificationKind;
		}
	}
}
