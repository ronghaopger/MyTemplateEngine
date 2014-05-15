using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.Data;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class Article : TagsAbstractCls
    {
        public Article()
        { }
        #region 属性
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
        /// 内容长度
        /// </summary>
        private String infolen = "200";
        public String InfoLen
        {
            get
            {
                return infolen;
            }
            set
            {
                infolen = value;
            }
        }
        /// <summary>
        /// “关于我们”类似列表ID
        /// </summary>
        private int idlist = -1;
        public int IDList
        {
            get
            {
                return idlist;
            }
            set
            {
                idlist = value;
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
            if (flagArray.Length != 4)
            {
                throw new TagErrorException("没有{/wlsd:article}结束标记。");
            }
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            //存储构造的sql语句
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("SELECT * FROM Article ");
            sqlsb.Append(" WHERE 1=1 ");
            if (this.idlist != -1)
            {
                sqlsb.Append("AND ID = ");
                sqlsb.Append(this.idlist);
            }

            DataRow drArtc = sh.ExcuteDataRow(sqlsb.ToString());
            if (drArtc != null)
            {
                    String contentStr = temContent.Substring(flagArray[1] + 1, flagArray[2] - flagArray[1] - 1);
                    //替换文章标题
                    if (contentStr.Contains("[field:title/]"))
                    {
                        contentStr = contentStr.Replace("[field:title/]", drArtc["Title"].ToString().Substring(0, int.Parse(this.titlelen)));
                    }
                    //替换文章内容
                    if (contentStr.Contains("[field:description/]"))
                    {
                        contentStr = contentStr.Replace("[field:description/]", drArtc["Content"].ToString().Substring(0, int.Parse(this.infolen)));
                    }
                    newStr.Append(contentStr);
            }
            return newStr.ToString();
        }
    }
}
