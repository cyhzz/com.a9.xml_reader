using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System;
using Newtonsoft.Json;
using Excel;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using ICSharpCode.SharpZipLib;
using ICSharpCode;

#if UNITY_WEBGL
using WeChatWASM;
#endif

namespace Com.A9.FileReader
{
    public static class xmlReader
    {

#if UNITY_EDITOR
        //For development
        public static string path = Application.dataPath + "/IO/";
#else
    //For release
         public static string path=Application.persistentDataPath+"/";
#endif
        public static string game_data_path = Application.dataPath + $"/Resources/GameData/TheTable.xlsx";

        //     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        //     static void InitPath()
        //     {
        // #if UNITY_EDITOR
        //         //For development
        //         path = Application.dataPath + "/IO/";
        // #else
        //     //For release
        //          path=Application.persistentDataPath+"/";
        // #endif
        //         game_data_path = Application.dataPath + $"/Resources/GameData/TheTable.xlsx";
        //     }

        public static void SaveAsXml<T>(string fileName, T t)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.NewLineHandling = NewLineHandling.Entitize;
            ws.Encoding = Encoding.UTF8;
            ws.Indent = true;

            XmlWriter xW = XmlWriter.Create(path + fileName, ws);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            serializer.Serialize(xW, t);
            xW.Close();
        }
        public static void ReadXml<T>(string fileName, out T t)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            if (!File.Exists(path + fileName))
            {
                t = default(T);
                return;
            }
            FileStream stream = new FileStream(path + fileName, FileMode.Open);
            T result = (T)serializer.Deserialize(stream);
            stream.Close();
            t = result;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
    public static void SaveAsJson<T>(string fileName, T t, bool full_type = false)
    {
        string json = JsonConvert.SerializeObject(t);
        WXFileSystemManager fs = WX.GetFileSystemManager();
        fs.WriteFileSync(WX.env.USER_DATA_PATH + "/" + fileName, json, "utf-8");
        Debug.Log("save to local storage " + WX.env.USER_DATA_PATH);
    }
#else
        public static void SaveAsJson<T>(string fileName, T t, bool full_type = false)
        {
            if (full_type)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                string json = JsonConvert.SerializeObject(t, settings);
                System.IO.File.WriteAllText(path + fileName, json);
            }
            else
            {
                string json = JsonConvert.SerializeObject(t);
                System.IO.File.WriteAllText(path + fileName, json);
            }
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        public static void Delete(string fileName)
        {
            string json = JsonConvert.SerializeObject(t);
            WXFileSystemManager fs = WX.GetFileSystemManager();
            fs.Delete(WX.env.USER_DATA_PATH + "/" + fileName);
            Debug.Log("delete wechat local storage " + WX.env.USER_DATA_PATH);
        }
#else
        public static void Delete(string fileName)
        {
            if (!File.Exists(path + fileName))
            {
                return;
            }
            System.IO.File.Delete(path + fileName);
        }
#endif
        public static void DeleteAtPath(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            System.IO.File.Delete(path);
        }
#if UNITY_WEBGL && !UNITY_EDITOR
    public static void ReadJson<T>(string fileName, out T t, bool full_type = false)
    {
        WXFileSystemManager fs = WX.GetFileSystemManager();

        if (fs.AccessSync(WX.env.USER_DATA_PATH + "/" + fileName).Equals("access:ok"))
        {
            string data = fs.ReadFileSync(WX.env.USER_DATA_PATH + "/" + fileName, "utf-8");
            if (data != "")
            {
                t = JsonConvert.DeserializeObject<T>(data);
                Debug.Log("have local storage");
                return;
            }
        }
        Debug.Log("didnt have local storage");
        t = default(T);
    }
public static void ReadJson<T>(string fileName, out T t, JsonSerializerSettings setting)
    {
        WXFileSystemManager fs = WX.GetFileSystemManager();

        if (fs.AccessSync(WX.env.USER_DATA_PATH + "/" + fileName).Equals("access:ok"))
        {
            string data = fs.ReadFileSync(WX.env.USER_DATA_PATH + "/" + fileName, "utf-8");
            if (data != "")
            {
                t = JsonConvert.DeserializeObject<T>(data,setting);
                Debug.Log("have local storage");
                return;
            }
        }
        Debug.Log("didnt have local storage");
        t = default(T);
    }
#else
        public static void ReadJson<T>(string fileName, out T t, bool full_type = false)
        {
            if (!File.Exists(path + fileName))
            {
                t = default(T);
                return;
            }
            StreamReader r = null;
            try
            {
                r = new StreamReader(path + fileName);
                var json = r.ReadToEnd();
                if (full_type)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                    var items = JsonConvert.DeserializeObject<T>(json, settings);
                    t = items;
                }
                else
                {
                    var items = JsonConvert.DeserializeObject<T>(json);
                    t = items;
                }
                r.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                r?.Close();
                File.Delete(path + fileName);
                t = default(T);
            }
        }

        public static void ReadJson<T>(string fileName, out T t, JsonSerializerSettings setting)
        {
            if (!File.Exists(path + fileName))
            {
                t = default(T);
                return;
            }
            StreamReader r = null;
            try
            {
                r = new StreamReader(path + fileName);
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<T>(json, setting);
                t = items;
                r.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                r?.Close();
                File.Delete(path + fileName);
                t = default(T);
            }
        }
#endif
        public static void ReadJsonByPath<T>(string fileName, out T t)
        {
            StreamReader r = null;
            r = new StreamReader(fileName);
            var json = r.ReadToEnd();
            var items = JsonConvert.DeserializeObject<T>(json);
            t = items;
            r.Close();
        }

        public static DataRowCollection ReadExcel(string filePath, string table_name, ref int columnNum, ref int rowNum)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    DataSet result = excelReader.AsDataSet();
                    columnNum = result.Tables[table_name].Columns.Count;
                    rowNum = result.Tables[table_name].Rows.Count;
                    stream.Close();
                    return result.Tables[table_name].Rows;
                }
            }
        }

        public static T CloneJson<T>(this T source)
        {
            if (ReferenceEquals(source, null)) return default;

            var serializeSettings = new JsonSerializerSettings
            {

            };

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            serializeSettings.ContractResolver = new JsonIgnoreAttributeIgnorerContractResolver();
            deserializeSettings.ContractResolver = new JsonIgnoreAttributeIgnorerContractResolver();
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }

        public static T CloneReflection<T>(this T source) where T : new()
        {
            if (ReferenceEquals(source, null)) return default;

            var fd = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var n = new T();
            foreach (var item in fd)
            {

                item.SetValue(n, item.GetValue(source));
            }
            return n;
        }

        public static bool EditorFileExist(string sub_dir, string file_name)
        {
            var entries = Directory.GetFileSystemEntries(Application.dataPath + "/" + sub_dir, "*", SearchOption.AllDirectories).ToList();
            entries.Append(Application.dataPath + "/" + sub_dir + "/");

            bool found = false;
            entries.ForEach(c =>
            {
                if (c.Contains('.'))
                {
                    return;
                }
                FileInfo[] info = new DirectoryInfo(c).GetFiles("*.*");
                foreach (FileInfo f in info)
                {
                    if (f.Name.Contains(".meta"))
                    {
                        continue;
                    }
                    if (f.Name.Contains(file_name))
                    {
                        found = true;
                        return;
                    }
                }
            }
            );

            return found;
        }

        public static List<FileInfo> EditorGetAllFile(string sub_dir)
        {
            List<FileInfo> path = new List<FileInfo>();
            var entries = Directory.GetFileSystemEntries(Application.dataPath + "/" + sub_dir, "*", SearchOption.AllDirectories).ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                Debug.Log(entries[i]);
            }
            entries.Append(Application.dataPath + "/" + sub_dir + "/");

            entries.ForEach(c =>
            {
                if (c.Contains('.'))
                {
                    return;
                }
                FileInfo[] info = new DirectoryInfo(c).GetFiles("*.*");
                foreach (FileInfo f in info)
                {
                    if (f.Name.Contains(".meta"))
                    {
                        continue;
                    }
                    path.Add(f);
                }
            }
            );

            return path;
        }
    }
}

