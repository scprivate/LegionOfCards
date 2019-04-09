using System.Collections.Generic;
using LegionOfCards.Data.Stringle;
using Newtonsoft.Json;

namespace LegionOfCards.Data
{
    public class JsonData : Dictionary<string, object>
    {
        public bool Exists(string key)
        {
            return ContainsKey(key);
        }

        public void Delete(string key, bool check = true)
        {
            if (check && !Exists(key))
            {
                return;
            }

            Remove(key);
        }

        public JsonData Write(string key, object val, bool replace = true)
        {
            if (replace && Exists(key))
            {
                Delete(key, false);
            }

            Add(key, val);
            return this;
        }

        public object Read(string key, bool check = true)
        {
            if (check && !Exists(key))
            {
                return null;
            }

            return this[key];
        }

        public T Read<T>(string key, bool check = true)
        {
            object val = Read(key, check);
            if (val is T variable) return variable;
            return default(T);
        }

        public bool ReadBool(string key, bool check = true)
        {
            return Read<bool>(key, check);
        }

        public string ReadString(string key, bool check = true)
        {
            return Read<string>(key, check);
        }

        public JsonData ReadData(string key, bool check = true)
        {
            return Read<JsonData>(key, check);
        }

        public int ReadInt(string key, bool check = true)
        {
            return Read<int>(key, check);
        }

        public short ReadShort(string key, bool check = true)
        {
            return Read<short>(key, check);
        }

        public long ReadLong(string key, bool check = true)
        {
            return Read<long>(key, check);
        }

        public ushort ReadUShort(string key, bool check = true)
        {
            return Read<ushort>(key, check);
        }

        public uint ReadUInt(string key, bool check = true)
        {
            return Read<uint>(key, check);
        }

        public ulong ReadULong(string key, bool check = true)
        {
            return Read<ulong>(key, check);
        }

        public float ReadFloat(string key, bool check = true)
        {
            return Read<float>(key, check);
        }

        public double ReadDouble(string key, bool check = true)
        {
            return Read<double>(key, check);
        }

        public T ReadStringle<T>(string key, bool check = true) where T : StringleConvert
        {
            string data = ReadString(key, check);
            if (data == null) return null;
            return StringleConvert.Deserialize<T>(data);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JsonData FromString(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<JsonData>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
