using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedEventBackingFieldSymbol : SynthesizedBackingFieldBase<SourceEventSymbol>
	{
		private TypeSymbol _lazyType;

		internal override bool IsNotSerialized => _propertyOrEvent.GetDecodedWellKnownAttributeData()?.HasNonSerializedAttribute ?? false;

		public override TypeSymbol Type
		{
			get
			{
				if ((object)_lazyType == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					TypeSymbol typeSymbol = _propertyOrEvent.Type;
					if (_propertyOrEvent.IsWindowsRuntimeEvent)
					{
						NamedTypeSymbol wellKnownType = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T);
						instance.Add(Binder.GetUseSiteInfoForWellKnownType(wellKnownType), _propertyOrEvent.Locations[0]);
						typeSymbol = wellKnownType.Construct(typeSymbol);
					}
					((SourceModuleSymbol)ContainingModule).AtomicStoreReferenceAndDiagnostics(ref _lazyType, typeSymbol, instance);
					instance.Free();
				}
				return _lazyType;
			}
		}

		public SynthesizedEventBackingFieldSymbol(SourceEventSymbol propertyOrEvent, string name, bool isShared)
			: base(propertyOrEvent, name, isShared)
		{
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			_ = Type;
		}
	}
}
