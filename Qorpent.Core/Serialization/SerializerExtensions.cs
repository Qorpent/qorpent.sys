using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Qorpent.Json;

namespace Qorpent.Serialization {
    /// <summary>
    /// ���������� ��� ��������� ������ � ��������������
    /// </summary>
    public static class SerializerExtensions {
        /// <summary>
        /// ��� �� ��������� ��� ������������� ��������
        /// </summary>
        public const string DefaultObjectName = "root";

        /// <summary>
        /// ��������� ������������ � ������
        /// </summary>
        /// <param name="serializer">������� ������������</param>
        /// <param name="value">������������� ������</param>
        /// <param name="name">��� ��� �������������� �������</param>
        /// <returns></returns>
        public static string Serialize(this ISerializer serializer, object value, string name = DefaultObjectName) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = DefaultObjectName;
            }
            var sw = new StringWriter();
            serializer.Serialize(name, value, sw);
            return sw.ToString();
        }
        /// <summary>
        /// ������������ ������ � �������� ������� � �������� �����
        /// </summary>
        /// <param name="value">������������� ������</param>
        /// <param name="format">������</param>
        /// <param name="output">������� �����</param>
        /// <param name="objectname">��� ��� �������������� �������</param>
        public static void SerializeToFormat(this object value, SerializationFormat format, TextWriter output,
                                             string objectname = DefaultObjectName) {
            if (null==output) throw new SerializationException("output not given");
            var serializer = Applications.Application.Current.Serialization.GetSerializer(format);
            if (string.IsNullOrWhiteSpace(objectname)) {
                objectname = DefaultObjectName;
            }
            serializer.Serialize(objectname, value, output);
        }
        /// <summary>
        /// ������������ ������ � �������� ������� � ����������� � �������� ����
        /// </summary>
        /// <param name="value">������������� ������</param>
        /// <param name="format">������</param>
        /// <param name="filename">��� ��������</param>
        /// <param name="objectname">��� ��� �������������� �������</param>
        public static void SerializeToFormat(this object value, SerializationFormat format, string filename,
                                             string objectname = DefaultObjectName)
        {
            if(string.IsNullOrWhiteSpace(filename))throw new SerializationException("filename not given");
            var serializer = Applications.Application.Current.Serialization.GetSerializer(format);
            if (string.IsNullOrWhiteSpace(objectname))
            {
                objectname = DefaultObjectName;
            }
            if (!Path.IsPathRooted(filename)) {
                filename = Path.GetFullPath(filename);
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }
            using (var sw = new StreamWriter(filename)) {
                serializer.Serialize(objectname, value, sw);    
                sw.Flush();
            }
            
        }
        /// <summary>
        /// ���������� �������� ������������� ������� � ��������� �������
        /// </summary>
        /// <param name="value">������������� ������</param>
        /// <param name="format">������</param>
        /// <param name="objectname">��� ��� �������������� �������</param>
        public static string SerializeAsFormat(this object value, SerializationFormat format,string objectname = DefaultObjectName) {
            var serializer = Applications.Application.Current.Serialization.GetSerializer(format);
            if (string.IsNullOrWhiteSpace(objectname)) {
                objectname = DefaultObjectName;
            }
            return serializer.Serialize(value,objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ��������� ����� � ������� XML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="output"></param>
        /// <param name="objectname"></param>
        public static void SerializeToXml(this object value, TextWriter output,string objectname= DefaultObjectName) {
            value.SerializeToFormat(SerializationFormat.Xml, output, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ��������� ����� � ������� JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="output"></param>
        /// <param name="objectname"></param>
        public static void SerializeToJson(this object value, TextWriter output, string objectname = DefaultObjectName)
        {
            value.SerializeToFormat(SerializationFormat.Json, output, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ��������� ����� � ������� HTML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="output"></param>
        /// <param name="objectname"></param>
        public static void SerializeToHtml(this object value, TextWriter output, string objectname = DefaultObjectName)
        {
            value.SerializeToFormat(SerializationFormat.Html, output, objectname);
        }

        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� XML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filename"></param>
        /// <param name="objectname"></param>
        public static void SerializeToXml(this object value, string filename, string objectname = DefaultObjectName)
        {
            value.SerializeToFormat(SerializationFormat.Xml, filename, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filename"></param>
        /// <param name="objectname"></param>
        public static void SerializeToJson(this object value, string filename, string objectname = DefaultObjectName)
        {
            value.SerializeToFormat(SerializationFormat.Json, filename, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� HTML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filename"></param>
        /// <param name="objectname"></param>
        public static void SerializeToHtml(this object value, string filename, string objectname = DefaultObjectName)
        {
            value.SerializeToFormat(SerializationFormat.Html, filename, objectname);
        }

        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� XML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objectname"></param>
        public static string SerializeAsXmlString(this object value,  string objectname = DefaultObjectName)
        {
            return value.SerializeAsFormat(SerializationFormat.Xml, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� XML
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objectname"></param>
        public static XElement SerializeAsXml(this object value, string objectname = DefaultObjectName) {
            return XElement.Parse(value.SerializeAsXmlString(objectname));
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objectname"></param>
        public static string SerializeAsJsonString(this object value, string objectname = DefaultObjectName)
        {
            return value.SerializeAsFormat(SerializationFormat.Json, objectname);
        }
        /// <summary>
        /// ����������� ��������� ������ � ������� ���� � ������� JSON
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objectname"></param>
        public static JsonItem SerializeAsJson(this object value, string objectname = DefaultObjectName) {
            var parser = Applications.Application.Current.Container.Get<IJsonParser>();
            if (null == parser) {
                throw new SerializationException("json parser not configured in application");
            }
            var jsonstring = value.SerializeAsJsonString(objectname);
            return parser.Parse(jsonstring);
        }
        
    }
}