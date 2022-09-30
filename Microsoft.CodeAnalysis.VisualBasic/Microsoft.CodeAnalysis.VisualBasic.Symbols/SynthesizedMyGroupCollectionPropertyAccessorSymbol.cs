using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedMyGroupCollectionPropertyAccessorSymbol : SynthesizedPropertyAccessorBase<SynthesizedMyGroupCollectionPropertySymbol>
	{
		private readonly string _createOrDisposeMethod;

		internal override FieldSymbol BackingFieldSymbol => base.PropertyOrEvent.AssociatedField;

		internal sealed override bool GenerateDebugInfoImpl => false;

		public SynthesizedMyGroupCollectionPropertyAccessorSymbol(SourceNamedTypeSymbol container, SynthesizedMyGroupCollectionPropertySymbol property, string createOrDisposeMethod)
			: base((NamedTypeSymbol)container, property)
		{
			_createOrDisposeMethod = createOrDisposeMethod;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeDebuggerHiddenAttribute());
		}

		private static string MakeSafeName(string name)
		{
			if (SyntaxFacts.GetKeywordKind(name) != 0)
			{
				return "[" + name + "]";
			}
			return name;
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out Binder methodBodyBinder = null)
		{
			SourceNamedTypeSymbol sourceNamedTypeSymbol = (SourceNamedTypeSymbol)base.ContainingType;
			string text = MakeSafeName(sourceNamedTypeSymbol.Name);
			string targetTypeName = base.PropertyOrEvent.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			string text2 = MakeSafeName(base.PropertyOrEvent.Name);
			string name = base.PropertyOrEvent.AssociatedField.Name;
			SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText("Partial Class " + text + "\r\nProperty " + text2 + "\r\n" + GetMethodBlock(name, MakeSafeName(_createOrDisposeMethod), targetTypeName) + "End Property\r\nEnd Class\r\n");
			Location location = VisualBasicExtensions.GetVisualBasicSyntax(base.PropertyOrEvent.AttributeSyntax).GetLocation();
			CompilationUnitSyntax compilationUnitRoot = VisualBasicExtensions.GetCompilationUnitRoot(syntaxTree);
			bool flag = false;
			foreach (Diagnostic diagnostic in syntaxTree.GetDiagnostics(compilationUnitRoot))
			{
				VBDiagnostic vBDiagnostic = (VBDiagnostic)diagnostic;
				diagnostics.Add(vBDiagnostic.WithLocation(location));
				if (diagnostic.Severity == DiagnosticSeverity.Error)
				{
					flag = true;
				}
			}
			AccessorBlockSyntax accessorBlockSyntax = ((PropertyBlockSyntax)((ClassBlockSyntax)compilationUnitRoot.Members[0]).Members[0]).Accessors[0];
			BoundStatement boundStatement;
			if (flag)
			{
				boundStatement = new BoundBadStatement(accessorBlockSyntax, ImmutableArray<BoundNode>.Empty);
			}
			else
			{
				Binder containingBinder = BinderBuilder.CreateBinderForType(sourceNamedTypeSymbol.ContainingSourceModule, base.PropertyOrEvent.AttributeSyntax.SyntaxTree, sourceNamedTypeSymbol);
				methodBodyBinder = BinderBuilder.CreateBinderForMethodBody(this, accessorBlockSyntax, containingBinder);
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance(), diagnostics.DependenciesBag);
				boundStatement = methodBodyBinder.BindStatement(accessorBlockSyntax, bindingDiagnosticBag);
				foreach (VBDiagnostic item in bindingDiagnosticBag.DiagnosticBag!.AsEnumerable())
				{
					diagnostics.Add(item.WithLocation(location));
				}
				bindingDiagnosticBag.DiagnosticBag!.Free();
				if (boundStatement.Kind == BoundKind.Block)
				{
					return (BoundBlock)boundStatement;
				}
			}
			return new BoundBlock(accessorBlockSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundStatement));
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected abstract string GetMethodBlock(string fieldName, string createOrDisposeMethodName, string targetTypeName);
	}
}
