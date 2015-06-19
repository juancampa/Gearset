using System.IO;
using System.Runtime.Serialization;
//using System.Xml;

namespace Gearset
{
    public class BinarySerializer
    {
        public void Serialize<T>(Stream stream, T obj)
        {
            var serializer = new DataContractSerializer(typeof(T));
            //using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            //{
            //    serializer.WriteObject(writer, obj);
            //}

            serializer.WriteObject(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T));
            //using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
            //{
            //    return (T)serializer.ReadObject(reader);
            //}

            return (T)serializer.ReadObject(stream);
        }
    }
}
