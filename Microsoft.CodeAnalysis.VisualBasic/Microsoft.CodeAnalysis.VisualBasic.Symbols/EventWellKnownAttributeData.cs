namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class EventWellKnownAttributeData : CommonEventWellKnownAttributeData
	{
		private bool _hasNonSerializedAttribute;

		internal bool HasNonSerializedAttribute
		{
			get
			{
				return _hasNonSerializedAttribute;
			}
			set
			{
				_hasNonSerializedAttribute = value;
			}
		}

		public EventWellKnownAttributeData()
		{
			_hasNonSerializedAttribute = false;
		}
	}
}
