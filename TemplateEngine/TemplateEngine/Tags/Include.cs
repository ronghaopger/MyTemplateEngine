using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.Data;
using RHClassLibrary.Data;
using System.IO;
using RHClassLibrary;
using TemplateEngine.TemplateEngine.Handle;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class Include : TagsAbstractCls
    {
        #region 属性
        /// <summary>
        /// 文件名称
        /// </summary>
        private String filename = String.Empty;
        public String Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
        #endregion

        /// <summary>
        /// 组装html
        /// </summary>
        /// <param name="temContent">模板页所有内容</param>
        /// <param name="flagArray">标记此标签的起止的索引的数组。</param>
        /// <returns></returns>
        public override String AssembleSelf(String temContent, int[] flagArray)
        {
            //构造模板所在的绝对路径
            String temMapPath = PathHelper.GetMapPath(@"/admin/Template/" + this.Filename);
            if (!File.Exists(temMapPath))
            {
                throw new FileNotFoundException(this.Filename + " 子模板不存在。");
            }
            //解释模板文件
            TagHandle.ExplainTemplate(temMapPath,@"/web/common/");
            //构造生成的静态文件的绝对路径
            String htmMapPath = PathHelper.GetMapPath(@"/web/common/" + this.Filename);
            using(StreamReader sr = new StreamReader(htmMapPath,Encoding.GetEncoding("GB2312")))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
