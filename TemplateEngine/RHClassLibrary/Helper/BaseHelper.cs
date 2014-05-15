using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.Xml.Serialization;
using RHClassLibrary;

namespace RHClassLibrary
{
    /// <summary>
    /// 缓存时间
    /// </summary>
    public enum CacheTime
    {
        Short = 10,
        Medium = 20,
        Long = 30
    }

    /// <summary>
    /// 逻辑层助手处理基类
    /// </summary>
    [Serializable]
    public abstract class BaseHelper : IHelper
    {
        #region 属性区域

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseHelper()
        {
        }

        string name;
        /// <summary>
        /// 当前Helper名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string description;
        /// <summary>
        /// 当前Helper作用说明
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        string root;
        /// <summary>
        /// 当前Helper的程序集所处目录（物理）
        /// </summary>
        public string Root
        {
            get { return root; }
            set { root = value; }
        }


        #endregion

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cacheKey">关键字</param>
        /// <param name="context">Http上下文</param>
        /// <param name="obj">数据</param>
        /// <param name="ct">缓存时间</param>
        public void CacherCache(string cacheKey, HttpContext context, object obj, CacheTime ct)
        {
            if (obj != null)
            {
                context.Cache.Insert(cacheKey, obj, null, DateTime.Now.AddSeconds((int)ct), TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        bool Save(object obj, string filename)
        {
            bool success = false;

            FileStream fs = null;
            // serialize it...
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return success;

        }
    }
}
