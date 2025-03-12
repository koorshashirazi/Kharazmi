using System.IO;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Localization.Json.Internal
{
    internal static class JsonReader
    {
        private static readonly object Lock = typeof(object);

        public static T Read<T>(string path) where T : new()
        {
            lock (Lock)
            {
                var jsonObject = new T();
                if (!File.Exists(path))
                {
                    Write(jsonObject, path);
                }

                jsonObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return jsonObject;
            }
        }

        public static void Write<T>(T jsonObject, string path)
        {
            lock (Lock)
            {
                var output = JsonConvert.SerializeObject(jsonObject);
                var directoryName = Path.GetDirectoryName(path);
                
                if(!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                
                File.WriteAllText(path, output);
            }
        }
    }
}