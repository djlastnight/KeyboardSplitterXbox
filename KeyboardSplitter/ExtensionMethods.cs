namespace KeyboardSplitter
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public static class ExtensionMethods
    {
        public static string ToXml(this object obj)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
                return sb.ToString();
            }
        }
    }
}