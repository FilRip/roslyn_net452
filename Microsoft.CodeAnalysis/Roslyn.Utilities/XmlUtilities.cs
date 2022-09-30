using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

#nullable enable

namespace Roslyn.Utilities
{
    public static class XmlUtilities
    {
        public static TNode Copy<TNode>(this TNode node, bool copyAttributeAnnotations) where TNode : XNode
        {
            XNode xNode;
            if (node.NodeType == XmlNodeType.Document)
            {
                xNode = new XDocument((XDocument)(object)node);
            }
            else
            {
                XElement xElement = new XElement("temp");
                xElement.Add(node);
                xNode = xElement.LastNode;
                xElement.RemoveNodes();
            }
            CopyAnnotations(node, xNode);
            if (copyAttributeAnnotations && node.NodeType == XmlNodeType.Element)
            {
                XElement obj = (XElement)(object)node;
                XElement xElement2 = (XElement)xNode;
                IEnumerator<XAttribute> enumerator = obj.Attributes().GetEnumerator();
                IEnumerator<XAttribute> enumerator2 = xElement2.Attributes().GetEnumerator();
                while (enumerator.MoveNext() && enumerator2.MoveNext())
                {
                    CopyAnnotations(enumerator.Current, enumerator2.Current);
                }
            }
            return (TNode)xNode;
        }

        private static void CopyAnnotations(XObject source, XObject target)
        {
            foreach (object item in source.Annotations<object>())
            {
                target.AddAnnotation(item);
            }
        }

        public static XElement[]? TrySelectElements(XNode node, string xpath, out string? errorMessage, out bool invalidXPath)
        {
            errorMessage = null;
            invalidXPath = false;
            try
            {
                return node.XPathSelectElements(xpath)?.ToArray();
            }
            catch (InvalidOperationException ex)
            {
                errorMessage = ex.Message;
                return null;
            }
            catch (Exception ex2) when (ex2.GetType().FullName == "System.Xml.XPath.XPathException")
            {
                errorMessage = ex2.Message;
                invalidXPath = true;
                return null;
            }
        }
    }
}
