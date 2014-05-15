using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RHClassLibrary;
using TemplateEngine.RHClassLibrary.Exception;
using RHClassLibrary.Data;

namespace TemplateEngine.TemplateEngine
{
    public abstract class TagsAbstractCls
    {
        /// <summary>
        /// 解释并替换标签内容
        /// </summary>
        /// <param name="temContent">模板页所有内容</param>
        /// <param name="flag">标记此标签的起止的索引的字符串。</param>
        public void ExplainSelf(object obj)
        {
            String[] sArray = (String[])obj;
            String savePath = sArray[0];
            String flag = sArray[1];
            //读取模板内容
            String temContent = TemplateHandle.ReadTemplate(savePath);
            //分解起止索引
            int[] flagArray = Array.ConvertAll<String, int>(flag.Split('$'), delegate(string s) { return int.Parse(s); });
            //解析标签内容
            AnalyzeTag(temContent, flagArray);
            //组装html，并返回
            String newStr = AssembleSelf(temContent, flagArray);
            //用组装的新字符串去替换标签。
            if (flagArray.Length == 4)
            {
                temContent = temContent.Remove(flagArray[0], flagArray[3] - flagArray[0]).Insert(flagArray[0], newStr);
            }
            else if (flagArray.Length == 2)
            {
                temContent = temContent.Remove(flagArray[0], flagArray[1] - flagArray[0]).Insert(flagArray[0], newStr);
            }
            //保存模板
            TemplateHandle.SaveTemplate(savePath, temContent);
        }
        /// <summary>
        /// 解析标签内容
        /// </summary>
        /// <param name="temContent">模板页所有内容</param>
        /// <param name="flagArray">标记此标签的起止的索引的数组。</param>
        public void AnalyzeTag(String temContent, int[] flagArray)
        {
            Type tThis = this.GetType();
            PropertyInfo[] pArray = tThis.GetProperties();
            //循环此类对象的属性，一旦用户有设置就赋值。
            foreach (PropertyInfo pi in pArray)
            {
                String tagStr = temContent.Substring(flagArray[0], flagArray[1] - flagArray[0]);
                int piIndex = tagStr.IndexOf(pi.Name.ToLower());
                //若用户设置了指定属性，就去一步步读取指定属性的值。指定属性的值必须包含在''中。
                if (piIndex != -1 && StringHelper.judgeNextChar(tagStr.Substring(piIndex + pi.Name.Length), 0, "="))
                {
                    tagStr = tagStr.Substring(piIndex + pi.Name.Length + 1);
                    if (StringHelper.judgeNextChar(tagStr, 0, "'"))
                    {
                        tagStr = tagStr.Substring(tagStr.IndexOf("'") + 1);
                        tagStr = tagStr.Substring(0, tagStr.IndexOf("'"));
                        if (pi.PropertyType == typeof(int))
                        {
                            pi.SetValue(this, int.Parse(tagStr), null);
                        }
                        else
                        {
                            pi.SetValue(this, tagStr, null);
                        }
                    }
                    else
                    {
                        throw new TagErrorException("属性没有设置相应的值，即'='后没有'值'。");
                    }
                }
            }
        }
         /// <summary>
        /// 组装html
        /// </summary>
        /// <param name="temContent">模板页所有内容</param>
        /// <param name="flagArray">标记此标签的起止的索引的数组。</param>
        /// <returns></returns>
        public abstract String AssembleSelf(String temContent, int[] flagArray);
        /// <summary>
        /// 数据库引擎
        /// </summary>
        private SqlHelper _sh = new SqlHelper();
        public SqlHelper sh
        {
            get { return _sh; }
        }
    }
}
