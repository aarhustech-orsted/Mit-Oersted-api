using System;
using System.Text.Json;

namespace FoodieCommunityCase.Domain.Events.Serialization
{
    public class JsonSerializerExt
    {
        public string Serialize(object item)
        {
            string assemblyQualifiedName = item.GetType().AssemblyQualifiedName;
            string serializedValue = JsonSerializer.Serialize(item);

            return assemblyQualifiedName + "|" + serializedValue;
        }

        public object Deserialize(string serializedItem)
        {
            int index = serializedItem.IndexOf('|');

            string assemblyQualifiedName = serializedItem.Substring(0, index);
            string serializedValue = serializedItem.Substring(index + 1);

            Type type = Type.GetType(assemblyQualifiedName);
            return JsonSerializer.Deserialize(serializedValue, type);
        }
    }
}