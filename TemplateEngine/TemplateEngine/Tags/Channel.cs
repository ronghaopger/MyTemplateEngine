using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RHClassLibrary.Data;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class Channel : TagsAbstractCls
    {
        #region 属性
        /// <summary>
        /// 导航数量
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
        /// 栏目级别，top为一级，son为二级
        /// </summary>
        private String type = "top";
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
        /// 若为son（二级栏目），设置其所属一级栏目的ID
        /// </summary>
        private int typeid = 0;
        public int TypeID
        {
            get
            {
                return typeid;
            }
            set
            {
                typeid = value;
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
            sqlsb.Append(this.row);
            sqlsb.Append(" * FROM Channel ");
            if (this.type == "son")
            {
                sqlsb.Append("WHERE ParentID = ");
                sqlsb.Append(this.typeid);
            }
            else if (this.type == "top")
            {
                sqlsb.Append("WHERE ParentID = 0 ");
            }
            sqlsb.Append(" ORDER BY OrderNumber ASC ");
            DataTable dtChannel = sh.ExcuteDataTable(sqlsb.ToString());
            if (dtChannel != null)
            {
                foreach (DataRow dr in dtChannel.Rows)
                {
                    String contentStr = temContent.Substring(flagArray[1] + 1, flagArray[2] - flagArray[1] - 1);
                    //替换导航地址
                    if (contentStr.Contains("[field:typelink/]"))
                    {
                        contentStr = contentStr.Replace("[field:typelink/]", dr["FullUrl"].ToString());
                    }
                    //替换导航名称
                    if (contentStr.Contains("[field:typename/]"))
                    {
                        contentStr = contentStr.Replace("[field:typename/]", dr["Title"].ToString());
                    }
                    newStr.Append(contentStr);
                }
            }
            return newStr.ToString();
        }
    }
}
