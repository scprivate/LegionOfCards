using System;
using System.IO;
using System.Reflection;
using LegionOfCards.Utils;
using Newtonsoft.Json;

namespace LegionOfCards.Data
{
    public class JsonStorage
    {
        private static readonly string DirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\json_storage";

        public static void Save(string name, object obj, bool sneaky = false)
        {
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }

            try
            {
                File.WriteAllText(DirPath + "\\" + name + ".json", JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
            catch(Exception e)
            {
                if(!sneaky)
                    Logger.Error("An Error occurred while saving to json storage", e);
            }
        }

        public static bool Exists(string name)
        {
            return File.Exists(DirPath + "\\" + name + ".json");
        }

        public static T Load<T>(string name, bool sneaky = false)
        {
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(DirPath + "\\" + name + ".json"));
            }
            catch (Exception e)
            {
                if (!sneaky)
                    Logger.Error("An Error occurred while loading from json storage!", e);
                return default(T);
            }
        }
    }
}
