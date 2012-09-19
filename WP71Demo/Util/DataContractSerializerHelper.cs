using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

public static class DataContractSerializerHelper
{
    public static string Serialize(object obj)
    {
        return Serialize(obj, null);
    }

    /// <summary>
    /// Serializes the specified obj in a compact form
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <param name="knownTypes">The known types.</param>
    /// <returns></returns>
    public static string Serialize(object obj, IEnumerable<Type> knownTypes)
    {
        DataContractSerializer ser = new DataContractSerializer(obj.GetType(), knownTypes);
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                //settings.NewLineOnAttributes = true;
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    ser.WriteObject(writer, obj);
                return sw.ToString();
            }
        }
    }

    public static T Deserialize<T>(string data)
    {
        return Deserialize<T>(data, null);
    }

    public static T Deserialize<T>(string data, IEnumerable<Type> knownTypes)
    {
        using (StringReader sr = new StringReader(data))
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(sr, settings))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T), knownTypes);
                return (T)ser.ReadObject(reader);
            }
        }
    }
}
