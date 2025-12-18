using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Debug = UnityEngine.Debug;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Utilities
{
    /// <summary>
    /// JSON快速读写操作封装静态类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 保存为文件时的文件扩展名
        /// </summary>
        public const string SavedFileExtension = ".json";
        /// <summary>
        /// 保存压缩数据文件时的文件扩展名
        /// </summary>
        public const string CompressedFileExtension = ".jbin";

        /// <summary>
        /// 从指定路径读取JSON数据，并将其保存到指定的变量中
        /// </summary>
        /// <typeparam name="T">要保存的数据类型，应为结构体</typeparam>
        /// <param name="filePath">要保存到的文件完整路径</param>
        /// <param name="data">要输出的数据</param>
        /// <param name="compress">是否经过了压缩，非超大数据一般无需压缩</param>
        /// <exception cref="Exception">文件路径不存在时报错</exception>
        public static void Load<T>(string filePath, out T data, bool compress = false)
        {
            var str = "";
            //打开文件并读取
            if (!File.Exists(filePath)) throw new Exception($"{filePath}不存在或已被移动！");
            if (compress)
            {
                //读取压缩的JSON数据
                str = Decompress(File.ReadAllBytes(filePath));
            }
            else
            {
                //读取未压缩的JSON数据
                using (var sr = File.OpenText(filePath))
                {
                    str = sr.ReadToEnd();
                    sr.Close();
                }
            }
            //载入所读取的数据
            data = JsonConvert.DeserializeObject<T>(str);
            Debug.Log($"已成功从{filePath}读取JSON数据");
        }

        /// <summary>
        /// 将数据以JSON格式保存到指定路径
        /// </summary>
        /// <typeparam name="T">要保存的数据类型，应为结构体</typeparam>
        /// <param name="data">要保存的数据</param>
        /// <param name="filePath">完整文件路径</param>
        /// <param name="compress">是否要进行压缩，非超大数据一般无需压缩</param>
        public static void Save<T>(T data, string filePath, bool compress = false)
        {
            //不存在配置时，将其保存
            if (!File.Exists(filePath)) File.Create(filePath).Close();
            //将数据转为JSON，并进行保存
            var str = JsonConvert.SerializeObject(data, Formatting.Indented);
            //文件写入流
            if (compress)
            {
                //压缩数据并写入文件
                var compressedData = Compress(str);
                File.WriteAllBytes(filePath, compressedData);
                Debug.Log($"已将JSON数据压缩并保存到{filePath}， (原始大小: {str.Length} 压缩后: {compressedData.Length})");
            }
            else
            {
                using (var sw = new StreamWriter(filePath, false))
                {
                    sw.WriteLine(str);
                    sw.Close();
                }
                Debug.Log($"已将JSON数据保存到{filePath}");
            }
        }

        /// <summary>
        /// 将数据保存到持久化路径下的文件中
        /// </summary>
        /// <typeparam name="T">支持转为JSON的数据类型</typeparam>
        /// <param name="data">要保存的数据对象</param>
        /// <param name="fileName">要保存到的文件名，不带后缀名与根路径</param>
        /// <param name="compress">是否经过压缩</param>
        public static void SaveToPersistent<T>(T data, string fileName, bool compress = false) =>
            Save(data, Path.Combine(Application.persistentDataPath, fileName + (compress ? CompressedFileExtension : SavedFileExtension)), compress);

        /// <summary>
        /// 从持久化路径下的文件中读取数据
        /// </summary>
        /// <typeparam name="T">支持转为JSON的数据类型</typeparam>
        /// <param name="fileName">要进行读取的文件名，不带后缀名与根路径</param>
        /// <param name="data">要读取到的数据对象</param>
        /// <param name="compress">是否经过压缩</param>
        public static void LoadFromPersistent<T>(string fileName, out T data, bool compress = false) =>
            Load(Path.Combine(Application.persistentDataPath, fileName + (compress ? CompressedFileExtension : SavedFileExtension)), out data, compress);

        /// <summary>
        /// 将数据保存到StreamingAssets路径下的文件中
        /// </summary>
        /// <typeparam name="T">要保持的数据结构泛型</typeparam>
        /// <param name="data">要保存的数据</param>
        /// <param name="relativePath">文件相对于StreamingAsset的路径</param>
        /// <param name="compress">是否使用压缩</param>
        public static void SaveToStreamingAssets<T>(T data, string relativePath, bool compress = false) =>
            Save(data, Path.Combine(Application.streamingAssetsPath, relativePath), compress);

        /// <summary>
        /// 从StreamingAssets路径下的文件中读取数据
        /// </summary>
        /// <typeparam name="T">要读取的数据结构</typeparam>
        /// <param name="relativePath">文件相对于StreamingAsset的路径</param>
        /// <param name="data">读取到的数据</param>
        /// <param name="compress">是否经过了压缩</param>
        public static void LoadFromStreamingAssets<T>(string relativePath, out T data, bool compress = false) =>
            Load(Path.Combine(Application.streamingAssetsPath, relativePath), out data, compress);

        #region 压缩与解压缩操作
        /// <summary>
        /// 将字符串数据为字节数组并进行压缩
        /// </summary>
        /// <param name="input">输入的字符串数据</param>
        /// <returns>经过压缩的字节数据</returns>
        private static byte[] Compress(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将压缩的字节数组解压缩为字符串
        /// </summary>
        /// <param name="compressedData">从文件中读取到的要进行解压缩的数据</param>
        /// <returns>解压缩后得到的字符串数据</returns>
        private static string Decompress(byte[] compressedData)
        {
            using (MemoryStream inputStream = new MemoryStream(compressedData))
            using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gzip.CopyTo(outputStream);
                return System.Text.Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }
        #endregion
    }

    /// <summary>
    /// JSON配置数据加载器
    /// </summary>
    /// <typeparam name="T">配置数据结构</typeparam>
    public abstract class JsonConfigLoader<T> :MonoBehaviour where T: struct
    {
        [SerializeField]
        [Tooltip("游戏配置数据")]
        private T _config;

        [SerializeField]
        [Tooltip("是否在Awake时自动加载配置文件")]
        protected bool autoLoadOnAwake = true;

        /// <summary>
        /// WORKFLOW:重写该属性以指定配置文件在StreamingAssets中的路径
        /// </summary>
        protected abstract string FilePathInStreamingAssets { get;}

        /// <summary>
        /// 获取配置数据
        /// </summary>
        public T Config => _config;

        private void Awake()
        {
            if (autoLoadOnAwake) LoadGameConfig();
        }

        /// <summary>
        /// 将当前配置数据保存到JSON文件中
        /// </summary>
#if ODIN_INSPECTOR
        [Button("保存游戏配置")]
#else
        [ContextMenu("保存游戏配置")]
#endif
        public void SaveGameConfig()
        {
            JsonHelper.SaveToStreamingAssets(_config, FilePathInStreamingAssets, false);
        }

        /// <summary>
        /// 从JSON文件中加载配置数据
        /// </summary>
#if ODIN_INSPECTOR
        [Button("加载游戏配置")]
#else
        [ContextMenu("加载游戏配置")]
#endif
        public void LoadGameConfig()
        {
            JsonHelper.LoadFromStreamingAssets(FilePathInStreamingAssets, out _config);
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
#if ODIN_INSPECTOR
        [Button("打开配置文件")]
#else
        [ContextMenu("打开配置文件")]
#endif
        public void OpenGameConfigFile()
        {
            Process.Start(Path.Combine(Application.streamingAssetsPath, FilePathInStreamingAssets));
        }
    }

    /// <summary>
    /// 单例模式下的JSON配置数据加载器
    /// </summary>
    /// <typeparam name="T">配置数据结构</typeparam>
    public abstract class SingleTonJsonConfigLoader<T> : JsonConfigLoader<T> where T : struct
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        private static SingleTonJsonConfigLoader<T> _instance;
        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static SingleTonJsonConfigLoader<T> Instance => _instance ??= FindFirstObjectByType<SingleTonJsonConfigLoader<T>>();

        /// <summary>
        /// 确保单例，若继承应当放在最前
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance && _instance != this) Destroy(this);
            else _instance = this;
            if (autoLoadOnAwake) LoadGameConfig();
        }
        protected virtual void OnDestroy() { if (_instance == this) _instance = null; }
    }
}
