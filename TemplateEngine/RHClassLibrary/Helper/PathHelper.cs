///创建人：荣浩
///2013.9.2
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RHClassLibrary
{
    public class PathHelper
    {
        /// <summary>
        /// 返回指定路径中的文件名称
        /// </summary>
        /// <param name="path">指定的路径</param>
        /// <returns></returns>
        public static String GetFileName(String path)
        {
            String[] pathArray = path.Split('\\');
            return pathArray[pathArray.Length - 1];
        }
        /// <summary>
        /// 返回除去文件名称以外的路径
        /// </summary>
        /// <param name="path">指定的路径</param>
        /// <returns></returns>
        public static String GetPathWithoutName(String path)
        {
            String[] pathArray = path.Split('\\');
            String pathReturn = String.Empty;
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                pathReturn += pathArray[i] + "\\";
            }
            return pathReturn;
        }
        /// <summary>
        /// 返回绝对路径
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        public static String GetMapPath(String path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
