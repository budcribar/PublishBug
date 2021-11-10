using Newtonsoft.Json;

namespace Components.Shared
{
    public static class ExtensionMethods
    {
        public static T? DeepCopy<T>(this T obj)
        {

            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
