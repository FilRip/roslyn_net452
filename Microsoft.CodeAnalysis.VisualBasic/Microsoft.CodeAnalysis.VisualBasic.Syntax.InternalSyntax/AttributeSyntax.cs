using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AttributeSyntax : VisualBasicSyntaxNode
	{
		internal readonly AttributeTargetSyntax _target;

		internal readonly TypeSyntax _name;

		internal readonly ArgumentListSyntax _argumentList;

		internal static Func<ObjectReader, object> CreateInstance;

		internal AttributeTargetSyntax Target => _target;

		internal TypeSyntax Name => _name;

		internal ArgumentListSyntax ArgumentList => _argumentList;

		internal AttributeSyntax(SyntaxKind kind, AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
			: base(kind)
		{
			base._slotCount = 3;
			if (target != null)
			{
				AdjustFlagsAndWidth(target);
				_target = target;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal AttributeSyntax(SyntaxKind kind, AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (target != null)
			{
				AdjustFlagsAndWidth(target);
				_target = target;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal AttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (target != null)
			{
				AdjustFlagsAndWidth(target);
				_target = target;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal AttributeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			AttributeTargetSyntax attributeTargetSyntax = (AttributeTargetSyntax)reader.ReadValue();
			if (attributeTargetSyntax != null)
			{
				AdjustFlagsAndWidth(attributeTargetSyntax);
				_target = attributeTargetSyntax;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_name = typeSyntax;
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
			writer.WriteValue(_target);
			writer.WriteValue(_name);
			writer.WriteValue(_argumentList);
		}

		static AttributeSyntax()
		{
			CreateInstance = (ObjectReader o) => new AttributeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AttributeSyntax), (ObjectReader r) => new AttributeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _target, 
				1 => _name, 
				2 => _argumentList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AttributeSyntax(base.Kind, newErrors, GetAnnotations(), _target, _name, _argumentList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AttributeSyntax(base.Kind, GetDiagnostics(), annotations, _target, _name, _argumentList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAttribute(this);
		}
	}
}
