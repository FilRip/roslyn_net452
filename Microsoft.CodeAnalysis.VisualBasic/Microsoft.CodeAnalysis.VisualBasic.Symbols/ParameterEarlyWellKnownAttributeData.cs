namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ParameterEarlyWellKnownAttributeData : CommonParameterEarlyWellKnownAttributeData
	{
		private bool _hasMarshalAsAttribute;

		private bool _hasParamArrayAttribute;

		internal bool HasMarshalAsAttribute
		{
			get
			{
				return _hasMarshalAsAttribute;
			}
			set
			{
				_hasMarshalAsAttribute = value;
			}
		}

		internal bool HasParamArrayAttribute
		{
			get
			{
				return _hasParamArrayAttribute;
			}
			set
			{
				_hasParamArrayAttribute = value;
			}
		}
	}
}
