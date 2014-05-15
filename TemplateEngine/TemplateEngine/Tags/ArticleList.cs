using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RHClassLibrary;
using RHClassLibrary.Data;
using System.Data;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class ArticleList : TagsAbstractCls
    {
        public ArticleList()
        { }
        #region 属性
        /// <summary>
        /// 文章类别
        /// </summary>
        private String typeid = "all";
        public String TypeID
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
        /// <summary>
        /// 文章列表的行数
        /// </summary>
        private int row = 1;
        public int Row
        {
            get {
                return row;
            }
            set{
                row = value;
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
        /// 排序方式
        /// </summary>
        private String orderby = String.Empty;
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
        /// flag = 'h' 自定义属性值：头条[h]推荐[c]图片[p]幻灯[f]滚动[s]跳转[j]图文[a]加粗[b]
        /// </summary>
        private String flag = String.Empty;
        public String Flag
        {
            get
            {
                return flag;
            }
            set
            {
                flag = value;
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
        /// <summary>
        /// “关于我们”类似列表ID
        /// </summary>
        private int idlist;
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
            //if (flagArray.Length != 4)
            //{
            //    throw new TagErrorException("没有{/wlsd:articlelist}结束标记。");
            //}
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            //存储构造的sql语句
            StringBuilder sqlsb = new StringBuilder();
            sqlsb.Append("SELECT TOP ");
            sqlsb.Append(this.row);
            sqlsb.Append(" * FROM ArticleList ");
            sqlsb.Append(" WHERE 1=1 ");
            if (this.typeid != "all")
            {
                sqlsb.Append("AND CategoryID in ( ");
                sqlsb.Append(this.typeid);
                sqlsb.Append(") ");
            }
            if (this.flag != String.Empty)
            {
                sqlsb.Append("AND Is_ ");
                sqlsb.Append(this.flag);
                sqlsb.Append(" = 1");
            }
            if (this.orderby != String.Empty)
            {
                sqlsb.Append("orderby ");
                sqlsb.Append(this.orderby);
                sqlsb.Append(" ");
                sqlsb.Append(this.orderway);
            }

            DataTable dtArtc = sh.ExcuteDataTable(sqlsb.ToString());
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
                       contentStr= contentStr.Replace("[field:title/]", dr["Title"].ToString().Substring(0,int.Parse(this.titlelen)));
                    }
                    //替换文章图片
                    if (contentStr.Contains("[field:image/]"))
                    {
                        contentStr = contentStr.Replace("[field:image/]", "<image src = \"\" alt = \"" + dr["Title"].ToString() + "\" width = \"" + this.imgwidth + "\" height = \"" + this.imgheight + "\"></image>");
                    }
                    newStr.Append(contentStr);
                }
            }
            return newStr.ToString();
        }
    }
}
