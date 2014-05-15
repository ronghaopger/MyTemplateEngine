using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TemplateEngine.RHClassLibrary.Exception;

namespace TemplateEngine.TemplateEngine.Tags
{
    public class ChannelArtList : TagsAbstractCls
    {
        #region 属性
        /// <summary>
        /// 重复的次数
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
                throw new TagErrorException("没有{/wlsd:channelarclist}结束标记。");
            }
            //存储构造出来的html
            StringBuilder newStr = new StringBuilder();
            for (int i = 0; i < this.row; i++)
            {
                String channelContent = temContent.Substring(flagArray[1], flagArray[2] - flagArray[1]);
                String[] foundArray = TagHandle.FoundTag(channelContent, "{wlsd:articlelist", 0);
                channelContent = temContent.Substring(flagArray[1], flagArray[2] - flagArray[1]);
                //分解起止索引
                int[] channelflagArray = Array.ConvertAll<String, int>(foundArray[1].Split('$'), delegate(string s) { return int.Parse(s); });
                //解析标签内容
                ArticleList al = new ArticleList();
                al.AnalyzeTag(channelContent, channelflagArray);
                String channelStr = al.AssembleSelf(channelContent, channelflagArray);
                //用组装的新字符串去替换标签。
                if (channelflagArray.Length == 4)
                {
                    channelContent = channelContent.Remove(channelflagArray[0], channelflagArray[3] - channelflagArray[0]).Insert(channelflagArray[0], channelStr);
                }
                else if (channelflagArray.Length == 2)
                {
                    channelContent = channelContent.Remove(channelflagArray[0], channelflagArray[1] - channelflagArray[0]).Insert(channelflagArray[0], channelStr);
                }
                ////TODO:可以继续解析下一个标签。
                newStr.Append(channelContent);
            }
            return newStr.ToString();
        }
    }
}
