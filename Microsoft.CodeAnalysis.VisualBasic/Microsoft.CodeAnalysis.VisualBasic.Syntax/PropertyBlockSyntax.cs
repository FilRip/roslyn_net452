using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PropertyBlockSyntax : DeclarationStatementSyntax
	{
		internal PropertyStatementSyntax _propertyStatement;

		internal SyntaxNode _accessors;

		internal EndBlockStatementSyntax _endPropertyStatement;

		public PropertyStatementSyntax PropertyStatement => GetRedAtZero(ref _propertyStatement);

		public SyntaxList<AccessorBlockSyntax> Accessors
		{
			get
			{
				SyntaxNode red = GetRed(ref _accessors, 1);
				return new SyntaxList<AccessorBlockSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndPropertyStatement => GetRed(ref _endPropertyStatement, 2);

		internal PropertyBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PropertyBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PropertyStatementSyntax propertyStatement, SyntaxNode accessors, EndBlockStatementSyntax endPropertyStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)propertyStatement.Green, accessors?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endPropertyStatement.Green), null, 0)
		{
		}

		public PropertyBlockSyntax WithPropertyStatement(PropertyStatementSyntax propertyStatement)
		{
			return Update(propertyStatement, Accessors, EndPropertyStatement);
		}

		public PropertyBlockSyntax WithAccessors(SyntaxList<AccessorBlockSyntax> accessors)
		{
			return Update(PropertyStatement, accessors, EndPropertyStatement);
		}

		public PropertyBlockSyntax AddAccessors(params AccessorBlockSyntax[] items)
		{
			return WithAccessors(Accessors.AddRange(items));
		}

		public PropertyBlockSyntax WithEndPropertyStatement(EndBlockStatementSyntax endPropertyStatement)
		{
			return Update(PropertyStatement, Accessors, endPropertyStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _propertyStatement, 
				1 => _accessors, 
				2 => _endPropertyStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => PropertyStatement, 
				1 => GetRed(ref _accessors, 1), 
				2 => EndPropertyStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPropertyBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPropertyBlock(this);
		}

		public PropertyBlockSyntax Update(PropertyStatementSyntax propertyStatement, SyntaxList<AccessorBlockSyntax> accessors, EndBlockStatementSyntax endPropertyStatement)
		{
			if (propertyStatement != PropertyStatement || accessors != Accessors || endPropertyStatement != EndPropertyStatement)
			{
				PropertyBlockSyntax propertyBlockSyntax = SyntaxFactory.PropertyBlock(propertyStatement, accessors, endPropertyStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(propertyBlockSyntax, annotations);
				}
				return propertyBlockSyntax;
			}
			return this;
		}
	}
}
