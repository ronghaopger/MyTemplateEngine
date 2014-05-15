using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemplateEngine.RHClassLibrary.Exception;
using System.Data;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class Sql : TagsAbstractCls
    {
        #region 属性
        private String sqlstr = String.Empty;
        public String SqlStr
        {
            get
            {
                return sqlstr;
            }
            set
            {
                sqlstr = value;
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
                throw new TagErrorException("没有{/wlsd:sql}结束标记。");
            }
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            DataRow drArtc = sh.ExcuteDataRow(this.sqlstr);
            if (drArtc != null)
            {
                String contentStr = temContent.Substring(flagArray[1] + 1, flagArray[2] - flagArray[1] - 1);
                //替换各个字段
                if (contentStr.Contains("[field:title/]"))
                {
                    contentStr = contentStr.Replace("[field:title/]", drArtc["Title"].ToString());
                }
                if (contentStr.Contains("[field:content/]"))
                {
                    contentStr = contentStr.Replace("[field:content/]", drArtc["Content"].ToString());
                }
                newStr.Append(contentStr);
            }
            return newStr.ToString();
        }
    }
}
