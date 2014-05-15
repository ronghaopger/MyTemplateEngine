using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TemplateEngine.TemplateEngine
{
    public class TemplateHandle
    {
        //TODO:缓存模板内容
        //TODO:异常处理
        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="savePath">模板所在路径</param>
        /// <returns>读取到的内容</returns>
        public static String ReadTemplate(String savePath)
        {
            if (!File.Exists(savePath))
            {
                throw new FileNotFoundException("未找到指定路径的模板：" + savePath);
            }
            using (StreamReader sr = new StreamReader(savePath, Encoding.GetEncoding("GB2312")))
            {
                return sr.ReadToEnd();
            }
        }
        /// <summary>
        /// 清空指定路径的文件内容，并更新为新内容。
        /// </summary>
        /// <param name="savePath">文件路径</param>
        /// <param name="temContent">新路径</param>
        public static void SaveTemplate(String savePath, String temContent)
        {
            if (!File.Exists(savePath))
            {
                throw new FileNotFoundException("未找到指定路径的模板：" + savePath);
            }
            using (StreamWriter sw = new StreamWriter(savePath, false, Encoding.GetEncoding("GB2312")))
            {
                sw.Write(temContent);
                sw.Flush();
                sw.Close();
            }
        }
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="savePath">模板路径</param>
        public static void DeleteTemplate(String savePath)
        {
            File.Delete(savePath);
        }
    }
}
