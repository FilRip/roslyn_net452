using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedEventAccessorSymbol : SynthesizedAccessor<SourceEventSymbol>
	{
		private TypeSymbol _lazyReturnType;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private ImmutableArray<MethodSymbol> _lazyExplicitImplementations;

		private SourceEventSymbol SourceEvent => m_propertyOrEvent;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyExplicitImplementations, SourceEvent.GetAccessorImplementations(MethodKind));
				}
				return _lazyExplicitImplementations;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					TypeSymbol type;
					if (MethodKind == MethodKind.EventRemove && m_propertyOrEvent.IsWindowsRuntimeEvent)
					{
						type = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
						instance.Add(Binder.GetUseSiteInfoForWellKnownType(type), base.Locations[0]);
					}
					else
					{
						type = SourceEvent.Type;
					}
					ImmutableArray<ParameterSymbol> value = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, type, 0, isByRef: false, "obj"));
					((SourceModuleSymbol)ContainingModule).AtomicStoreArrayAndDiagnostics(ref _lazyParameters, value, instance);
					instance.Free();
				}
				return _lazyParameters;
			}
		}

		public override TypeSymbol ReturnType
		{
			get
			{
				if ((object)_lazyReturnType == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					VisualBasicCompilation declaringCompilation = DeclaringCompilation;
					TypeSymbol typeSymbol;
					UseSiteInfo<AssemblySymbol> info;
					if (IsSub)
					{
						typeSymbol = declaringCompilation.GetSpecialType(SpecialType.System_Void);
						info = ((MethodKind == MethodKind.EventRemove) ? Binder.GetUseSiteInfoForSpecialType(typeSymbol) : default(UseSiteInfo<AssemblySymbol>));
					}
					else
					{
						typeSymbol = declaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
						info = Binder.GetUseSiteInfoForWellKnownType(typeSymbol);
					}
					instance.Add(info, base.Locations[0]);
					((SourceModuleSymbol)ContainingModule).AtomicStoreReferenceAndDiagnostics(ref _lazyReturnType, typeSymbol, instance);
					instance.Free();
				}
				return _lazyReturnType;
			}
		}

		public override bool IsSub
		{
			get
			{
				if (MethodKind == MethodKind.EventAdd)
				{
					return !m_propertyOrEvent.IsWindowsRuntimeEvent;
				}
				return true;
			}
		}

		internal sealed override bool GenerateDebugInfoImpl => false;

		internal override MethodImplAttributes ImplementationAttributes
		{
			get
			{
				MethodImplAttributes methodImplAttributes = base.ImplementationAttributes;
				if (!base.IsMustOverride && !SourceEvent.IsWindowsRuntimeEvent && !TypeSymbolExtensions.IsStructureType(base.ContainingType) && (object)DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember.System_Threading_Interlocked__CompareExchange_T) == null)
				{
					methodImplAttributes |= MethodImplAttributes.Synchronized;
				}
				return methodImplAttributes;
			}
		}

		protected SynthesizedEventAccessorSymbol(SourceMemberContainerTypeSymbol container, SourceEventSymbol @event)
			: base((NamedTypeSymbol)container, @event)
		{
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			return ConstructFieldLikeEventAccessorBody(m_propertyOrEvent, MethodKind == MethodKind.EventAdd, declaringCompilation, diagnostics);
		}

		protected static BoundBlock ConstructFieldLikeEventAccessorBody(SourceEventSymbol eventSymbol, bool isAddMethod, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			return (eventSymbol.IsWindowsRuntimeEvent ? ConstructFieldLikeEventAccessorBody_WinRT(eventSymbol, isAddMethod, compilation, diagnostics) : ConstructFieldLikeEventAccessorBody_Regular(eventSymbol, isAddMethod, compilation, diagnostics)) ?? new BoundBlock((VisualBasicSyntaxNode)eventSymbol.SyntaxReference.GetSyntax(), default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty, hasErrors: true);
		}

		private static BoundBlock ConstructFieldLikeEventAccessorBody_WinRT(SourceEventSymbol eventSymbol, bool isAddMethod, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(eventSymbol.SyntaxReference);
			MethodSymbol methodSymbol = (isAddMethod ? eventSymbol.AddMethod : eventSymbol.RemoveMethod);
			FieldSymbol associatedField = eventSymbol.AssociatedField;
			NamedTypeSymbol type = (NamedTypeSymbol)associatedField.Type;
			if (TypeSymbolExtensions.IsErrorType(type))
			{
				return null;
			}
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			MethodSymbol methodSymbol2 = (MethodSymbol)Binder.GetWellKnownTypeMember(compilation, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable, out useSiteInfo);
			diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation());
			if ((object)methodSymbol2 == null)
			{
				return null;
			}
			methodSymbol2 = SymbolExtensions.AsMember(methodSymbol2, type);
			WellKnownMember member = (isAddMethod ? WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler : WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler);
			useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			MethodSymbol methodSymbol3 = (MethodSymbol)Binder.GetWellKnownTypeMember(compilation, member, out useSiteInfo);
			diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation());
			if ((object)methodSymbol3 == null)
			{
				return null;
			}
			methodSymbol3 = SymbolExtensions.AsMember(methodSymbol3, type);
			BoundFieldAccess item = BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(visualBasicSyntax, associatedField.IsShared ? null : new BoundMeReference(visualBasicSyntax, methodSymbol.MeParameter.Type), associatedField, isLValue: true, associatedField.Type));
			BoundCall receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(visualBasicSyntax, methodSymbol2, null, null, ImmutableArray.Create((BoundExpression)item), null, methodSymbol2.ReturnType));
			ParameterSymbol parameterSymbol = methodSymbol.Parameters.Single();
			BoundParameter item2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundParameter(visualBasicSyntax, parameterSymbol, isLValue: false, parameterSymbol.Type));
			BoundCall boundCall = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(visualBasicSyntax, methodSymbol3, null, receiverOpt, ImmutableArray.Create((BoundExpression)item2), null, methodSymbol3.ReturnType));
			if (isAddMethod)
			{
				BoundReturnStatement item3 = new BoundReturnStatement(visualBasicSyntax, boundCall, null, null);
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(visualBasicSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)item3)));
			}
			BoundExpressionStatement item4 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, boundCall));
			BoundReturnStatement item5 = new BoundReturnStatement(visualBasicSyntax, null, null, null);
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(visualBasicSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)item4, (BoundStatement)item5)));
		}

		private static BoundBlock ConstructFieldLikeEventAccessorBody_Regular(SourceEventSymbol eventSymbol, bool isAddMethod, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			if (!TypeSymbolExtensions.IsDelegateType(eventSymbol.Type))
			{
				return null;
			}
			VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(eventSymbol.SyntaxReference);
			TypeSymbol type = eventSymbol.Type;
			MethodSymbol methodSymbol = (isAddMethod ? eventSymbol.AddMethod : eventSymbol.RemoveMethod);
			ParameterSymbol meParameter = methodSymbol.MeParameter;
			TypeSymbol specialType = compilation.GetSpecialType(SpecialType.System_Boolean);
			SpecialMember member = (isAddMethod ? SpecialMember.System_Delegate__Combine : SpecialMember.System_Delegate__Remove);
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			MethodSymbol methodSymbol2 = (MethodSymbol)Binder.GetSpecialTypeMember(compilation.Assembly, member, out useSiteInfo);
			diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation());
			BoundStatement boundStatement = BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(visualBasicSyntax, null, null, null));
			if ((object)methodSymbol2 == null)
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(visualBasicSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundStatement)));
			}
			useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			MethodSymbol methodSymbol3 = (MethodSymbol)Binder.GetWellKnownTypeMember(compilation, WellKnownMember.System_Threading_Interlocked__CompareExchange_T, out useSiteInfo);
			BoundMeReference receiverOpt = (eventSymbol.IsShared ? null : BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(visualBasicSyntax, meParameter.Type)));
			FieldSymbol associatedField = eventSymbol.AssociatedField;
			BoundFieldAccess boundFieldAccess = BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(visualBasicSyntax, receiverOpt, associatedField, isLValue: true, associatedField.Type));
			ParameterSymbol parameterSymbol = methodSymbol.Parameters[0];
			BoundParameter boundParameter = BoundNodeExtensions.MakeCompilerGenerated(new BoundParameter(visualBasicSyntax, parameterSymbol, isLValue: false, parameterSymbol.Type));
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, compilation.Assembly);
			BoundExpression right;
			if ((object)methodSymbol3 == null)
			{
				Conversions.ClassifyDirectCastConversion(associatedField.Type, methodSymbol2.Parameters[0].Type, ref useSiteInfo2);
				Conversions.ClassifyDirectCastConversion(boundParameter.Type, methodSymbol2.Parameters[1].Type, ref useSiteInfo2);
				diagnostics.Add(visualBasicSyntax.GetLocation(), useSiteInfo2);
				right = BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(visualBasicSyntax, new BoundCall(visualBasicSyntax, methodSymbol2, null, null, ImmutableArray.Create((BoundExpression)new BoundDirectCast(visualBasicSyntax, boundFieldAccess.MakeRValue(), ConversionKind.WideningReference, methodSymbol2.Parameters[0].Type), (BoundExpression)new BoundDirectCast(visualBasicSyntax, boundParameter, ConversionKind.WideningReference, methodSymbol2.Parameters[1].Type)), null, methodSymbol2.ReturnType), ConversionKind.NarrowingReference, type, TypeSymbolExtensions.IsErrorType(type)));
				BoundStatement item = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(visualBasicSyntax, boundFieldAccess, right, suppressObjectClone: true, type))));
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(visualBasicSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item, boundStatement)));
			}
			diagnostics.Add(useSiteInfo, visualBasicSyntax.GetLocation());
			methodSymbol3 = methodSymbol3.Construct(ImmutableArray.Create(type));
			GeneratedLabelSymbol label = new GeneratedLabelSymbol("LOOP");
			LocalSymbol[] array = new LocalSymbol[3];
			BoundLocal[] array2 = new BoundLocal[3];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SynthesizedLocal(methodSymbol, type, SynthesizedLocalKind.LoweringTemp);
				array2[i] = new BoundLocal(visualBasicSyntax, array[i], type);
			}
			BoundStatement boundStatement2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(visualBasicSyntax, array2[0], boundFieldAccess.MakeRValue(), suppressObjectClone: true, type))));
			BoundStatement boundStatement3 = BoundNodeExtensions.MakeCompilerGenerated(new BoundLabelStatement(visualBasicSyntax, label));
			BoundStatement boundStatement4 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(visualBasicSyntax, array2[1], array2[0].MakeRValue(), suppressObjectClone: true, type))));
			TypeSymbol type2 = array2[1].Type;
			TypeSymbol type3 = methodSymbol2.Parameters[0].Type;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo3 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			Conversions.ClassifyDirectCastConversion(type2, type3, ref useSiteInfo3);
			TypeSymbol type4 = boundParameter.Type;
			TypeSymbol type5 = methodSymbol2.Parameters[1].Type;
			useSiteInfo3 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			Conversions.ClassifyDirectCastConversion(type4, type5, ref useSiteInfo3);
			diagnostics.Add(visualBasicSyntax.GetLocation(), useSiteInfo2);
			right = BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(visualBasicSyntax, new BoundCall(visualBasicSyntax, methodSymbol2, null, null, ImmutableArray.Create((BoundExpression)new BoundDirectCast(visualBasicSyntax, array2[1].MakeRValue(), ConversionKind.WideningReference, methodSymbol2.Parameters[0].Type), (BoundExpression)new BoundDirectCast(visualBasicSyntax, boundParameter, ConversionKind.WideningReference, methodSymbol2.Parameters[1].Type)), null, methodSymbol2.ReturnType), ConversionKind.NarrowingReference, type, TypeSymbolExtensions.IsErrorType(type)));
			BoundStatement boundStatement5 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(visualBasicSyntax, array2[2], right, suppressObjectClone: true, type))));
			BoundExpression right2 = new BoundCall(visualBasicSyntax, methodSymbol3, null, null, ImmutableArray.Create<BoundExpression>(boundFieldAccess, array2[2].MakeRValue(), array2[1].MakeRValue()), null, methodSymbol3.ReturnType);
			BoundStatement boundStatement6 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(visualBasicSyntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(visualBasicSyntax, array2[0], right2, suppressObjectClone: true, type))));
			BoundExpression condition = BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(visualBasicSyntax, BinaryOperatorKind.Is, array2[0].MakeRValue(), array2[1].MakeRValue(), @checked: false, specialType));
			BoundStatement boundStatement7 = BoundNodeExtensions.MakeCompilerGenerated(new BoundConditionalGoto(visualBasicSyntax, condition, jumpIfTrue: false, label));
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(visualBasicSyntax, default(SyntaxList<StatementSyntax>), array.AsImmutable(), ImmutableArray.Create<BoundStatement>(boundStatement2, boundStatement3, boundStatement4, boundStatement5, boundStatement6, boundStatement7, boundStatement)));
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_ = Parameters;
			_ = ReturnType;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
