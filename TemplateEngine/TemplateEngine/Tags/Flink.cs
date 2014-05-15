using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.Reflection;
using RHClassLibrary;
using System.Data;
using RHClassLibrary.Data;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class Flink : TagsAbstractCls
    {
        #region 属性
        /// <summary>
        /// 友情链接数量
        /// </summary>
        private int row = 1;
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }
        /// <summary>
        /// 链接类型 text文字 image图片
        /// </summary>
        private String type = "text";
        public String Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        /// <summary>
        /// 链接文字的长度
        /// </summary>
        private int titlelen = 10;
        public int TitleLen
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
        /// 链接位置 1为首页 2为内页
        /// </summary>
        private int linktype = 1;
        public int LinkType
        {
            get
            {
                return linktype;
            }
            set
            {
                linktype = value;
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
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            //存储构造的sql语句
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("SELECT TOP ");
            sqlsb.Append(this.Row);
            sqlsb.Append(" * FROM Link ");
            sqlsb.Append("ORDER BY OrderNumber ASC ");
            sqlsb.Append("WHERE LinkType = ");
            sqlsb.Append(this.linktype);
            DataTable dtLink = sh.ExcuteDataTable(sqlsb.ToString());
            if (dtLink != null)
            {
                foreach (DataRow dr in dtLink.Rows)
                {
                    if (this.type == "text")
                    {
                        String contentStr = "<li><a href='{0}'>{1}</a></li>";
                        newStr.Append(String.Format(contentStr, dr["Url"], dr["Title"].ToString().Substring(0, this.titlelen)));
                    }
                    else if (this.type == "iamge")
                    {
                        String contentStr = "<li><a href='{0}'><image src = \"{1}\" alt = \"{2}\"></image></a></li>";
                        newStr.Append(String.Format(contentStr, dr["Url"], dr["ImageUrl"], dr["Title"].ToString().Substring(0, this.titlelen)));
                    }
                }
            }
            return newStr.ToString();
        }
    }
}
