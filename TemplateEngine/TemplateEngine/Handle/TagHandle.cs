using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using TemplateEngine.TemplateEngine.Tags;
using System.IO;
using System.Web;
using RHClassLibrary;
using TemplateEngine.RHClassLibrary.Exception;

namespace TemplateEngine.TemplateEngine.Handle
{
    public class TagHandle
    {
        public static void ExplainTemplate(String templatePath,String savePath)
        {
            savePath = PathHelper.GetMapPath(savePath + PathHelper.GetFileName(templatePath));
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Close();
            }
            //先把模板复制到最终静态页要保存的地方。
            File.Copy(templatePath, savePath, true);
            Assembly asb = Assembly.Load("TemplateEngine.TemplateEngine");
            Type[] tArray = asb.GetTypes();
            //把List和PageList类放到数组的最后两位。
            for (int i = 0; i < tArray.Length; i++)
            {
                if (tArray[i].Name == "List" && i!=tArray.Length-2)
                {
                    Type tSwitch = tArray[tArray.Length - 2];
                    tArray[tArray.Length - 2] = tArray[i];
                    tArray[i] = tSwitch;
                    //保证每个类型都被扫描到。
                    i--;
                    continue;
                }
                if (tArray[i].Name == "PageList" && i != tArray.Length - 1)
                {
                    Type tSwitch = tArray[tArray.Length - 1];
                    tArray[tArray.Length - 1] = tArray[i];
                    tArray[i] = tSwitch;
                    //保证每个类型都被扫描到。
                    i--;
                    continue;
                }
            }
            //循环解释每个标签
            foreach (Type t in tArray)
            {
                if (t.Namespace == "TemplateEngine.TemplateEngine.Tags")
                {
                    //记录循环次数
                    int count = 0;
                    String[] foundArray = new String[2] { String.Empty, "0" };
                    String temContent = String.Empty;
                    while (true)
                    {
                        temContent = TemplateHandle.ReadTemplate(savePath);
                        foundArray = FoundTag(temContent, "{wlsd:" + t.Name.ToLower(), int.Parse(foundArray[1]));
                        if (int.Parse(foundArray[1]) == -1 || (count != 0 && int.Parse(foundArray[1]) == 0 && foundArray[0] == String.Empty))
                        {
                            break;
                        }
                        MethodInfo mi = t.GetMethod("ExplainSelf");
                        object oo = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new Object[0]);
                        mi.Invoke(oo, new object[] { new String[] { savePath, foundArray[0] } });
                        count++;
                        //默认列表页中list标签只出现一次。
                        if (t.Name == "List" && count > 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 寻找指定的标签在指定字符串中的索引
        /// </summary>
        /// <param name="temContent">指定字符串</param>
        /// <param name="tagName">指定的标签</param>
        /// <param name="index">开始寻找的位置的索引</param>
        /// <returns></returns>
        internal static String[] FoundTag(String temContent, String tagName,int index)
        {
            String[] returnArray = new String[2];
            //用于记录标签的起始和终止位置的索引。用$隔开。
            String flag;
            flag = String.Empty;
            index = temContent.IndexOf(tagName, index);
            if (index == -1)
            {
                returnArray[1] = "-1";
                return returnArray;
            }
            int temporaryIndex1 = temContent.IndexOf("/}", index); //临时变量1
            int temporaryIndex2 = temContent.IndexOf("}", index); //临时变量2
            //如果存在"/}"，并且"/}"之前没有再次出现"{wlsd:"以及"{/wlsd:"。
            if (temporaryIndex1 != -1 && (temContent.IndexOf("{wlsd:", index + 5) == -1 || temContent.IndexOf("{wlsd:", index + 5) > temporaryIndex1) && (temContent.IndexOf("{/wlsd:", index) == -1 || temContent.IndexOf("{/wlsd:", index) > temporaryIndex1))
            {
                flag += index.ToString();
                returnArray[1] = index.ToString();
                index = temContent.IndexOf("/}", index) + 2;
                flag += "$" + index;
                returnArray[0] = flag;
                return returnArray;
            }
            //如果存在"}"，并且"}"之前没有再次出现"{wlsd:"以及"{/wlsd:"。
            else if (temporaryIndex2 != -1 && (temContent.IndexOf("{wlsd:", index + 5) == -1 || temContent.IndexOf("{wlsd:", index + 5) > temporaryIndex2) && (temContent.IndexOf("{/wlsd:", index) == -1 || temContent.IndexOf("{/wlsd:", index) > temporaryIndex2))
            {
                flag += index.ToString();
                returnArray[1] = index.ToString();
                index = temContent.IndexOf("}", index) + 1;
                flag += "$" + index;
                String tagEnd = "{/" + tagName.Substring(1, tagName.Length - 1) + "}";
                int temporaryIndex3 = temContent.IndexOf(tagEnd, index); //临时变量3
                //如果存在结束标签，并且结束标签之前没有再次出现"{wlsd:"。
                if (temporaryIndex3 != -1 && (temContent.IndexOf("{wlsd:", index + 5)==-1 || temContent.IndexOf("{wlsd:", index + 5) > temporaryIndex3))
                {
                    index = temContent.IndexOf(tagEnd, index);
                    flag += "$" + index;
                    index = index + tagEnd.Length + 1;
                    flag += "$" + index;
                    returnArray[0] = flag;
                    return returnArray;
                }
                else
                {
                    throw new TagErrorException("没有正规的标签结束符：'{/...}'");
                }
            }
            else
            {
                throw new TagErrorException("没有正规的标签结束符：'/}' or '} {/...}'");
            }
        }
    }
}
