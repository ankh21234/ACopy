﻿using System;
using System.Linq;
using System.Xml;

namespace ADatabase
{
    public class XmlConversionsReader : IXmlConversionsReader
    {
        private readonly ITypeDescriptionFactory _typeDescriptionFactory;

        public XmlConversionsReader(ITypeDescriptionFactory typeDescriptionFactory)
        {
            _typeDescriptionFactory = typeDescriptionFactory;
        }

        public XmlNode GetRootNode(string xmlText)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode rootNode;
            try
            {
                xmlDocument.LoadXml(xmlText);
                rootNode = xmlDocument.DocumentElement?.SelectSingleNode("/TypeConversions");
            }
            catch (Exception ex)
            {
                throw new XmlException("Error when reading conversion XML", ex);
            }
            if (rootNode == null) throw new XmlException("Can't find root element 'TypeConversions'");
            if (rootNode.Attributes == null) throw new XmlException("Root element 'TypeConversions' has no attributes");
            var sourceSystem = rootNode.Attributes["From"]?.InnerText;
            if (string.IsNullOrWhiteSpace(sourceSystem))
                throw new XmlException("Error with attribute 'From' for 'TypeConversions'");
            var destinationSystem = rootNode.Attributes["To"]?.InnerText;
            if (string.IsNullOrWhiteSpace(destinationSystem))
                throw new XmlException("Error with attribute 'To' for 'TypeConversions'");

            if (rootNode.ChildNodes.Count == 0) throw new XmlException("No conversions found");

            return rootNode;
        }

        public ITypeDescription GetColumnTypeDescription(XmlNode xmlNode)
        {
            var colDesc = _typeDescriptionFactory.GetColumnTypeDescription();
            GetTypeAttributes(xmlNode, colDesc);

            if (!xmlNode.HasChildNodes) return colDesc;

            CheckTypeDetails(xmlNode);
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                GetTypeConstraints(childNode, colDesc);
            }

            return colDesc;
        }

        public string GetSourceSystem(XmlNode rootNode)
        {
            return rootNode.Attributes?["From"]?.InnerText;
        }

        public string GetDestinationSystem(XmlNode rootNode)
        {
            return rootNode.Attributes?["To"]?.InnerText;
        }

        #region Private

        private static bool IsLegalTypeDetail(string detail)
        {
            return detail == "Prec" || detail == "Scale" || detail == "Length";
        }

        private void CheckTypeDetails(XmlNode xmlNode)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (!IsLegalTypeDetail(childNode.Name)) throw new XmlException($"Illegal type detail '{childNode.Name}' for type '{xmlNode.Attributes?["Source"].InnerText}'");
            }
        }

        private static void GetTypeAttributes(XmlNode xmlNode, ITypeDescription colDesc)
        {
            var sourceType = xmlNode.Attributes?["Source"]?.InnerText;
            if (string.IsNullOrWhiteSpace(sourceType))
                throw new XmlException("Error with attribute 'Name' for 'Type'");
            var destinationType = xmlNode.Attributes?["Destination"]?.InnerText;
            if (string.IsNullOrWhiteSpace(destinationType))
                throw new XmlException("Error with attribute 'To' for 'Type'");

            colDesc.TypeName = sourceType;
            colDesc.ConvertTo = destinationType;
        }

        private static void GetTypeConstraints(XmlNode xmlNode, ITypeDescription colDesc)
        {
            var constraintName = xmlNode.Name;
            var opName = xmlNode.Attributes?["Operator"].InnerText;
            if (opName == "in")
            {
                var constraintValues = xmlNode.InnerText.Split(',').Select(v => Convert.ToInt32(v));
                colDesc.AddConstraint(constraintName, opName, constraintValues);
            }
            else
            {
                var constraintValue = Convert.ToInt32(xmlNode.InnerText);
                colDesc.AddConstraint(constraintName, opName, constraintValue);
            }
        }

        #endregion
    }
}