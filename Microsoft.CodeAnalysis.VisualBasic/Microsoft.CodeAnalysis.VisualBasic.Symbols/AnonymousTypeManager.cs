using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class AnonymousTypeManager : CommonAnonymousTypeManager
	{
		private sealed class AnonymousTypeConstructorSymbol : SynthesizedConstructorBase
		{
			private ImmutableArray<ParameterSymbol> _parameters;

			internal override int ParameterCount => _parameters.Length;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			internal override bool GenerateDebugInfoImpl => false;

			internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
			{
				methodBodyBinder = null;
				SyntaxNode syntax = base.Syntax;
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol = (AnonymousTypeTemplateSymbol)base.ContainingType;
				BoundMeReference receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(syntax, anonymousTypeTemplateSymbol));
				int num = ParameterCount - 1;
				for (int i = 0; i <= num; i++)
				{
					AnonymousTypePropertySymbol anonymousTypePropertySymbol = anonymousTypeTemplateSymbol.Properties[i];
					TypeSymbol type = anonymousTypePropertySymbol.Type;
					BoundFieldAccess left = BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(syntax, receiverOpt, anonymousTypePropertySymbol.AssociatedField, isLValue: true, type));
					BoundParameter right = BoundNodeExtensions.MakeCompilerGenerated(new BoundParameter(syntax, _parameters[i], isLValue: false, type));
					BoundAssignmentOperator expression = BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, left, right, suppressObjectClone: false, type));
					instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, expression)));
				}
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, null, null, null)));
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree()));
			}

			public AnonymousTypeConstructorSymbol(AnonymousTypeTemplateSymbol container)
				: base(VisualBasicSyntaxTree.DummyReference, container, isShared: false, null, null)
			{
				int length = container.Properties.Length;
				ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					PropertySymbol propertySymbol = container.Properties[i];
					array[i] = new AnonymousTypeOrDelegateParameterSymbol(this, propertySymbol.Type, i, isByRef: false, propertySymbol.Name, i);
				}
				_parameters = array.AsImmutableOrNull();
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation compilation = ((AnonymousTypeTemplateSymbol)base.ContainingType).Manager.Compilation;
				Symbol.AddSynthesizedAttribute(ref attributes, compilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypeEqualsMethodSymbol : SynthesizedRegularMethodBase
		{
			private readonly ImmutableArray<ParameterSymbol> _parameters;

			private readonly MethodSymbol _iEquatableEqualsMethod;

			private AnonymousTypeTemplateSymbol AnonymousType => (AnonymousTypeTemplateSymbol)m_containingType;

			public override bool IsOverrides => true;

			public override bool IsOverridable => false;

			public override bool IsOverloads => true;

			internal override int ParameterCount => 1;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override MethodSymbol OverriddenMethod => AnonymousType.Manager.System_Object__Equals;

			public override Accessibility DeclaredAccessibility => Accessibility.Public;

			public override bool IsSub => false;

			public override TypeSymbol ReturnType => AnonymousType.Manager.System_Boolean;

			internal override bool GenerateDebugInfoImpl => false;

			internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
			{
				methodBodyBinder = null;
				SyntaxNode syntax = base.Syntax;
				BoundMeReference receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(syntax, AnonymousType));
				BoundParameter operand = BoundNodeExtensions.MakeCompilerGenerated(new BoundParameter(syntax, _parameters[0], isLValue: false, AnonymousType.Manager.System_Object));
				BoundExpression item = BoundNodeExtensions.MakeCompilerGenerated(new BoundTryCast(syntax, operand, ConversionKind.NarrowingReference, AnonymousType));
				BoundExpression expressionOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, _iEquatableEqualsMethod, null, receiverOpt, ImmutableArray.Create(item), null, AnonymousType.Manager.System_Boolean));
				return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, expressionOpt, null, null))));
			}

			public AnonymousTypeEqualsMethodSymbol(AnonymousTypeTemplateSymbol container, MethodSymbol iEquatableEqualsMethod)
				: base(VisualBasicSyntaxTree.Dummy.GetRoot(), container, "Equals")
			{
				_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSimpleSymbol(this, container.Manager.System_Object, 0, "obj"));
				_iEquatableEqualsMethod = iEquatableEqualsMethod;
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation compilation = ((AnonymousTypeTemplateSymbol)base.ContainingType).Manager.Compilation;
				Symbol.AddSynthesizedAttribute(ref attributes, compilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypeGetHashCodeMethodSymbol : SynthesizedRegularMethodBase
		{
			private AnonymousTypeTemplateSymbol AnonymousType => (AnonymousTypeTemplateSymbol)m_containingType;

			public override bool IsOverrides => true;

			public override MethodSymbol OverriddenMethod => AnonymousType.Manager.System_Object__GetHashCode;

			public override Accessibility DeclaredAccessibility => Accessibility.Public;

			public override bool IsSub => false;

			public override TypeSymbol ReturnType => AnonymousType.Manager.System_Int32;

			internal override bool GenerateDebugInfoImpl => false;

			internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
			{
				methodBodyBinder = null;
				SyntaxNode syntax = base.Syntax;
				TypeSymbol system_Object = AnonymousType.Manager.System_Object;
				MethodSymbol system_Object__GetHashCode = AnonymousType.Manager.System_Object__GetHashCode;
				TypeSymbol system_Int = AnonymousType.Manager.System_Int32;
				TypeSymbol system_Boolean = AnonymousType.Manager.System_Boolean;
				ImmutableArray<AnonymousTypePropertySymbol> properties = AnonymousType.Properties;
				string[] array = new string[properties.Length - 1 + 1];
				int num = properties.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = properties[i].Name;
				}
				int value = (int)CRC32.ComputeCRC32(array);
				BoundMeReference receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(syntax, AnonymousType));
				BoundExpression boundExpression = null;
				boundExpression = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(value), system_Int));
				BoundLiteral right = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(-1521134295), system_Int));
				BoundLiteral whenTrue = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(0), system_Int));
				BoundLiteral right2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Nothing, system_Object));
				ImmutableArray<AnonymousTypePropertySymbol>.Enumerator enumerator = AnonymousType.Properties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AnonymousTypePropertySymbol current = enumerator.Current;
					if (current.IsReadOnly)
					{
						boundExpression = BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(syntax, BinaryOperatorKind.Multiply, boundExpression, right, @checked: false, system_Int));
						BoundBinaryOperator condition = BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(syntax, BinaryOperatorKind.Is, BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(syntax, receiverOpt, current.AssociatedField, isLValue: false, current.Type)), ConversionKind.WideningTypeParameter, system_Object)), right2, @checked: false, system_Boolean));
						BoundCall whenFalse = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, system_Object__GetHashCode, null, BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(syntax, receiverOpt, current.AssociatedField, isLValue: false, current.Type)), ImmutableArray<BoundExpression>.Empty, null, system_Int));
						BoundTernaryConditionalExpression right3 = BoundNodeExtensions.MakeCompilerGenerated(new BoundTernaryConditionalExpression(syntax, condition, whenTrue, whenFalse, null, system_Int));
						boundExpression = BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(syntax, BinaryOperatorKind.Add, boundExpression, right3, @checked: false, system_Int));
					}
				}
				return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, boundExpression, null, null))));
			}

			public AnonymousTypeGetHashCodeMethodSymbol(AnonymousTypeTemplateSymbol container)
				: base(VisualBasicSyntaxTree.Dummy.GetRoot(), container, "GetHashCode")
			{
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation compilation = ((AnonymousTypeTemplateSymbol)base.ContainingType).Manager.Compilation;
				Symbol.AddSynthesizedAttribute(ref attributes, compilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousType_IEquatable_EqualsMethodSymbol : SynthesizedRegularMethodBase
		{
			private readonly ImmutableArray<ParameterSymbol> _parameters;

			private readonly ImmutableArray<MethodSymbol> _interfaceMethod;

			private AnonymousTypeTemplateSymbol AnonymousType => (AnonymousTypeTemplateSymbol)m_containingType;

			public override bool IsOverrides => false;

			public override bool IsOverridable => false;

			public override bool IsNotOverridable => false;

			public override bool IsOverloads => true;

			internal override int ParameterCount => 1;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _interfaceMethod;

			public override Accessibility DeclaredAccessibility => Accessibility.Public;

			public override bool IsSub => false;

			public override TypeSymbol ReturnType => AnonymousType.Manager.System_Boolean;

			internal override bool GenerateDebugInfoImpl => false;

			internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
			{
				methodBodyBinder = null;
				SyntaxNode syntax = base.Syntax;
				TypeSymbol system_Object = AnonymousType.Manager.System_Object;
				TypeSymbol system_Boolean = AnonymousType.Manager.System_Boolean;
				LocalSymbol localSymbol = new SynthesizedLocal(this, system_Object, SynthesizedLocalKind.LoweringTemp);
				LocalSymbol localSymbol2 = new SynthesizedLocal(this, system_Object, SynthesizedLocalKind.LoweringTemp);
				BoundMeReference boundMeReference = new BoundMeReference(syntax, AnonymousType);
				BoundParameter boundParameter = new BoundParameter(syntax, _parameters[0], isLValue: false, AnonymousType);
				BoundLiteral boundLiteral = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Nothing, system_Object));
				BoundExpression right = BuildConditionsForFields(boundMeReference, boundParameter, boundLiteral, localSymbol, localSymbol2, system_Boolean);
				BoundExpression left = BuildIsCheck(BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(syntax, boundParameter, ConversionKind.WideningReference, system_Object)), boundLiteral, system_Boolean, reverse: true);
				BoundExpression right2 = BuildAndAlso(left, right, system_Boolean);
				BoundExpression left2 = BuildIsCheck(boundMeReference, boundParameter, system_Boolean);
				right2 = BuildOrElse(left2, right2, system_Boolean);
				return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(localSymbol, localSymbol2), ImmutableArray.Create((BoundStatement)BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, right2, null, null))));
			}

			private BoundExpression BuildConditionsForFields(BoundMeReference boundMe, BoundParameter boundOther, BoundExpression boundNothing, LocalSymbol localMyFieldBoxed, LocalSymbol localOtherFieldBoxed, TypeSymbol booleanType)
			{
				BoundExpression boundExpression = null;
				ImmutableArray<AnonymousTypePropertySymbol>.Enumerator enumerator = AnonymousType.Properties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AnonymousTypePropertySymbol current = enumerator.Current;
					if (current.IsReadOnly)
					{
						BoundExpression boundExpression2 = BuildConditionForField(current, boundMe, boundOther, boundNothing, localMyFieldBoxed, localOtherFieldBoxed, booleanType);
						boundExpression = ((boundExpression == null) ? boundExpression2 : BuildAndAlso(boundExpression, boundExpression2, booleanType));
					}
				}
				return boundExpression;
			}

			private BoundExpression BuildConditionForField(AnonymousTypePropertySymbol property, BoundMeReference boundMe, BoundParameter boundOther, BoundExpression boundNothing, LocalSymbol localMyFieldBoxed, LocalSymbol localOtherFieldBoxed, TypeSymbol booleanType)
			{
				FieldSymbol associatedField = property.AssociatedField;
				SyntaxNode syntax = base.Syntax;
				BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, localMyFieldBoxed, isLValue: false, localMyFieldBoxed.Type));
				BoundLocal boundLocal2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, localOtherFieldBoxed, isLValue: false, localOtherFieldBoxed.Type));
				BoundExpression condition = BuildAndAlso(BuildIsCheck(boundLocal, boundNothing, booleanType, reverse: true), BuildIsCheck(boundLocal2, boundNothing, booleanType, reverse: true), booleanType);
				BoundExpression whenTrue = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, AnonymousType.Manager.System_Object__Equals, null, boundLocal, ImmutableArray.Create((BoundExpression)boundLocal2), null, booleanType));
				BoundExpression boundExpression = BoundNodeExtensions.MakeCompilerGenerated(new BoundTernaryConditionalExpression(syntax, condition, whenTrue, BuildIsCheck(boundLocal, boundLocal2, booleanType), null, booleanType));
				BoundExpression item = BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, new BoundLocal(syntax, localMyFieldBoxed, isLValue: true, localMyFieldBoxed.Type), BuildBoxedFieldAccess(boundMe, associatedField), suppressObjectClone: true, localMyFieldBoxed.Type));
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundSequence(sideEffects: ImmutableArray.Create(item, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, new BoundLocal(syntax, localOtherFieldBoxed, isLValue: true, localOtherFieldBoxed.Type), BuildBoxedFieldAccess(boundOther, associatedField), suppressObjectClone: true, localOtherFieldBoxed.Type))), syntax: syntax, locals: ImmutableArray<LocalSymbol>.Empty, valueOpt: boundExpression, type: boundExpression.Type));
			}

			private BoundExpression BuildBoxedFieldAccess(BoundExpression receiver, FieldSymbol field)
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(base.Syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(base.Syntax, receiver, field, isLValue: false, field.Type)), ConversionKind.WideningTypeParameter, AnonymousType.Manager.System_Object));
			}

			private BoundExpression BuildIsCheck(BoundExpression left, BoundExpression right, TypeSymbol booleanType, bool reverse = false)
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(base.Syntax, reverse ? BinaryOperatorKind.IsNot : BinaryOperatorKind.Is, left, right, @checked: false, booleanType));
			}

			private BoundExpression BuildAndAlso(BoundExpression left, BoundExpression right, TypeSymbol booleanType)
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(base.Syntax, BinaryOperatorKind.AndAlso, left, right, @checked: false, booleanType));
			}

			private BoundExpression BuildOrElse(BoundExpression left, BoundExpression right, TypeSymbol booleanType)
			{
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(base.Syntax, BinaryOperatorKind.OrElse, left, right, @checked: false, booleanType));
			}

			public AnonymousType_IEquatable_EqualsMethodSymbol(AnonymousTypeTemplateSymbol container, MethodSymbol interfaceMethod)
				: base(VisualBasicSyntaxTree.Dummy.GetRoot(), container, "Equals")
			{
				_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSimpleSymbol(this, container, 0, "val"));
				_interfaceMethod = ImmutableArray.Create(interfaceMethod);
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation compilation = ((AnonymousTypeTemplateSymbol)base.ContainingType).Manager.Compilation;
				Symbol.AddSynthesizedAttribute(ref attributes, compilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypeToStringMethodSymbol : SynthesizedRegularMethodBase
		{
			private AnonymousTypeTemplateSymbol AnonymousType => (AnonymousTypeTemplateSymbol)m_containingType;

			public override bool IsOverrides => true;

			public override MethodSymbol OverriddenMethod => AnonymousType.Manager.System_Object__ToString;

			public override Accessibility DeclaredAccessibility => Accessibility.Public;

			public override bool IsSub => false;

			public override TypeSymbol ReturnType => AnonymousType.Manager.System_String;

			internal override bool GenerateDebugInfoImpl => false;

			internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
			{
				methodBodyBinder = null;
				SyntaxNode syntax = base.Syntax;
				TypeSymbol system_Object = AnonymousType.Manager.System_Object;
				TypeSymbol returnType = ReturnType;
				TypeSymbol type = AnonymousType.Manager.Compilation.CreateArrayTypeSymbol(system_Object);
				int length = AnonymousType.Properties.Length;
				BoundMeReference receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(syntax, AnonymousType));
				BoundExpression[] array = new BoundExpression[length - 1 + 1];
				PooledStringBuilder instance = PooledStringBuilder.GetInstance();
				instance.Builder.Append("{{ ");
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					AnonymousTypePropertySymbol anonymousTypePropertySymbol = AnonymousType.Properties[i];
					instance.Builder.AppendFormat((i == 0) ? "{0} = {{{1}}}" : ", {0} = {{{1}}}", anonymousTypePropertySymbol.MetadataName, i);
					array[i] = BoundNodeExtensions.MakeCompilerGenerated(new BoundDirectCast(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundFieldAccess(syntax, receiverOpt, anonymousTypePropertySymbol.AssociatedField, isLValue: false, anonymousTypePropertySymbol.Type)), ConversionKind.WideningTypeParameter, system_Object));
				}
				instance.Builder.Append(" }}");
				string value = instance.ToStringAndFree();
				BoundArrayInitialization initializerOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundArrayInitialization(syntax, array.AsImmutableOrNull(), type));
				BoundExpression item = BoundNodeExtensions.MakeCompilerGenerated(new BoundArrayCreation(syntax, ImmutableArray.Create((BoundExpression)BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(length), AnonymousType.Manager.System_Int32))), initializerOpt, type));
				MethodSymbol system_String__Format_IFormatProvider = AnonymousType.Manager.System_String__Format_IFormatProvider;
				BoundExpression expressionOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, system_String__Format_IFormatProvider, null, null, ImmutableArray.Create(BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Nothing, system_String__Format_IFormatProvider.Parameters[0].Type)), BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(value), returnType)), item), null, returnType));
				return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, expressionOpt, null, null))));
			}

			public AnonymousTypeToStringMethodSymbol(AnonymousTypeTemplateSymbol container)
				: base(VisualBasicSyntaxTree.Dummy.GetRoot(), container, "ToString")
			{
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				VisualBasicCompilation compilation = ((AnonymousTypeTemplateSymbol)base.ContainingType).Manager.Compilation;
				Symbol.AddSynthesizedAttribute(ref attributes, compilation.SynthesizeDebuggerHiddenAttribute());
			}

			internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypeComparer : IComparer<AnonymousTypeOrDelegateTemplateSymbol>
		{
			private readonly VisualBasicCompilation _compilation;

			internal AnonymousTypeComparer(VisualBasicCompilation compilation)
			{
				_compilation = compilation;
			}

			public int Compare(AnonymousTypeOrDelegateTemplateSymbol x, AnonymousTypeOrDelegateTemplateSymbol y)
			{
				if ((object)x == y)
				{
					return 0;
				}
				int num = CompareLocations(x.SmallestLocation, y.SmallestLocation);
				if (num == 0)
				{
					num = x.TypeDescriptorKey.CompareTo(y.TypeDescriptorKey);
				}
				return num;
			}

			int IComparer<AnonymousTypeOrDelegateTemplateSymbol>.Compare(AnonymousTypeOrDelegateTemplateSymbol x, AnonymousTypeOrDelegateTemplateSymbol y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(x, y);
			}

			private int CompareLocations(Location x, Location y)
			{
				if ((object)x == y)
				{
					return 0;
				}
				if (x == Location.None)
				{
					return -1;
				}
				if (y == Location.None)
				{
					return 1;
				}
				return _compilation.CompareSourceLocations(x, y);
			}
		}

		internal sealed class AnonymousDelegatePublicSymbol : AnonymousTypeOrDelegatePublicSymbol
		{
			private readonly ImmutableArray<SynthesizedDelegateMethodSymbol> _members;

			public override TypeKind TypeKind => TypeKind.Delegate;

			internal override bool IsInterface => false;

			public override MethodSymbol DelegateInvokeMethod => _members[_members.Length - 1];

			public AnonymousDelegatePublicSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
				: base(manager, typeDescr)
			{
				ImmutableArray<AnonymousTypeField> parameters = typeDescr.Parameters;
				TypeSymbol returnType = (AnonymousTypeExtensions.IsSubDescription(parameters) ? manager.System_Void : parameters.Last().Type);
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(parameters.Length + 1);
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol = new SynthesizedDelegateMethodSymbol("Invoke", this, SourceMemberFlags.MethodKindDelegateInvoke | SourceMemberFlags.Overridable, returnType);
				int num = parameters.Length - 2;
				int i;
				for (i = 0; i <= num; i++)
				{
					instance.Add(ParameterFromField(synthesizedDelegateMethodSymbol, parameters[i], i));
				}
				synthesizedDelegateMethodSymbol.SetParameters(instance.ToImmutable());
				instance.Clear();
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol2 = new SynthesizedDelegateMethodSymbol(".ctor", this, SourceMemberFlags.Static, manager.System_Void);
				synthesizedDelegateMethodSymbol2.SetParameters(ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol2, manager.System_Object, 0, isByRef: false, "TargetObject"), (ParameterSymbol)new SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol2, manager.System_IntPtr, 1, isByRef: false, "TargetMethod")));
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol3;
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol4;
				if (SymbolExtensions.IsCompilationOutputWinMdObj(this))
				{
					synthesizedDelegateMethodSymbol3 = null;
					synthesizedDelegateMethodSymbol4 = null;
					instance.Free();
					_members = ImmutableArray.Create(synthesizedDelegateMethodSymbol2, synthesizedDelegateMethodSymbol);
					return;
				}
				synthesizedDelegateMethodSymbol3 = new SynthesizedDelegateMethodSymbol("BeginInvoke", this, SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable, manager.System_IAsyncResult);
				int num2 = parameters.Length - 2;
				for (i = 0; i <= num2; i++)
				{
					instance.Add(ParameterFromField(synthesizedDelegateMethodSymbol3, parameters[i], i));
				}
				instance.Add(new SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_AsyncCallback, i, isByRef: false, "DelegateCallback"));
				i++;
				instance.Add(new SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_Object, i, isByRef: false, "DelegateAsyncState"));
				synthesizedDelegateMethodSymbol3.SetParameters(instance.ToImmutable());
				instance.Clear();
				synthesizedDelegateMethodSymbol4 = new SynthesizedDelegateMethodSymbol("EndInvoke", this, SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable, returnType);
				int num3 = 0;
				int num4 = parameters.Length - 2;
				for (i = 0; i <= num4; i++)
				{
					if (parameters[i].IsByRef)
					{
						instance.Add(ParameterFromField(synthesizedDelegateMethodSymbol4, parameters[i], num3));
						num3++;
					}
				}
				instance.Add(new SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol4, manager.System_IAsyncResult, num3, isByRef: false, "DelegateAsyncResult"));
				synthesizedDelegateMethodSymbol4.SetParameters(instance.ToImmutableAndFree());
				_members = ImmutableArray.Create(synthesizedDelegateMethodSymbol2, synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol4, synthesizedDelegateMethodSymbol);
			}

			private static ParameterSymbol ParameterFromField(SynthesizedDelegateMethodSymbol container, AnonymousTypeField field, int ordinal)
			{
				return new SynthesizedParameterWithLocationSymbol(container, field.Type, ordinal, field.IsByRef, field.Name, field.Location);
			}

			public override ImmutableArray<Symbol> GetMembers()
			{
				return StaticCast<Symbol>.From(_members);
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
			{
				AnonymousTypeDescriptor newDescriptor = default(AnonymousTypeDescriptor);
				return TypeDescriptor.SubstituteTypeParametersIfNeeded(substitution, out newDescriptor) ? new TypeWithModifiers(Manager.ConstructAnonymousDelegateSymbol(newDescriptor)) : new TypeWithModifiers(this);
			}

			public override NamedTypeSymbol MapToImplementationSymbol()
			{
				return Manager.ConstructAnonymousDelegateImplementationSymbol(this);
			}

			internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
			{
				return Manager.System_MulticastDelegate;
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
		}

		internal abstract class AnonymousTypeOrDelegatePublicSymbol : InstanceTypeSymbol
		{
			public readonly AnonymousTypeManager Manager;

			public readonly AnonymousTypeDescriptor TypeDescriptor;

			public sealed override string Name => string.Empty;

			internal sealed override bool MangleName => false;

			internal sealed override bool HasSpecialName => false;

			public sealed override bool IsSerializable => false;

			internal override TypeLayout Layout => default(TypeLayout);

			internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

			public abstract override TypeKind TypeKind { get; }

			public override int Arity => 0;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

			public override bool IsMustInherit => false;

			public override bool IsNotInheritable => true;

			public override bool MightContainExtensionMethods => false;

			internal override bool HasCodeAnalysisEmbeddedAttribute => false;

			internal override bool HasVisualBasicEmbeddedAttribute => false;

			internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

			internal override string DefaultPropertyName => null;

			public override IEnumerable<string> MemberNames => new HashSet<string>(from member in GetMembers()
				select member.Name);

			public override Symbol ContainingSymbol => Manager.ContainingModule.GlobalNamespace;

			public override NamedTypeSymbol ContainingType => null;

			public override Accessibility DeclaredAccessibility => Accessibility.Internal;

			public override ImmutableArray<Location> Locations => ImmutableArray.Create(TypeDescriptor.Location);

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public override bool IsAnonymousType => true;

			public sealed override bool IsImplicitlyDeclared => TypeDescriptor.IsImplicitlyDeclared;

			internal override bool IsWindowsRuntimeImport => false;

			internal override bool ShouldAddWinRTMembers => false;

			internal override bool IsComImport => false;

			internal override TypeSymbol CoClassType => null;

			internal override bool HasDeclarativeSecurity => false;

			internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

			protected AnonymousTypeOrDelegatePublicSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
			{
				Manager = manager;
				TypeDescriptor = typeDescr;
			}

			internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
			{
				return MakeAcyclicBaseType(diagnostics);
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
			{
				return MakeAcyclicInterfaces(diagnostics);
			}

			public override ImmutableArray<Symbol> GetMembers(string name)
			{
				return ImmutableArray.CreateRange(from member in GetMembers()
					where CaseInsensitiveComparison.Equals(member.Name, name)
					select member);
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			internal override ImmutableArray<string> GetAppliedConditionalSymbols()
			{
				return ImmutableArray<string>.Empty;
			}

			internal override AttributeUsageInfo GetAttributeUsageInfo()
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal abstract override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution);

			internal sealed override IEnumerable<FieldSymbol> GetFieldsToEmit()
			{
				throw ExceptionUtilities.Unreachable;
			}

			public abstract NamedTypeSymbol MapToImplementationSymbol();

			public MethodSymbol MapMethodToImplementationSymbol(MethodSymbol method)
			{
				return FindMethodInTypeProvided(method, MapToImplementationSymbol());
			}

			public MethodSymbol FindSubstitutedMethodSymbol(MethodSymbol method)
			{
				return FindMethodInTypeProvided(method, this);
			}

			private static MethodSymbol FindMethodInTypeProvided(MethodSymbol method, NamedTypeSymbol type)
			{
				if (type.IsDefinition)
				{
					int num = 0;
					ImmutableArray<Symbol>.Enumerator enumerator = method.ContainingType.GetMembers().GetEnumerator();
					while (enumerator.MoveNext() && (object)enumerator.Current != method)
					{
						num++;
					}
					return (MethodSymbol)type.GetMembers()[num];
				}
				NamedTypeSymbol originalDefinition = type.OriginalDefinition;
				MethodSymbol member = FindMethodInTypeProvided(method, originalDefinition);
				return (MethodSymbol)((SubstitutedNamedType)type).GetMemberForDefinition(member);
			}

			internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
			{
				return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
			}

			public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
			{
				return Equals(other as AnonymousTypeOrDelegatePublicSymbol, comparison);
			}

			public bool Equals(AnonymousTypeOrDelegatePublicSymbol other, TypeCompareKind comparison)
			{
				if ((object)this == other)
				{
					return true;
				}
				return (object)other != null && TypeKind == other.TypeKind && TypeDescriptor.Equals(other.TypeDescriptor, comparison);
			}

			public sealed override int GetHashCode()
			{
				AnonymousTypeDescriptor typeDescriptor = TypeDescriptor;
				return Hash.Combine(typeDescriptor.GetHashCode(), (int)TypeKind);
			}
		}

		private abstract class AnonymousTypePropertyAccessorPublicSymbol : SynthesizedPropertyAccessorBase<PropertySymbol>
		{
			private readonly TypeSymbol _returnType;

			internal sealed override FieldSymbol BackingFieldSymbol
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			public override TypeSymbol ReturnType => _returnType;

			internal sealed override bool GenerateDebugInfoImpl => false;

			public AnonymousTypePropertyAccessorPublicSymbol(PropertySymbol property, TypeSymbol returnType)
				: base(property.ContainingType, property)
			{
				_returnType = returnType;
			}

			internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypePropertyGetAccessorPublicSymbol : AnonymousTypePropertyAccessorPublicSymbol
		{
			public override bool IsSub => false;

			public override MethodKind MethodKind => MethodKind.PropertyGet;

			public AnonymousTypePropertyGetAccessorPublicSymbol(PropertySymbol property)
				: base(property, property.Type)
			{
			}
		}

		private sealed class AnonymousTypePropertySetAccessorPublicSymbol : AnonymousTypePropertyAccessorPublicSymbol
		{
			private ImmutableArray<ParameterSymbol> _parameters;

			public override bool IsSub => true;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override MethodKind MethodKind => MethodKind.PropertySet;

			public AnonymousTypePropertySetAccessorPublicSymbol(PropertySymbol property, TypeSymbol voidTypeSymbol)
				: base(property, voidTypeSymbol)
			{
				_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, m_propertyOrEvent.Type, 0, isByRef: false, "Value"));
			}
		}

		internal sealed class AnonymousTypePropertyPublicSymbol : SynthesizedPropertyBase
		{
			private readonly AnonymousTypePublicSymbol _container;

			private readonly MethodSymbol _getMethod;

			private readonly MethodSymbol _setMethod;

			internal readonly int PropertyIndex;

			internal AnonymousTypePublicSymbol AnonymousType => _container;

			public override MethodSymbol SetMethod => _setMethod;

			public override MethodSymbol GetMethod => _getMethod;

			public override TypeSymbol Type => _container.TypeDescriptor.Fields[PropertyIndex].Type;

			public override string Name => _container.TypeDescriptor.Fields[PropertyIndex].Name;

			public override Symbol ContainingSymbol => _container;

			public override NamedTypeSymbol ContainingType => _container;

			public override ImmutableArray<Location> Locations => ImmutableArray.Create(_container.TypeDescriptor.Fields[PropertyIndex].Location);

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<FieldInitializerSyntax>(Locations);

			public override bool IsImplicitlyDeclared => ContainingType.IsImplicitlyDeclared;

			public AnonymousTypePropertyPublicSymbol(AnonymousTypePublicSymbol container, int index)
			{
				_container = container;
				PropertyIndex = index;
				_getMethod = new AnonymousTypePropertyGetAccessorPublicSymbol(this);
				if (!container.TypeDescriptor.Fields[index].IsKey)
				{
					_setMethod = new AnonymousTypePropertySetAccessorPublicSymbol(this, container.Manager.System_Void);
				}
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj == this)
				{
					return true;
				}
				if (!(obj is AnonymousTypePropertyPublicSymbol anonymousTypePropertyPublicSymbol))
				{
					return false;
				}
				return (object)anonymousTypePropertyPublicSymbol != null && CaseInsensitiveComparison.Equals(anonymousTypePropertyPublicSymbol.Name, Name) && anonymousTypePropertyPublicSymbol.ContainingType.Equals(ContainingType);
			}

			public override int GetHashCode()
			{
				return Hash.Combine(ContainingType.GetHashCode(), CaseInsensitiveComparison.GetHashCode(Name));
			}
		}

		internal sealed class AnonymousTypePublicSymbol : AnonymousTypeOrDelegatePublicSymbol
		{
			private readonly ImmutableArray<AnonymousTypePropertyPublicSymbol> _properties;

			private readonly ImmutableArray<Symbol> _members;

			private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

			public override TypeKind TypeKind => TypeKind.Class;

			internal override bool IsInterface => false;

			public ImmutableArray<AnonymousTypePropertyPublicSymbol> Properties => _properties;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<AnonymousObjectCreationExpressionSyntax>(Locations);

			public AnonymousTypePublicSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
				: base(manager, typeDescr)
			{
				int length = typeDescr.Fields.Length;
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance();
				AnonymousTypePropertyPublicSymbol[] array = new AnonymousTypePropertyPublicSymbol[length - 1 + 1];
				bool flag = false;
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (typeDescr.Fields[i].IsKey)
					{
						flag = true;
					}
					AnonymousTypePropertyPublicSymbol anonymousTypePropertyPublicSymbol = (array[i] = new AnonymousTypePropertyPublicSymbol(this, i));
					instance2.Add(anonymousTypePropertyPublicSymbol);
					instance.Add(anonymousTypePropertyPublicSymbol.GetMethod);
					if ((object)anonymousTypePropertyPublicSymbol.SetMethod != null)
					{
						instance.Add(anonymousTypePropertyPublicSymbol.SetMethod);
					}
				}
				_properties = array.AsImmutableOrNull();
				instance.Add(CreateConstructorSymbol());
				instance.Add(CreateToStringMethod());
				if (flag && (object)Manager.System_IEquatable_T_Equals != null)
				{
					instance.Add(CreateGetHashCodeMethod());
					NamedTypeSymbol namedTypeSymbol = Manager.System_IEquatable_T.Construct(ImmutableArray.Create((TypeSymbol)this));
					_interfaces = ImmutableArray.Create(namedTypeSymbol);
					Symbol memberForDefinition = ((SubstitutedNamedType)namedTypeSymbol).GetMemberForDefinition(Manager.System_IEquatable_T_Equals);
					instance.Add(CreateIEquatableEqualsMethod((MethodSymbol)memberForDefinition));
					instance.Add(CreateEqualsMethod());
				}
				else
				{
					_interfaces = ImmutableArray<NamedTypeSymbol>.Empty;
				}
				instance.AddRange(instance2);
				instance2.Free();
				_members = instance.ToImmutableAndFree();
			}

			private MethodSymbol CreateConstructorSymbol()
			{
				SynthesizedSimpleConstructorSymbol synthesizedSimpleConstructorSymbol = new SynthesizedSimpleConstructorSymbol(this);
				int length = _properties.Length;
				ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					PropertySymbol propertySymbol = _properties[i];
					array[i] = new SynthesizedParameterSimpleSymbol(synthesizedSimpleConstructorSymbol, propertySymbol.Type, i, propertySymbol.Name);
				}
				synthesizedSimpleConstructorSymbol.SetParameters(array.AsImmutableOrNull());
				return synthesizedSimpleConstructorSymbol;
			}

			private MethodSymbol CreateEqualsMethod()
			{
				SynthesizedSimpleMethodSymbol synthesizedSimpleMethodSymbol = new SynthesizedSimpleMethodSymbol(this, "Equals", Manager.System_Boolean, Manager.System_Object__Equals, null, isOverloads: true);
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSimpleSymbol(synthesizedSimpleMethodSymbol, Manager.System_Object, 0, "obj")));
				return synthesizedSimpleMethodSymbol;
			}

			private MethodSymbol CreateIEquatableEqualsMethod(MethodSymbol iEquatableEquals)
			{
				SynthesizedSimpleMethodSymbol synthesizedSimpleMethodSymbol = new SynthesizedSimpleMethodSymbol(this, "Equals", Manager.System_Boolean, null, iEquatableEquals, isOverloads: true);
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSimpleSymbol(synthesizedSimpleMethodSymbol, this, 0, "val")));
				return synthesizedSimpleMethodSymbol;
			}

			private MethodSymbol CreateGetHashCodeMethod()
			{
				SynthesizedSimpleMethodSymbol synthesizedSimpleMethodSymbol = new SynthesizedSimpleMethodSymbol(this, "GetHashCode", Manager.System_Int32, Manager.System_Object__GetHashCode);
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray<ParameterSymbol>.Empty);
				return synthesizedSimpleMethodSymbol;
			}

			private MethodSymbol CreateToStringMethod()
			{
				SynthesizedSimpleMethodSymbol synthesizedSimpleMethodSymbol = new SynthesizedSimpleMethodSymbol(this, "ToString", Manager.System_String, Manager.System_Object__ToString);
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray<ParameterSymbol>.Empty);
				return synthesizedSimpleMethodSymbol;
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
			{
				AnonymousTypeDescriptor newDescriptor = default(AnonymousTypeDescriptor);
				return TypeDescriptor.SubstituteTypeParametersIfNeeded(substitution, out newDescriptor) ? new TypeWithModifiers(Manager.ConstructAnonymousTypeSymbol(newDescriptor)) : new TypeWithModifiers(this);
			}

			public override ImmutableArray<Symbol> GetMembers()
			{
				return _members;
			}

			internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
			{
				return Manager.System_Object;
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
			{
				return _interfaces;
			}

			public override NamedTypeSymbol MapToImplementationSymbol()
			{
				return Manager.ConstructAnonymousTypeImplementationSymbol(this);
			}
		}

		private class AnonymousDelegateTemplateSymbol : AnonymousTypeOrDelegateTemplateSymbol
		{
			private const int s_ctorIndex = 0;

			private const int s_beginInvokeIndex = 1;

			private const int s_endInvokeIndex = 2;

			private const int s_invokeIndex = 3;

			protected readonly AnonymousTypeDescriptor TypeDescr;

			private readonly ImmutableArray<SynthesizedDelegateMethodSymbol> _members;

			internal override string GeneratedNamePrefix => "VB$AnonymousDelegate_";

			public override MethodSymbol DelegateInvokeMethod => _members[_members.Length - 1];

			public override TypeKind TypeKind => TypeKind.Delegate;

			internal override bool IsInterface => false;

			internal static AnonymousDelegateTemplateSymbol Create(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
			{
				ImmutableArray<AnonymousTypeField> parameters = typeDescr.Parameters;
				if (parameters.Length != 1 || !AnonymousTypeExtensions.IsSubDescription(parameters))
				{
					return new AnonymousDelegateTemplateSymbol(manager, typeDescr);
				}
				return new NonGenericAnonymousDelegateSymbol(manager, typeDescr);
			}

			public AnonymousDelegateTemplateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
				: base(manager, typeDescr)
			{
				TypeDescr = typeDescr;
				ImmutableArray<AnonymousTypeField> parameters = typeDescr.Parameters;
				TypeSymbol returnType = (AnonymousTypeExtensions.IsSubDescription(parameters) ? ((TypeSymbol)manager.System_Void) : ((TypeSymbol)TypeParameters.Last()));
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(parameters.Length + 1);
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol = new SynthesizedDelegateMethodSymbol("Invoke", this, SourceMemberFlags.MethodKindDelegateInvoke | SourceMemberFlags.Overridable, returnType);
				int num = parameters.Length - 2;
				for (int i = 0; i <= num; i++)
				{
					instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol, TypeParameters[i], i, parameters[i].IsByRef, parameters[i].Name, i));
				}
				synthesizedDelegateMethodSymbol.SetParameters(instance.ToImmutable());
				instance.Clear();
				SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol2 = new SynthesizedDelegateMethodSymbol(".ctor", this, SourceMemberFlags.Static, manager.System_Void);
				synthesizedDelegateMethodSymbol2.SetParameters(ImmutableArray.Create((ParameterSymbol)new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol2, manager.System_Object, 0, isByRef: false, "TargetObject"), (ParameterSymbol)new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol2, manager.System_IntPtr, 1, isByRef: false, "TargetMethod")));
				if (SymbolExtensions.IsCompilationOutputWinMdObj(this))
				{
					SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol3 = null;
					SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol4 = null;
					_members = ImmutableArray.Create(synthesizedDelegateMethodSymbol2, synthesizedDelegateMethodSymbol);
				}
				else
				{
					SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol3 = new SynthesizedDelegateMethodSymbol("BeginInvoke", this, SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable, manager.System_IAsyncResult);
					int num2 = synthesizedDelegateMethodSymbol.ParameterCount - 1;
					int i;
					for (i = 0; i <= num2; i++)
					{
						ParameterSymbol parameterSymbol = synthesizedDelegateMethodSymbol.Parameters[i];
						instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol3, parameterSymbol.Type, i, parameterSymbol.IsByRef, parameterSymbol.Name, i));
					}
					instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_AsyncCallback, i, isByRef: false, "DelegateCallback"));
					i++;
					instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_Object, i, isByRef: false, "DelegateAsyncState"));
					synthesizedDelegateMethodSymbol3.SetParameters(instance.ToImmutable());
					instance.Clear();
					SynthesizedDelegateMethodSymbol synthesizedDelegateMethodSymbol4 = new SynthesizedDelegateMethodSymbol("EndInvoke", this, SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable, returnType);
					int num3 = 0;
					int num4 = synthesizedDelegateMethodSymbol.ParameterCount - 1;
					for (i = 0; i <= num4; i++)
					{
						ParameterSymbol parameterSymbol2 = synthesizedDelegateMethodSymbol.Parameters[i];
						if (parameterSymbol2.IsByRef)
						{
							instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol4, parameterSymbol2.Type, num3, parameterSymbol2.IsByRef, parameterSymbol2.Name, i));
							num3++;
						}
					}
					instance.Add(new AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol4, manager.System_IAsyncResult, num3, isByRef: false, "DelegateAsyncResult"));
					synthesizedDelegateMethodSymbol4.SetParameters(instance.ToImmutable());
					_members = ImmutableArray.Create(synthesizedDelegateMethodSymbol2, synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol4, synthesizedDelegateMethodSymbol);
				}
				instance.Free();
			}

			internal override AnonymousTypeKey GetAnonymousTypeKey()
			{
				ImmutableArray<AnonymousTypeKeyField> fields = TypeDescr.Parameters.SelectAsArray((AnonymousTypeField p) => new AnonymousTypeKeyField(p.Name, p.IsByRef, ignoreCase: true));
				return new AnonymousTypeKey(fields, isDelegate: true);
			}

			public override ImmutableArray<Symbol> GetMembers()
			{
				return StaticCast<Symbol>.From(_members);
			}

			internal sealed override IEnumerable<FieldSymbol> GetFieldsToEmit()
			{
				return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
			}

			internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
			{
				return Manager.System_MulticastDelegate;
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				Symbol.AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
				TypedConstant typedConstant = new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, "<generated method>");
				Symbol.AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor, ImmutableArray.Create(typedConstant), ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type, typedConstant))));
			}
		}

		private sealed class NonGenericAnonymousDelegateSymbol : AnonymousDelegateTemplateSymbol
		{
			public override ImmutableArray<Location> Locations => ImmutableArray.Create(TypeDescr.Location);

			public NonGenericAnonymousDelegateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
				: base(manager, typeDescr)
			{
			}

			public override int GetHashCode()
			{
				return Manager.GetHashCode();
			}

			public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
			{
				if ((object)obj == this)
				{
					return true;
				}
				return obj is NonGenericAnonymousDelegateSymbol nonGenericAnonymousDelegateSymbol && nonGenericAnonymousDelegateSymbol.Manager == Manager;
			}
		}

		internal sealed class NameAndIndex
		{
			public readonly string Name;

			public readonly int Index;

			public NameAndIndex(string name, int index)
			{
				Name = name;
				Index = index;
			}
		}

		internal abstract class AnonymousTypeOrDelegateTemplateSymbol : InstanceTypeSymbol
		{
			private sealed class LocationAndNames
			{
				public readonly Location Location;

				public readonly ImmutableArray<string> Names;

				public LocationAndNames(AnonymousTypeDescriptor typeDescr)
				{
					Location = typeDescr.Location;
					Names = typeDescr.Fields.SelectAsArray((AnonymousTypeField d) => d.Name);
				}
			}

			public readonly AnonymousTypeManager Manager;

			private NameAndIndex _nameAndIndex;

			private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

			private LocationAndNames _adjustedPropertyNames;

			internal readonly string TypeDescriptorKey;

			public override string Name => _nameAndIndex.Name;

			internal override bool MangleName => _typeParameters.Length > 0;

			internal sealed override bool HasSpecialName => false;

			public sealed override bool IsSerializable => false;

			internal override TypeLayout Layout => default(TypeLayout);

			internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

			public abstract override TypeKind TypeKind { get; }

			public override int Arity => _typeParameters.Length;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

			public override bool IsMustInherit => false;

			public override bool IsNotInheritable => true;

			public override bool MightContainExtensionMethods => false;

			internal override bool HasCodeAnalysisEmbeddedAttribute => false;

			internal override bool HasVisualBasicEmbeddedAttribute => false;

			internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

			internal override bool IsWindowsRuntimeImport => false;

			internal override bool ShouldAddWinRTMembers => false;

			internal override bool IsComImport => false;

			internal override TypeSymbol CoClassType => null;

			internal override bool HasDeclarativeSecurity => false;

			internal override string DefaultPropertyName => null;

			public override IEnumerable<string> MemberNames => new HashSet<string>(from member in GetMembers()
				select member.Name);

			public override Symbol ContainingSymbol => Manager.ContainingModule.GlobalNamespace;

			public override NamedTypeSymbol ContainingType => null;

			public override Accessibility DeclaredAccessibility => Accessibility.Internal;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public override bool IsImplicitlyDeclared => true;

			internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

			internal NameAndIndex NameAndIndex
			{
				get
				{
					return _nameAndIndex;
				}
				set
				{
					Interlocked.CompareExchange(ref _nameAndIndex, value, null);
				}
			}

			internal abstract string GeneratedNamePrefix { get; }

			public Location SmallestLocation => _adjustedPropertyNames.Location;

			protected AnonymousTypeOrDelegateTemplateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
			{
				Manager = manager;
				TypeDescriptorKey = typeDescr.Key;
				_adjustedPropertyNames = new LocationAndNames(typeDescr);
				int num = typeDescr.Fields.Length;
				if (TypeKind == TypeKind.Delegate && AnonymousTypeExtensions.IsSubDescription(typeDescr.Fields))
				{
					num--;
				}
				if (num == 0)
				{
					_typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
					return;
				}
				TypeParameterSymbol[] array = new TypeParameterSymbol[num - 1 + 1];
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					array[i] = new AnonymousTypeOrDelegateTypeParameterSymbol(this, i);
				}
				_typeParameters = array.AsImmutable();
			}

			internal abstract AnonymousTypeKey GetAnonymousTypeKey();

			internal override ImmutableArray<string> GetAppliedConditionalSymbols()
			{
				return ImmutableArray<string>.Empty;
			}

			internal override AttributeUsageInfo GetAttributeUsageInfo()
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
			{
				return MakeAcyclicBaseType(diagnostics);
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
			{
				return MakeAcyclicInterfaces(diagnostics);
			}

			public override ImmutableArray<Symbol> GetMembers(string name)
			{
				return ImmutableArray.CreateRange(from member in GetMembers()
					where CaseInsensitiveComparison.Equals(member.Name, name)
					select member);
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}

			internal void AdjustMetadataNames(AnonymousTypeDescriptor typeDescr)
			{
				Location location = typeDescr.Location;
				LocationAndNames adjustedPropertyNames;
				LocationAndNames value;
				do
				{
					adjustedPropertyNames = _adjustedPropertyNames;
					if (adjustedPropertyNames == null || Manager.Compilation.CompareSourceLocations(adjustedPropertyNames.Location, location) > 0)
					{
						value = new LocationAndNames(typeDescr);
						continue;
					}
					break;
				}
				while (Interlocked.CompareExchange(ref _adjustedPropertyNames, value, adjustedPropertyNames) != adjustedPropertyNames);
			}

			internal string GetAdjustedName(int index)
			{
				return _adjustedPropertyNames.Names[index];
			}

			internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
			{
				throw ExceptionUtilities.Unreachable;
			}

			internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
			{
				return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
			}
		}

		private sealed class AnonymousTypeOrDelegateTypeParameterSymbol : TypeParameterSymbol
		{
			private readonly AnonymousTypeOrDelegateTemplateSymbol _container;

			private readonly int _ordinal;

			public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

			public override Symbol ContainingSymbol => _container;

			internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public override string Name
			{
				get
				{
					if (_container.TypeKind == TypeKind.Delegate)
					{
						if (_container.DelegateInvokeMethod.IsSub || Ordinal < _container.Arity - 1)
						{
							return "TArg" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(Ordinal);
						}
						return "TResult";
					}
					return "T" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(Ordinal);
				}
			}

			public override int Ordinal => _ordinal;

			public override bool HasConstructorConstraint => false;

			public override bool HasReferenceTypeConstraint => false;

			public override bool HasValueTypeConstraint => false;

			public override VarianceKind Variance => VarianceKind.None;

			public override bool IsImplicitlyDeclared => true;

			public AnonymousTypeOrDelegateTypeParameterSymbol(AnonymousTypeOrDelegateTemplateSymbol container, int ordinal)
			{
				_container = container;
				_ordinal = ordinal;
			}

			internal override void EnsureAllConstraintsAreResolved()
			{
			}

			public override int GetHashCode()
			{
				return RuntimeHelpers.GetHashCode(this);
			}

			public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
			{
				return (object)other == this;
			}
		}

		private sealed class AnonymousTypeOrDelegateParameterSymbol : SynthesizedParameterSymbol
		{
			public readonly int CorrespondingInvokeParameterOrProperty;

			public override string MetadataName
			{
				get
				{
					if (CorrespondingInvokeParameterOrProperty != -1)
					{
						return ((AnonymousTypeOrDelegateTemplateSymbol)_container.ContainingSymbol).GetAdjustedName(CorrespondingInvokeParameterOrProperty);
					}
					return base.MetadataName;
				}
			}

			public AnonymousTypeOrDelegateParameterSymbol(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name, int correspondingInvokeParameterOrProperty = -1)
				: base(container, type, ordinal, isByRef, name)
			{
				CorrespondingInvokeParameterOrProperty = correspondingInvokeParameterOrProperty;
			}
		}

		private abstract class AnonymousTypePropertyAccessorSymbol : SynthesizedPropertyAccessorBase<PropertySymbol>
		{
			private readonly TypeSymbol _returnType;

			internal sealed override FieldSymbol BackingFieldSymbol => ((AnonymousTypePropertySymbol)m_propertyOrEvent).AssociatedField;

			public override TypeSymbol ReturnType => _returnType;

			internal sealed override bool GenerateDebugInfoImpl => false;

			public AnonymousTypePropertyAccessorSymbol(PropertySymbol property, TypeSymbol returnType)
				: base(property.ContainingType, property)
			{
				_returnType = returnType;
			}

			protected override string GenerateMetadataName()
			{
				return Binder.GetAccessorName(m_propertyOrEvent.MetadataName, MethodKind, SymbolExtensions.IsCompilationOutputWinMdObj(this));
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
			}

			internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class AnonymousTypePropertyGetAccessorSymbol : AnonymousTypePropertyAccessorSymbol
		{
			public override bool IsSub => false;

			public override MethodKind MethodKind => MethodKind.PropertyGet;

			public AnonymousTypePropertyGetAccessorSymbol(PropertySymbol property)
				: base(property, property.Type)
			{
			}
		}

		private sealed class AnonymousTypePropertySetAccessorSymbol : AnonymousTypePropertyAccessorSymbol
		{
			private ImmutableArray<ParameterSymbol> _parameters;

			public override bool IsSub => true;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override MethodKind MethodKind => MethodKind.PropertySet;

			public AnonymousTypePropertySetAccessorSymbol(PropertySymbol property, TypeSymbol voidTypeSymbol)
				: base(property, voidTypeSymbol)
			{
				_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, m_propertyOrEvent.Type, 0, isByRef: false, "Value"));
			}
		}

		private sealed class AnonymousTypePropertyBackingFieldSymbol : SynthesizedBackingFieldBase<PropertySymbol>
		{
			public override bool IsReadOnly => _propertyOrEvent.IsReadOnly;

			public override string MetadataName => "$" + _propertyOrEvent.MetadataName;

			public override TypeSymbol Type => _propertyOrEvent.Type;

			public AnonymousTypePropertyBackingFieldSymbol(PropertySymbol property)
				: base(property, "$" + property.Name, isShared: false)
			{
			}
		}

		private sealed class AnonymousTypePropertySymbol : PropertySymbol
		{
			private readonly AnonymousTypeTemplateSymbol _containingType;

			private readonly TypeSymbol _type;

			private readonly string _name;

			private readonly MethodSymbol _getMethod;

			private readonly MethodSymbol _setMethod;

			private readonly FieldSymbol _backingField;

			internal readonly int PropertyIndex;

			internal AnonymousTypeTemplateSymbol AnonymousType => _containingType;

			internal override FieldSymbol AssociatedField => _backingField;

			public override bool IsDefault => false;

			public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

			public override ImmutableArray<CustomModifier> TypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

			public override MethodSymbol SetMethod => _setMethod;

			public override MethodSymbol GetMethod => _getMethod;

			public override bool ReturnsByRef => false;

			public override TypeSymbol Type => _type;

			public override string Name => _name;

			public override string MetadataName => AnonymousType.GetAdjustedName(PropertyIndex);

			internal override bool HasSpecialName => false;

			public override Accessibility DeclaredAccessibility => Accessibility.Public;

			internal override Microsoft.Cci.CallingConvention CallingConvention => Microsoft.Cci.CallingConvention.HasThis;

			public override Symbol ContainingSymbol => _containingType;

			public override NamedTypeSymbol ContainingType => _containingType;

			public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

			public override bool IsMustOverride => false;

			public override bool IsNotOverridable => false;

			public override bool IsOverloads => false;

			internal override bool ShadowsExplicitly => false;

			public override bool IsOverridable => false;

			public override bool IsOverrides => false;

			public override bool IsShared => false;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public override bool IsImplicitlyDeclared => true;

			internal override ObsoleteAttributeData ObsoleteAttributeData => null;

			internal override bool IsMyGroupCollectionProperty => false;

			public AnonymousTypePropertySymbol(AnonymousTypeTemplateSymbol container, AnonymousTypeField field, int index, TypeSymbol typeSymbol)
			{
				_containingType = container;
				_type = typeSymbol;
				_name = field.Name;
				PropertyIndex = index;
				_getMethod = new AnonymousTypePropertyGetAccessorSymbol(this);
				if (!field.IsKey)
				{
					_setMethod = new AnonymousTypePropertySetAccessorSymbol(this, container.Manager.System_Void);
				}
				_backingField = new AnonymousTypePropertyBackingFieldSymbol(this);
			}
		}

		private sealed class AnonymousTypeTemplateSymbol : AnonymousTypeOrDelegateTemplateSymbol
		{
			private readonly ImmutableArray<AnonymousTypePropertySymbol> _properties;

			private readonly ImmutableArray<Symbol> _members;

			private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

			internal readonly bool HasAtLeastOneKeyField;

			internal override string GeneratedNamePrefix => "VB$AnonymousType_";

			public ImmutableArray<AnonymousTypePropertySymbol> Properties => _properties;

			public override TypeKind TypeKind => TypeKind.Class;

			internal override bool IsInterface => false;

			public AnonymousTypeTemplateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
				: base(manager, typeDescr)
			{
				int length = typeDescr.Fields.Length;
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance();
				AnonymousTypePropertySymbol[] array = new AnonymousTypePropertySymbol[length - 1 + 1];
				HasAtLeastOneKeyField = false;
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					AnonymousTypeField field = typeDescr.Fields[i];
					if (field.IsKey)
					{
						HasAtLeastOneKeyField = true;
					}
					AnonymousTypePropertySymbol anonymousTypePropertySymbol = (array[i] = new AnonymousTypePropertySymbol(this, field, i, TypeParameters[i]));
					instance2.Add(anonymousTypePropertySymbol);
					instance.Add(anonymousTypePropertySymbol.GetMethod);
					if ((object)anonymousTypePropertySymbol.SetMethod != null)
					{
						instance.Add(anonymousTypePropertySymbol.SetMethod);
					}
					instance2.Add(anonymousTypePropertySymbol.AssociatedField);
				}
				_properties = array.AsImmutableOrNull();
				instance.Add(new AnonymousTypeConstructorSymbol(this));
				instance.Add(new AnonymousTypeToStringMethodSymbol(this));
				if (HasAtLeastOneKeyField && (object)Manager.System_IEquatable_T_Equals != null)
				{
					instance.Add(new AnonymousTypeGetHashCodeMethodSymbol(this));
					NamedTypeSymbol namedTypeSymbol = Manager.System_IEquatable_T.Construct(ImmutableArray.Create((TypeSymbol)this));
					_interfaces = ImmutableArray.Create(namedTypeSymbol);
					Symbol memberForDefinition = ((SubstitutedNamedType)namedTypeSymbol).GetMemberForDefinition(Manager.System_IEquatable_T_Equals);
					MethodSymbol methodSymbol = new AnonymousType_IEquatable_EqualsMethodSymbol(this, (MethodSymbol)memberForDefinition);
					instance.Add(methodSymbol);
					instance.Add(new AnonymousTypeEqualsMethodSymbol(this, methodSymbol));
				}
				else
				{
					_interfaces = ImmutableArray<NamedTypeSymbol>.Empty;
				}
				instance.AddRange(instance2);
				instance2.Free();
				_members = instance.ToImmutableAndFree();
			}

			internal override AnonymousTypeKey GetAnonymousTypeKey()
			{
				ImmutableArray<AnonymousTypeKeyField> fields = _properties.SelectAsArray((AnonymousTypePropertySymbol p) => new AnonymousTypeKeyField(p.Name, p.IsReadOnly, ignoreCase: true));
				return new AnonymousTypeKey(fields);
			}

			public override ImmutableArray<Symbol> GetMembers()
			{
				return _members;
			}

			[IteratorStateMachine(typeof(VB_0024StateMachine_11_GetFieldsToEmit))]
			internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
			{
				//yield-return decompiler failed: Method not found
				return new VB_0024StateMachine_11_GetFieldsToEmit(-2)
				{
					_0024VB_0024Me = this
				};
			}

			internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
			{
				return Manager.System_Object;
			}

			internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
			{
				return _interfaces;
			}

			internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
			{
				base.AddSynthesizedAttributes(compilationState, ref attributes);
				Symbol.AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
				Symbol.AddSynthesizedAttribute(ref attributes, SynthesizeDebuggerDisplayAttribute());
			}

			private SynthesizedAttributeData SynthesizeDebuggerDisplayAttribute()
			{
				PooledStringBuilder instance = PooledStringBuilder.GetInstance();
				StringBuilder builder = instance.Builder;
				int num = Math.Min(Properties.Length, 4);
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					string name = Properties[i].Name;
					if (i > 0)
					{
						builder.Append(", ");
					}
					builder.Append(name);
					builder.Append("={");
					builder.Append(name);
					builder.Append("}");
				}
				if (Properties.Length > num)
				{
					builder.Append(", ...");
				}
				return Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor, ImmutableArray.Create(new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, instance.ToStringAndFree())));
			}
		}

		public readonly VisualBasicCompilation Compilation;

		private ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> _concurrentTypesCache;

		private ConcurrentDictionary<string, AnonymousDelegateTemplateSymbol> _concurrentDelegatesCache;

		public SourceModuleSymbol ContainingModule => (SourceModuleSymbol)Compilation.SourceModule;

		private ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> AnonymousTypeTemplates
		{
			get
			{
				if (_concurrentTypesCache == null)
				{
					ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> concurrentDictionary = Compilation.PreviousSubmission?.AnonymousTypeManager._concurrentTypesCache;
					Interlocked.CompareExchange(ref _concurrentTypesCache, (concurrentDictionary == null) ? new ConcurrentDictionary<string, AnonymousTypeTemplateSymbol>() : new ConcurrentDictionary<string, AnonymousTypeTemplateSymbol>(concurrentDictionary), null);
				}
				return _concurrentTypesCache;
			}
		}

		private ConcurrentDictionary<string, AnonymousDelegateTemplateSymbol> AnonymousDelegateTemplates
		{
			get
			{
				if (_concurrentDelegatesCache == null)
				{
					ConcurrentDictionary<string, AnonymousDelegateTemplateSymbol> concurrentDictionary = Compilation.PreviousSubmission?.AnonymousTypeManager._concurrentDelegatesCache;
					Interlocked.CompareExchange(ref _concurrentDelegatesCache, (concurrentDictionary == null) ? new ConcurrentDictionary<string, AnonymousDelegateTemplateSymbol>() : new ConcurrentDictionary<string, AnonymousDelegateTemplateSymbol>(concurrentDictionary), null);
				}
				return _concurrentDelegatesCache;
			}
		}

		internal ImmutableArray<NamedTypeSymbol> AllCreatedTemplates
		{
			get
			{
				ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> instance = ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.GetInstance();
				GetAllCreatedTemplates(instance);
				return StaticCast<NamedTypeSymbol>.From(instance.ToImmutableAndFree());
			}
		}

		public NamedTypeSymbol System_Boolean => Compilation.GetSpecialType(SpecialType.System_Boolean);

		public NamedTypeSymbol System_Int32 => Compilation.GetSpecialType(SpecialType.System_Int32);

		public NamedTypeSymbol System_Object => Compilation.GetSpecialType(SpecialType.System_Object);

		public NamedTypeSymbol System_IntPtr => Compilation.GetSpecialType(SpecialType.System_IntPtr);

		public NamedTypeSymbol System_IAsyncResult => Compilation.GetSpecialType(SpecialType.System_IAsyncResult);

		public NamedTypeSymbol System_AsyncCallback => Compilation.GetSpecialType(SpecialType.System_AsyncCallback);

		public NamedTypeSymbol System_MulticastDelegate => Compilation.GetSpecialType(SpecialType.System_MulticastDelegate);

		public NamedTypeSymbol System_String => Compilation.GetSpecialType(SpecialType.System_String);

		public NamedTypeSymbol System_Void => Compilation.GetSpecialType(SpecialType.System_Void);

		public MethodSymbol System_String__Format_IFormatProvider => (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_String__Format_IFormatProvider);

		public MethodSymbol System_Object__ToString => (MethodSymbol)ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__ToString);

		public MethodSymbol System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor => (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor);

		public MethodSymbol System_Diagnostics_DebuggerDisplayAttribute__ctor => (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor);

		public PropertySymbol System_Diagnostics_DebuggerDisplayAttribute__Type => (PropertySymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type);

		public MethodSymbol System_Object__GetHashCode => (MethodSymbol)ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__GetHashCode);

		public MethodSymbol System_Object__Equals => (MethodSymbol)ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__Equals);

		public NamedTypeSymbol System_IEquatable_T => Compilation.GetWellKnownType(WellKnownType.System_IEquatable_T);

		public MethodSymbol System_IEquatable_T_Equals => (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_IEquatable_T__Equals);

		public AnonymousTypeManager(VisualBasicCompilation compilation)
		{
			_concurrentTypesCache = null;
			_concurrentDelegatesCache = null;
			Compilation = compilation;
		}

		public AnonymousTypePublicSymbol ConstructAnonymousTypeSymbol(AnonymousTypeDescriptor typeDescr)
		{
			return new AnonymousTypePublicSymbol(this, typeDescr);
		}

		public AnonymousDelegatePublicSymbol ConstructAnonymousDelegateSymbol(AnonymousTypeDescriptor delegateDescriptor)
		{
			return new AnonymousDelegatePublicSymbol(this, delegateDescriptor);
		}

		[Conditional("DEBUG")]
		private void CheckSourceLocationSeen(AnonymousTypeOrDelegatePublicSymbol anonymous)
		{
		}

		private NamedTypeSymbol ConstructAnonymousTypeImplementationSymbol(AnonymousTypePublicSymbol anonymous)
		{
			AnonymousTypeDescriptor typeDescriptor = anonymous.TypeDescriptor;
			AnonymousTypeTemplateSymbol value = null;
			string key = typeDescriptor.Key;
			if (!AnonymousTypeTemplates.TryGetValue(key, out value))
			{
				value = AnonymousTypeTemplates.GetOrAdd(key, new AnonymousTypeTemplateSymbol(this, typeDescriptor));
			}
			if (value.Manager == this)
			{
				value.AdjustMetadataNames(typeDescriptor);
			}
			ImmutableArray<TypeSymbol> typeArguments = typeDescriptor.Fields.SelectAsArray((AnonymousTypeField f) => f.Type);
			return value.Construct(typeArguments);
		}

		private NamedTypeSymbol ConstructAnonymousDelegateImplementationSymbol(AnonymousDelegatePublicSymbol anonymous)
		{
			AnonymousTypeDescriptor typeDescriptor = anonymous.TypeDescriptor;
			ImmutableArray<AnonymousTypeField> parameters = typeDescriptor.Parameters;
			AnonymousDelegateTemplateSymbol value = null;
			string key = typeDescriptor.Key;
			if (!AnonymousDelegateTemplates.TryGetValue(key, out value))
			{
				value = AnonymousDelegateTemplates.GetOrAdd(key, AnonymousDelegateTemplateSymbol.Create(this, typeDescriptor));
			}
			if (value.Manager == this)
			{
				value.AdjustMetadataNames(typeDescriptor);
			}
			if (value.Arity == 0)
			{
				return value;
			}
			TypeSymbol[] array = new TypeSymbol[value.Arity - 1 + 1];
			int num = value.Arity - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = parameters[i].Type;
			}
			return value.Construct(array);
		}

		private void AddFromCache<T>(ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> builder, ConcurrentDictionary<string, T> cache) where T : AnonymousTypeOrDelegateTemplateSymbol
		{
			if (cache == null)
			{
				return;
			}
			foreach (T value in cache.Values)
			{
				if (value.Manager == this)
				{
					builder.Add(value);
				}
			}
		}

		private static AnonymousTypeDescriptor CreatePlaceholderTypeDescriptor(AnonymousTypeKey key)
		{
			ImmutableArray<AnonymousTypeField> fields = key.Fields.SelectAsArray((AnonymousTypeKeyField f) => new AnonymousTypeField(f.Name, Location.None, f.IsKey));
			return new AnonymousTypeDescriptor(fields, Location.None, isImplicitlyDeclared: true);
		}

		public void AssignTemplatesNamesAndCompile(MethodCompiler compiler, PEModuleBuilder moduleBeingBuilt, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<AnonymousTypeKey>.Enumerator enumerator = moduleBeingBuilt.GetPreviousAnonymousTypes().GetEnumerator();
			_Closure_0024__22_002D0 closure_0024__22_002D = default(_Closure_0024__22_002D0);
			while (enumerator.MoveNext())
			{
				closure_0024__22_002D = new _Closure_0024__22_002D0(closure_0024__22_002D);
				closure_0024__22_002D._0024VB_0024Me = this;
				closure_0024__22_002D._0024VB_0024Local_key = enumerator.Current;
				string key = AnonymousTypeDescriptor.ComputeKey(closure_0024__22_002D._0024VB_0024Local_key.Fields, (AnonymousTypeKeyField f) => f.Name, (AnonymousTypeKeyField f) => f.IsKey);
				if (closure_0024__22_002D._0024VB_0024Local_key.IsDelegate)
				{
					AnonymousDelegateTemplates.GetOrAdd(key, closure_0024__22_002D._Lambda_0024__2);
				}
				else
				{
					AnonymousTypeTemplates.GetOrAdd(key, closure_0024__22_002D._Lambda_0024__3);
				}
			}
			ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> instance = ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.GetInstance();
			GetAllCreatedTemplates(instance);
			if (!base.AreTemplatesSealed)
			{
				string text;
				if (moduleBeingBuilt.OutputKind == OutputKind.NetModule)
				{
					text = moduleBeingBuilt.Name;
					string defaultExtension = OutputKind.NetModule.GetDefaultExtension();
					if (text.EndsWith(defaultExtension, StringComparison.OrdinalIgnoreCase))
					{
						text = text.Substring(0, text.Length - defaultExtension.Length);
					}
					text = "<" + MetadataHelpers.MangleForTypeNameIfNeeded(text) + ">";
				}
				else
				{
					text = string.Empty;
				}
				int num = moduleBeingBuilt.GetNextAnonymousTypeIndex(fromDelegates: false);
				int num2 = moduleBeingBuilt.GetNextAnonymousTypeIndex(fromDelegates: true);
				ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					AnonymousTypeOrDelegateTemplateSymbol current = enumerator2.Current;
					string name = null;
					int index = 0;
					if (!moduleBeingBuilt.TryGetAnonymousTypeName(current, out name, out index))
					{
						switch (current.TypeKind)
						{
						case TypeKind.Delegate:
							index = num2;
							num2++;
							break;
						case TypeKind.Class:
							index = num;
							num++;
							break;
						default:
							throw ExceptionUtilities.UnexpectedValue(current.TypeKind);
						}
						int submissionSlotIndex = Compilation.GetSubmissionSlotIndex();
						name = GeneratedNames.MakeAnonymousTypeTemplateName(current.GeneratedNamePrefix, index, submissionSlotIndex, text);
					}
					current.NameAndIndex = new NameAndIndex(name, index);
				}
				SealTemplates();
			}
			if (instance.Count > 0 && !CheckAndReportMissingSymbols(instance, diagnostics))
			{
				ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.Enumerator enumerator3 = instance.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					enumerator3.Current.Accept(compiler);
				}
			}
			instance.Free();
		}

		internal IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> GetAnonymousTypeMap()
		{
			Dictionary<AnonymousTypeKey, AnonymousTypeValue> dictionary = new Dictionary<AnonymousTypeKey, AnonymousTypeValue>();
			ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> instance = ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.GetInstance();
			GetAllCreatedTemplates(instance);
			ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AnonymousTypeOrDelegateTemplateSymbol current = enumerator.Current;
				NameAndIndex nameAndIndex = current.NameAndIndex;
				AnonymousTypeKey anonymousTypeKey = current.GetAnonymousTypeKey();
				AnonymousTypeValue value = new AnonymousTypeValue(nameAndIndex.Name, nameAndIndex.Index, current.GetCciAdapter());
				dictionary.Add(anonymousTypeKey, value);
			}
			instance.Free();
			return dictionary;
		}

		internal static NamedTypeSymbol TranslateAnonymousTypeSymbol(NamedTypeSymbol type)
		{
			return ((AnonymousTypeOrDelegatePublicSymbol)type).MapToImplementationSymbol();
		}

		internal static MethodSymbol TranslateAnonymousTypeMethodSymbol(MethodSymbol method)
		{
			return ((AnonymousTypeOrDelegatePublicSymbol)method.ContainingType).MapMethodToImplementationSymbol(method);
		}

		private void GetAllCreatedTemplates(ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> builder)
		{
			AddFromCache(builder, _concurrentTypesCache);
			AddFromCache(builder, _concurrentDelegatesCache);
			if (builder.Any())
			{
				builder.Sort(new AnonymousTypeComparer(Compilation));
			}
		}

		public bool ReportMissingOrErroneousSymbols(BindingDiagnosticBag diagnostics, bool hasClass, bool hasDelegate, bool hasKeys)
		{
			bool hasError = false;
			ReportErrorOnSymbol(System_Object, diagnostics, ref hasError);
			ReportErrorOnSymbol(System_Void, diagnostics, ref hasError);
			bool embedVbCoreRuntime = Compilation.Options.EmbedVbCoreRuntime;
			if (hasDelegate)
			{
				ReportErrorOnSymbol(System_IntPtr, diagnostics, ref hasError);
				ReportErrorOnSymbol(System_IAsyncResult, diagnostics, ref hasError);
				ReportErrorOnSymbol(System_AsyncCallback, diagnostics, ref hasError);
				ReportErrorOnSymbol(System_MulticastDelegate, diagnostics, ref hasError);
			}
			if (hasClass)
			{
				ReportErrorOnSymbol(System_Int32, diagnostics, ref hasError);
				ReportErrorOnSymbol(System_String, diagnostics, ref hasError);
				ReportErrorOnSpecialMember(System_Object__ToString, SpecialMember.System_Object__ToString, diagnostics, ref hasError, embedVbCoreRuntime);
				ReportErrorOnWellKnownMember(System_String__Format_IFormatProvider, WellKnownMember.System_String__Format_IFormatProvider, diagnostics, ref hasError, embedVbCoreRuntime);
				if (hasKeys)
				{
					ReportErrorOnSymbol(System_Boolean, diagnostics, ref hasError);
					ReportErrorOnSpecialMember(System_Object__GetHashCode, SpecialMember.System_Object__GetHashCode, diagnostics, ref hasError, embedVbCoreRuntime);
					ReportErrorOnSpecialMember(System_Object__Equals, SpecialMember.System_Object__Equals, diagnostics, ref hasError, embedVbCoreRuntime);
					ReportErrorOnSymbol(System_IEquatable_T, diagnostics, ref hasError);
					ReportErrorOnSymbol(System_IEquatable_T_Equals, diagnostics, ref hasError);
				}
			}
			return hasError;
		}

		private static void ReportErrorOnSymbol(Symbol symbol, BindingDiagnosticBag diagnostics, ref bool hasError)
		{
			if ((object)symbol != null)
			{
				UseSiteInfo<AssemblySymbol> useSiteInfo = symbol.GetUseSiteInfo();
				if (diagnostics.Add(useSiteInfo, NoLocation.Singleton))
				{
					hasError = true;
				}
			}
		}

		private static void ReportErrorOnWellKnownMember(Symbol symbol, WellKnownMember member, BindingDiagnosticBag diagnostics, ref bool hasError, bool embedVBCore)
		{
			if ((object)symbol == null)
			{
				MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(member);
				DiagnosticInfo diagnosticForMissingRuntimeHelper = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, embedVBCore);
				diagnostics.Add(diagnosticForMissingRuntimeHelper, NoLocation.Singleton);
				hasError = true;
			}
			else
			{
				ReportErrorOnSymbol(symbol, diagnostics, ref hasError);
				ReportErrorOnSymbol(symbol.ContainingType, diagnostics, ref hasError);
			}
		}

		private static void ReportErrorOnSpecialMember(Symbol symbol, SpecialMember member, BindingDiagnosticBag diagnostics, ref bool hasError, bool embedVBCore)
		{
			if ((object)symbol == null)
			{
				MemberDescriptor descriptor = SpecialMembers.GetDescriptor(member);
				DiagnosticInfo diagnosticForMissingRuntimeHelper = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, embedVBCore);
				diagnostics.Add(diagnosticForMissingRuntimeHelper, NoLocation.Singleton);
				hasError = true;
			}
			else
			{
				ReportErrorOnSymbol(symbol, diagnostics, ref hasError);
			}
		}

		private bool CheckAndReportMissingSymbols(ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol> anonymousTypes, BindingDiagnosticBag diagnostics)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			ArrayBuilder<AnonymousTypeOrDelegateTemplateSymbol>.Enumerator enumerator = anonymousTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AnonymousTypeOrDelegateTemplateSymbol current = enumerator.Current;
				switch (current.TypeKind)
				{
				case TypeKind.Class:
					flag = true;
					if (!((AnonymousTypeTemplateSymbol)current).HasAtLeastOneKeyField)
					{
						continue;
					}
					flag3 = true;
					if (!flag2)
					{
						continue;
					}
					break;
				case TypeKind.Delegate:
					flag2 = true;
					if (!flag3)
					{
						continue;
					}
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(current.TypeKind);
				}
				break;
			}
			if (!flag && !flag2)
			{
				return true;
			}
			return ReportMissingOrErroneousSymbols(diagnostics, flag, flag2, flag3);
		}
	}
}
