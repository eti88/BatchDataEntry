using MadMilkman.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BatchDataEntry.Business
{
    public static class Cache
    {
        public static bool CreateFile(string file_path)
        {
            IniFile file = new IniFile();
            file.Save(file_path);
            if (File.Exists(file_path))
                return true;
            else
                return false;
        }

        public static void AddSection(string fileName, string sectionName)
        {
            if(!File.Exists(fileName))
                return;

            IniFile file = new IniFile();
            file.Load(fileName);
            file.Sections.Add(sectionName);
            file.Save(fileName);
        }

        public static void AddKeyToSection(string fileName, string sectionName, string keyName, string keyValue)
        {
            if (!File.Exists(fileName))
                return;

            IniFile file = new IniFile();
            file.Load(fileName);
            IniSection section = file.Sections[sectionName];
            section.Keys.Add(keyName, keyValue);
            file.Save(fileName);
        }

        public static void AddSeriazliedValueToSection(string fileName, string sectionName, string keyName, string[] val)
        {
            if (!File.Exists(fileName))
                return;

            IniFile file = new IniFile();
            file.Load(fileName);
            IniSection section = file.Sections[sectionName];
            section.Keys.Add(keyName, JsonConvert.SerializeObject(val));
            file.Save(fileName);
        }

        public static string[] GetSerializedValue(string fileName, string sectionName, string keyName)
        {
            if (!File.Exists(fileName))
                return null;

            IniFile file = new IniFile();
            try
            {
                file.Load(fileName);
                IniSection section = file.Sections[sectionName];
                string val = section.Keys[keyName].Value;
                string[] res = JsonConvert.DeserializeObject<string[]>(val);

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void AddMultipleSection(string fileName, List<string> sections)
        {
            if (!File.Exists(fileName))
                return;

            IniFile file = new IniFile();
            file.Load(fileName);

            foreach (string s in sections)
            {
                file.Sections.Add(s);
            }
            file.Save(fileName);
        }

        public static void AddMultipleKeyToSection(string fileName, string sectionName, string[] keys, string[] values)
        {
            if(keys.Length != values.Length)
                return;

            if (!File.Exists(fileName))
                return;

            IniFile file = new IniFile();
            file.Load(fileName);
            IniSection section = file.Sections[sectionName];

            string[] tmp = values.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                section.Keys.Add(keys[i], values[i]);
            }
            file.Save(fileName);
        }

        public static Dictionary<string, string> GetKey(string fileName, string sectionName, string keyName)
        {
            if (!File.Exists(fileName))
                return null;

            Dictionary<string, string> result = new Dictionary<string, string>();

            IniFile file = new IniFile();
            file.Load(fileName);
            IniSection section = file.Sections[sectionName];
            IniKey key = section.Keys[keyName];
            result.Add(key.Name, key.Value);
            return result;
        }

        public static List<string> GetValuesFromSection(string fileName, string sectionName)
        {
            List<string> res = new List<string>();

            if (!File.Exists(fileName))
                return null;

            IniFile file = new IniFile();
            file.Load(fileName);
            IniSection section = file.Sections[sectionName];
            foreach (IniKey key in section.Keys)
            {
                res.Add(key.Value);
            }
            return res;
        }
    }
}
