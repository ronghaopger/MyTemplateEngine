using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.Data;
using RHClassLibrary.Data;
using System.Reflection;
using RHClassLibrary;
using System.IO;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class List : ITags
    {
        #region 属性
        /// <summary>
        /// 文章类别
        /// </summary>
        private String category = String.Empty;
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
        /// <summary>
        /// 文章列表每页的条数
        /// </summary>
        private int pagesize = 1;
        public int PageSize
        {
            get
            {
                return pagesize;
            }
            set
            {
                pagesize = value;
            }
        }
        /// <summary>
        /// 依据哪个字段排序
        /// </summary>
        private String orderby = "ID";
        public String OrderBy
        {
            get
            {
                return orderby;
            }
            set
            {
                orderby = value;
            }
        }
        /// <summary>
        /// 标题长度
        /// </summary>
        private String titlelen = "20";
        public String TitleLen
        {
            get
            {
                return titlelen;
            }
            set
            {
                titlelen = value;
            }
        }
        /// <summary>
        /// 升序降序
        /// </summary>
        private String orderway = "desc";
        public String OrderWay
        {
            get
            {
                return orderway;
            }
            set
            {
                orderway = value;
            }
        }
        /// <summary>
        /// 内容类型 默认article文章，image图片
        /// </summary>
        private String listtype = "article";
        public String ListType
        {
            get
            {
                return listtype;
            }
            set
            {
                listtype = value;
            }
        }
        /// <summary>
        /// 缩略图宽度
        /// </summary>
        private int imgwidth;
        public int ImgWith
        {
            get
            {
                return imgwidth;
            }
            set
            {
                imgwidth = value;
            }
        }
        /// <summary>
        /// 缩略图高度
        /// </summary>
        private int imgheight;
        public int ImgHeight
        {
            get
            {
                return imgheight;
            }
            set
            {
                imgheight = value;
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
            String flag = sArray[1];
            //读取模板内容
            String temContent = TemplateHandle.ReadTemplate(savePath);
            if (!temContent.Contains("{wlsd:pagelist"))
            {
                throw new TagErrorException("没有与{wlsd:list}相对应的{wlsd:pagelist}。");
            }
            //分解起止索引
            int[] flagArray = Array.ConvertAll<String, int>(flag.Split('$'), delegate(string s) { return int.Parse(s); });
            if (flagArray.Length != 4)
            {
                throw new TagErrorException("没有{/wlsd:list}结束标记。");
            }
            //解析标签内容
            AnalyzeTag(temContent, flagArray);
            //读取此类别的所有数据，保存到DataTable。
            //存储构造的sql语句
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("SELECT ROW_NUMBER() over(order by ");
            sqlsb.Append(this.OrderBy);
            sqlsb.Append(") as sort,* FROM Article ");
            sqlsb.Append(" WHERE 1=1 ");
            if (this.Category != String.Empty)
            {
                sqlsb.Append("AND CategoryID = ");
                sqlsb.Append(this.Category);
                sqlsb.Append(" order by ");
                sqlsb.Append(this.OrderBy);
                sqlsb.Append(" ");
                sqlsb.Append(this.orderway);
            }
            else
            {
                throw new TagErrorException("没有设置文章的类别属性：category。");
            }
            SqlHelper sh = new SqlHelper();
            DataTable dtArtc = sh.ExcuteDataTable(sqlsb.ToString());
            //页数
            int pagenum = (dtArtc.Rows.Count % this.PageSize) == 0 ? (dtArtc.Rows.Count / this.PageSize) : ((dtArtc.Rows.Count / this.PageSize) + 1);
            for (int i = 1; i <= pagenum; i++)
            {
                DataView dv = new DataView(dtArtc);
                dv.RowFilter = "sort > " + (i-1) * this.PageSize + " AND sort <= " + i * this.PageSize;
                //再读取一遍模板内容。
                temContent = TemplateHandle.ReadTemplate(savePath);
                //组装html，并返回
                String newStr = AssembleSelf(temContent, flagArray, dv.ToTable());
                //用组装的新字符串去替换标签。
                if (flagArray.Length == 4)
                {
                    temContent = temContent.Remove(flagArray[0], flagArray[3] - flagArray[0]).Insert(flagArray[0], newStr);
                }
                else if (flagArray.Length == 2)
                {
                    temContent = temContent.Remove(flagArray[0], flagArray[1] - flagArray[0]).Insert(flagArray[0], newStr);
                }
                String listPath = PathHelper.GetPathWithoutName(savePath) + this.Category + "\\" + i + ".htm";
                if (!File.Exists(listPath))
                {
                    File.Create(listPath).Close();
                }
                //先把模板复制到最终静态页要保存的地方。
                File.Copy(savePath, listPath, true);
                //保存模板
                TemplateHandle.SaveTemplate(listPath, temContent);
            }
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
        public String AssembleSelf(String temContent, int[] flagArray, DataTable dtArtc)
        {
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            if (dtArtc != null)
            {
                foreach (DataRow dr in dtArtc.Rows)
                {
                    String contentStr = temContent.Substring(flagArray[1] + 1, flagArray[2] - flagArray[1] - 1);
                    //替换文章地址
                    if (contentStr.Contains("[field:arcurl/]"))
                    {
                        contentStr = contentStr.Replace("[field:arcurl/]", dr["ContentUrl"].ToString());
                    }
                    //替换文章标题
                    if (contentStr.Contains("[field:title/]"))
                    {
                        contentStr = contentStr.Replace("[field:title/]", dr["Title"].ToString());
                    }
                    //替换文章图片
                    if (contentStr.Contains("[field:image/]"))
                    {
                        contentStr = contentStr.Replace("[field:image/]", "<image src = \"\" width = \"" + this.imgwidth + "\" height = \"" + this.imgheight + "\"></image>");
                    }
                    newStr.Append(contentStr);
                }
            }
            return newStr.ToString();
        }
    }
}
