using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class SourceMemberFieldSymbolFromDeclarator : SourceMemberFieldSymbol
    {
        private readonly bool _hasInitializer;

        private TypeWithAnnotations.Boxed _lazyType;

        private int _lazyFieldTypeInferred;

        protected sealed override TypeSyntax TypeSyntax => GetFieldDeclaration(VariableDeclaratorNode).Declaration.Type;

        protected sealed override SyntaxTokenList ModifiersTokenList => GetFieldDeclaration(VariableDeclaratorNode).Modifiers;

        public sealed override bool HasInitializer => _hasInitializer;

        protected VariableDeclaratorSyntax VariableDeclaratorNode => (VariableDeclaratorSyntax)base.SyntaxNode;

        protected override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList
        {
            get
            {
                if (containingType.AnyMemberHasAttributes)
                {
                    return GetFieldDeclaration(base.SyntaxNode).AttributeLists;
                }
                return default(SyntaxList<AttributeListSyntax>);
            }
        }

        internal override bool HasPointerType
        {
            get
            {
                if (_lazyType != null)
                {
                    return _lazyType.Value.DefaultType.Kind switch
                    {
                        SymbolKind.PointerType => true,
                        SymbolKind.FunctionPointerType => true,
                        _ => false,
                    };
                }
                return IsPointerFieldSyntactically();
            }
        }

        internal SourceMemberFieldSymbolFromDeclarator(SourceMemberContainerTypeSymbol containingType, VariableDeclaratorSyntax declarator, DeclarationModifiers modifiers, bool modifierErrors, BindingDiagnosticBag diagnostics)
            : base(containingType, modifiers, declarator.Identifier.ValueText, declarator.GetReference(), declarator.Identifier.GetLocation())
        {
            _hasInitializer = declarator.Initializer != null;
            CheckAccessibility(diagnostics);
            if (!modifierErrors)
            {
                ReportModifiersDiagnostics(diagnostics);
            }
            if (!containingType.IsInterface)
            {
                return;
            }
            if (IsStatic)
            {
                Binder.CheckFeatureAvailability(declarator, MessageID.IDS_DefaultInterfaceImplementation, diagnostics, ErrorLocation);
                if (!ContainingAssembly.RuntimeSupportsDefaultInterfaceImplementation)
                {
                    diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation, ErrorLocation);
                }
            }
            else
            {
                diagnostics.Add(ErrorCode.ERR_InterfacesCantContainFields, ErrorLocation);
            }
        }

        private static BaseFieldDeclarationSyntax GetFieldDeclaration(CSharpSyntaxNode declarator)
        {
            return (BaseFieldDeclarationSyntax)declarator.Parent!.Parent;
        }

        private bool IsPointerFieldSyntactically()
        {
            if (GetFieldDeclaration(VariableDeclaratorNode).Declaration.Type.Kind() switch
            {
                SyntaxKind.PointerType => true,
                SyntaxKind.FunctionPointerType => true,
                _ => false,
            })
            {
                return true;
            }
            return IsFixedSizeBuffer;
        }

        internal sealed override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if (_lazyType != null)
            {
                return _lazyType.Value;
            }
            VariableDeclaratorSyntax variableDeclaratorNode = VariableDeclaratorNode;
            BaseFieldDeclarationSyntax fieldDeclaration = GetFieldDeclaration(variableDeclaratorNode);
            TypeSyntax type = fieldDeclaration.Declaration.Type;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
            Symbol associatedSymbol = AssociatedSymbol;
            TypeWithAnnotations pointedAtType;
            if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Event)
            {
                EventSymbol eventSymbol = (EventSymbol)associatedSymbol;
                if (eventSymbol.IsWindowsRuntimeEvent)
                {
                    NamedTypeSymbol wellKnownType = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T);
                    Binder.ReportUseSite(wellKnownType, instance2, ErrorLocation);
                    pointedAtType = TypeWithAnnotations.Create(wellKnownType.Construct(ImmutableArray.Create(eventSymbol.TypeWithAnnotations)));
                }
                else
                {
                    pointedAtType = eventSymbol.TypeWithAnnotations;
                }
            }
            else
            {
                Binder binder = declaringCompilation.GetBinderFactory(base.SyntaxTree).GetBinder(type);
                binder = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
                if (!ContainingType.IsScriptClass)
                {
                    pointedAtType = binder.BindType(type, instance2);
                }
                else
                {
                    pointedAtType = binder.BindTypeOrVarKeyword(type, instance, out var isVar);
                    if (isVar)
                    {
                        if (IsConst)
                        {
                            instance2.Add(ErrorCode.ERR_ImplicitlyTypedVariableCannotBeConst, type.Location);
                        }
                        if (fieldsBeingBound.ContainsReference(this))
                        {
                            instance.Add(ErrorCode.ERR_RecursivelyTypedVariable, ErrorLocation, this);
                            pointedAtType = default(TypeWithAnnotations);
                        }
                        else if (fieldDeclaration.Declaration.Variables.Count > 1)
                        {
                            instance2.Add(ErrorCode.ERR_ImplicitlyTypedVariableMultipleDeclarator, type.Location);
                        }
                        else if (IsConst && ContainingType.IsScriptClass)
                        {
                            pointedAtType = default(TypeWithAnnotations);
                        }
                        else
                        {
                            fieldsBeingBound = new ConsList<FieldSymbol>(this, fieldsBeingBound);
                            BoundExpression boundExpression = new ImplicitlyTypedFieldBinder(binder, fieldsBeingBound).BindInferredVariableInitializer(instance, RefKind.None, variableDeclaratorNode.Initializer, variableDeclaratorNode);
                            if (boundExpression != null)
                            {
                                if ((object)boundExpression.Type != null && !boundExpression.Type.IsErrorType())
                                {
                                    pointedAtType = TypeWithAnnotations.Create(boundExpression.Type);
                                }
                                _lazyFieldTypeInferred = 1;
                            }
                        }
                        if (!pointedAtType.HasType)
                        {
                            pointedAtType = TypeWithAnnotations.Create(binder.CreateErrorType("var"));
                        }
                    }
                }
                if (IsFixedSizeBuffer)
                {
                    pointedAtType = TypeWithAnnotations.Create(new PointerTypeSymbol(pointedAtType));
                    if (ContainingType.TypeKind != TypeKind.Struct)
                    {
                        instance.Add(ErrorCode.ERR_FixedNotInStruct, ErrorLocation);
                    }
                    if (((PointerTypeSymbol)pointedAtType.Type).PointedAtType.FixedBufferElementSizeInBytes() == 0)
                    {
                        Location location = type.Location;
                        instance.Add(ErrorCode.ERR_IllegalFixedType, location);
                    }
                    if (!binder.InUnsafeRegion)
                    {
                        instance2.Add(ErrorCode.ERR_UnsafeNeeded, variableDeclaratorNode.Location);
                    }
                }
            }
            if (Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(pointedAtType.WithModifiers(base.RequiredCustomModifiers)), null) == null)
            {
                TypeChecks(pointedAtType.Type, instance);
                AddDeclarationDiagnostics(instance);
                if (fieldDeclaration.Declaration.Variables[0] == variableDeclaratorNode)
                {
                    AddDeclarationDiagnostics(instance2);
                }
                state.NotePartComplete(CompletionPart.Type);
            }
            instance.Free();
            instance2.Free();
            return _lazyType.Value;
        }

        internal bool FieldTypeInferred(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if (!ContainingType.IsScriptClass)
            {
                return false;
            }
            GetFieldType(fieldsBeingBound);
            if (_lazyFieldTypeInferred == 0)
            {
                return Volatile.Read(ref _lazyFieldTypeInferred) != 0;
            }
            return true;
        }

        protected sealed override ConstantValue MakeConstantValue(HashSet<SourceFieldSymbolWithSyntaxReference> dependencies, bool earlyDecodingWellKnownAttributes, BindingDiagnosticBag diagnostics)
        {
            if (!IsConst || VariableDeclaratorNode.Initializer == null)
            {
                return null;
            }
            return ConstantValueUtils.EvaluateFieldConstant(this, VariableDeclaratorNode.Initializer, dependencies, earlyDecodingWellKnownAttributes, diagnostics);
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (base.SyntaxTree == tree)
            {
                if (!definedWithinSpan.HasValue)
                {
                    return true;
                }
                BaseFieldDeclarationSyntax fieldDeclaration = GetFieldDeclaration(base.SyntaxNode);
                if (fieldDeclaration.SyntaxTree.HasCompilationUnitRoot)
                {
                    return fieldDeclaration.Span.IntersectsWith(definedWithinSpan.Value);
                }
                return false;
            }
            return false;
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            if (!IsFixedSizeBuffer)
            {
                base.Type.CheckAllConstraints(DeclaringCompilation, conversions, ErrorLocation, diagnostics);
            }
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
        }
    }
}
