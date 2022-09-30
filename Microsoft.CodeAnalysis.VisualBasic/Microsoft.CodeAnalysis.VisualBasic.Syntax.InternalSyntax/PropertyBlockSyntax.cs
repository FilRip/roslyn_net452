using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PropertyBlockSyntax : DeclarationStatementSyntax
	{
		internal readonly PropertyStatementSyntax _propertyStatement;

		internal readonly GreenNode _accessors;

		internal readonly EndBlockStatementSyntax _endPropertyStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PropertyStatementSyntax PropertyStatement => _propertyStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax> Accessors => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax>(_accessors);

		internal EndBlockStatementSyntax EndPropertyStatement => _endPropertyStatement;

		internal PropertyBlockSyntax(SyntaxKind kind, PropertyStatementSyntax propertyStatement, GreenNode accessors, EndBlockStatementSyntax endPropertyStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(propertyStatement);
			_propertyStatement = propertyStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endPropertyStatement);
			_endPropertyStatement = endPropertyStatement;
		}

		internal PropertyBlockSyntax(SyntaxKind kind, PropertyStatementSyntax propertyStatement, GreenNode accessors, EndBlockStatementSyntax endPropertyStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(propertyStatement);
			_propertyStatement = propertyStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endPropertyStatement);
			_endPropertyStatement = endPropertyStatement;
		}

		internal PropertyBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PropertyStatementSyntax propertyStatement, GreenNode accessors, EndBlockStatementSyntax endPropertyStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(propertyStatement);
			_propertyStatement = propertyStatement;
			if (accessors != null)
			{
				AdjustFlagsAndWidth(accessors);
				_accessors = accessors;
			}
			AdjustFlagsAndWidth(endPropertyStatement);
			_endPropertyStatement = endPropertyStatement;
		}

		internal PropertyBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)reader.ReadValue();
			if (propertyStatementSyntax != null)
			{
				AdjustFlagsAndWidth(propertyStatementSyntax);
				_propertyStatement = propertyStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_accessors = greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endPropertyStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_propertyStatement);
			writer.WriteValue(_accessors);
			writer.WriteValue(_endPropertyStatement);
		}

		static PropertyBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new PropertyBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PropertyBlockSyntax), (ObjectReader r) => new PropertyBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _propertyStatement, 
				1 => _accessors, 
				2 => _endPropertyStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PropertyBlockSyntax(base.Kind, newErrors, GetAnnotations(), _propertyStatement, _accessors, _endPropertyStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PropertyBlockSyntax(base.Kind, GetDiagnostics(), annotations, _propertyStatement, _accessors, _endPropertyStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPropertyBlock(this);
		}
	}
}
