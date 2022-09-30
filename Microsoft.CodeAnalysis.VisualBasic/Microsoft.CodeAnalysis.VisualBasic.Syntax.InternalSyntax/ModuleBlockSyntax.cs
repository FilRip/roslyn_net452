using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ModuleBlockSyntax : TypeBlockSyntax
	{
		internal readonly ModuleStatementSyntax _moduleStatement;

		internal readonly EndBlockStatementSyntax _endModuleStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ModuleStatementSyntax ModuleStatement => _moduleStatement;

		internal EndBlockStatementSyntax EndModuleStatement => _endModuleStatement;

		public override TypeStatementSyntax BlockStatement => ModuleStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndModuleStatement;

		internal ModuleBlockSyntax(SyntaxKind kind, ModuleStatementSyntax moduleStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endModuleStatement)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(moduleStatement);
			_moduleStatement = moduleStatement;
			AdjustFlagsAndWidth(endModuleStatement);
			_endModuleStatement = endModuleStatement;
		}

		internal ModuleBlockSyntax(SyntaxKind kind, ModuleStatementSyntax moduleStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endModuleStatement, ISyntaxFactoryContext context)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(moduleStatement);
			_moduleStatement = moduleStatement;
			AdjustFlagsAndWidth(endModuleStatement);
			_endModuleStatement = endModuleStatement;
		}

		internal ModuleBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModuleStatementSyntax moduleStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endModuleStatement)
			: base(kind, errors, annotations, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(moduleStatement);
			_moduleStatement = moduleStatement;
			AdjustFlagsAndWidth(endModuleStatement);
			_endModuleStatement = endModuleStatement;
		}

		internal ModuleBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			ModuleStatementSyntax moduleStatementSyntax = (ModuleStatementSyntax)reader.ReadValue();
			if (moduleStatementSyntax != null)
			{
				AdjustFlagsAndWidth(moduleStatementSyntax);
				_moduleStatement = moduleStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endModuleStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_moduleStatement);
			writer.WriteValue(_endModuleStatement);
		}

		static ModuleBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ModuleBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ModuleBlockSyntax), (ObjectReader r) => new ModuleBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _moduleStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endModuleStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ModuleBlockSyntax(base.Kind, newErrors, GetAnnotations(), _moduleStatement, _inherits, _implements, _members, _endModuleStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ModuleBlockSyntax(base.Kind, GetDiagnostics(), annotations, _moduleStatement, _inherits, _implements, _members, _endModuleStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitModuleBlock(this);
		}
	}
}
