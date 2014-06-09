using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.IO;
using RHClassLibrary;
using System.Reflection;
using TemplateEngine.TemplateEngine.Handle;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class PageList //: ITags
    {
        #region 属性
        /// <summary>
        /// 表示 [1][2][3] 这些项的长度
        /// </summary>
        private int listsize = 1;
        public int ListSize
        {
            get
            {
                return listsize;
            }
            set
            {
                listsize = value;
            }
        }
        /// <summary>
        /// 表示页码样式
        /// </summary>
        private String listitem = "index,info,end";
        public String ListItem
        {
            get
            {
                return listitem;
            }
            set
            {
                listitem = value;
            }
        }
        /// <summary>
        /// 文章类别
        /// </summary>
        private String category = "all";
        public String Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
            }
        }
        #endregion
        /// <summary>
        /// 解释并替换标签内容
        /// </summary>
        /// <param name="temContent">模板页所有内容</param>
        /// <param name="flag">标记此标签的起止的索引的字符串。</param>
        public void ExplainSelf(object obj)
        {
            String[] sArray = (String[])obj;
            String savePath = sArray[0];
            String flag;
            //把程序末尾时要删除的文件路径先保存下来。
            String deletePath = sArray[0];
            //获取指定类别的文件夹的信息：htm文件的数量。
            savePath = PathHelper.GetPathWithoutName(savePath) + "12" + "\\";/////////这个Category必然是List给。全局设置。
            DirectoryInfo dir = new DirectoryInfo(savePath);
            int count = 0;
            foreach (FileInfo fi in dir.GetFiles())
            {
                count++;
                String filePath = savePath + fi.Name;
                //读取模板内容
                String temContent = TemplateHandle.ReadTemplate(filePath);
                flag = TagHandle.FoundTag(temContent, "{wlsd:pagelist", 0)[0];
                //分解起止索引
                int[] flagArray = Array.ConvertAll<String, int>(flag.Split('$'), delegate(string s) { return int.Parse(s); });
                if (count == 1)
                {
                    //解析标签内容
                    AnalyzeTag(temContent, flagArray);
                }
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
                TemplateHandle.SaveTemplate(filePath, temContent);
            }
            //删除模板页
            TemplateHandle.DeleteTemplate(deletePath);
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
        public String AssembleSelf(String temContent, int[] flagArray)
        {
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            String[] itemArray = this.ListItem.Split(',');
            //用于标记是否出现了“上一页”“下一页”，出现了就变为1.
            int flag = 0; 
            for (int i = 0; i < itemArray.Length; i++)
            {
                switch (itemArray[i])
                {
                    case "index":
                        newStr.Append("<li><a href=\"/web/ArticleList/ArticleList.aspx?category="+this.Category+" && pageno=1\">首页</a></li>");
                        break;
                    case "end":
                        newStr.Append("<li><a href=\"/web/ArticleList/ArticleList.aspx?category=" + this.Category + " && pageno=-1\">尾页</a></li>");
                        break;
                    case "pre":
                        newStr.Append("<li><a href=\"#\" onlick=\"redirectUrl(\"pre\")\">上一页</a></li>");
                        flag = 1;
                        break;
                    case "next":
                        newStr.Append("<li><a href=\"#\" onlick=\"redirectUrl(\"next\")\">下一页</a></li>");
                        flag = 1;
                        break;
                    case "info":
                        DirectoryInfo dir = new DirectoryInfo(PathHelper.GetMapPath("/web/ArticleList/" + this.Category));
                        int filesNum = dir.GetFiles().Length;
                        this.ListSize = this.ListSize >= filesNum ? filesNum : this.ListSize;
                        for (int j = 1; j < this.ListSize+1;j++ )
                        {
                            newStr.Append("<li><a href=\"/web/ArticleList/ArticleList.aspx?category=" + this.Category + " && pageno=" + j + "\">" + j + "</a></li>");
                        }
                        break;
                    default:
                        throw new TagErrorException("分页标签的listitem属性设置有错误。");
                        //break;
                }
            }
            if (flag == 1)
            {
                newStr.Append("<script type=\"text/javascript\">function getURLParameter(param,url){var params=(url.substr(url.indexOf(\"?\") + 1)).split(\"&&\");if (params != null){for(var i=0;i<params.length;i++){var strs=params[i].split(\"=\");if(strs[0]==param){return strs[1];}}}}function redirectUrl(cmd){var url = window.loaction.herf;var category = getURLParameter(\"category\",url);var pageno;if(cmd == \"pre\"){pageno = getURLParameter(\"pageno\",url) - 1;}else{pageno =  getURLParameter(\"pageno\",url) + 1;} window.loaction.herf = url.substr(0,url.indexOf(\"?\") + 1) + \"category=\" + category + \"pageno\" + pageno}</script>");
            }
            return newStr.ToString();
        }
    }
}
