///作者：荣浩
///创建于：2013.8.28
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace RHClassLibrary
{
    public class StringHelper
    {
        /// <summary>
        /// 判断指定字符串的指定索引处开始，除去空格后第一个字符是指定字符。
        /// </summary>
        /// <param name="aimStr">指定字符串</param>
        /// <param name="startIndex">指定所引处</param>
        /// <param name="aimChar">指定字符</param>
        /// <returns></returns>
        public static bool judgeNextChar(String aimStr,int startIndex,String aimChar)
        { 
            aimStr = aimStr.Substring(startIndex);
            int aimIndex = aimStr.IndexOf(aimChar);
            if (aimIndex == -1)
                return false;
            if (aimStr.Substring(0, aimIndex).Trim() == String.Empty)
                return true;
            else
                return false;
        }
        #region 对字符串的加密/解密

        /// <summary>
        /// 对传递的参数字符串进行处理，防止注入式攻击
        /// </summary>
        /// <param name="str">传递的参数字符串</param>
        /// <returns>String</returns>
        public static string TranslateText(string str)
        {
            str = str.Trim();
            str = str.Replace("'", "''");
            str = str.Replace(";--", "");
            str = str.Replace("=", "");
            str = str.Replace(" or ", "");
            str = str.Replace(" and ", "");

            return str;
        }

        /// <summary>
        /// 对传递的参数字符串以及表单数据进行检查，防止注入式攻击 
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>String</returns>
        public static int CheckUp(string url)
        {
            int post1, post2, post3, post4, post5, post6, post7, post8, post9;
            post1 = instr(url, "%", true);
            post2 = instr(url, "'", true);
            post3 = instr(url, ";", true);
            post4 = instr(url, "where", true);
            post5 = instr(url, "select", true);
            post6 = instr(url, "/", true);
            post7 = instr(url, "and", true);
            post8 = instr(url, "or", true);
            post9 = instr(url, "=", true);
            if (post1 == 1 || post2 == 1 || post3 == 1 || post4 == 1 || post5 == 1 || post6 == 1 || post7 == 1 || post8 == 1 || post9 == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 判断指定字符串中是否含有指定字符（串） 
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符(串)</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static int instr(string str, string stringarray, bool caseInsensetive)
        {
            if (caseInsensetive)
            {
                if (str.ToLower().Contains(stringarray.ToLower()))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (str.Contains(stringarray))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 对字符串进行加密，本方法是通过配置文件中的EncryptMethod字段设置方法对登陆进行加密
        /// </summary>
        /// <param name="Password">要加密的字符串</param>
        /// <param name="Format">加密方式,-1 No Encrypt, 0 is SHA1,1 is MD5</param>
        /// <returns></returns>
        public static string NoneEncrypt(string Password, int Format)
        {
            string strResult = "";
            switch (Format)
            {
                case 0:
                    strResult = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "SHA1");
                    break;
                case 1:
                    strResult = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5");
                    break;
                default:
                    strResult = TranslateText(Password);
                    break;
            }

            return strResult;
        }


        /// <summary>
        /// 对字符串进行加密
        /// </summary>
        /// <param name="Passowrd">待加密的字符串</param>
        /// <returns>string</returns>
        public static string Encrypt(string Passowrd)
        {
            string strResult = "";

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(Passowrd, true, 2);
            strResult = FormsAuthentication.Encrypt(ticket).ToString();

            return strResult;
        }


        /// <summary>
        /// 对字符串进行解密
        /// </summary>
        /// <param name="Passowrd">已加密的字符串</param>
        /// <returns></returns>
        public static string Decrypt(string Passowrd)
        {
            string strResult = "";

            strResult = FormsAuthentication.Decrypt(Passowrd).Name.ToString();

            return strResult;
        }

        #endregion
    }
}
