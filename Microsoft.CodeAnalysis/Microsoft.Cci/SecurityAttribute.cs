using System.Reflection;

namespace Microsoft.Cci
{
    public struct SecurityAttribute
    {
        public DeclarativeSecurityAction Action { get; }

        public ICustomAttribute Attribute { get; }

        public SecurityAttribute(DeclarativeSecurityAction action, ICustomAttribute attribute)
        {
            Action = action;
            Attribute = attribute;
        }
    }
}
