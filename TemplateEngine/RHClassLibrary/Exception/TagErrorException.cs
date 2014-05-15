using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemplateEngine.RHClassLibrary.Exception
{
    [Serializable] //声明为可序列化的 因为要写入文件中  
    public class TagErrorException : System.Exception
    {
         /// <summary>  
        /// 默认构造函数  
        /// </summary>  
        public TagErrorException() { }  
        public TagErrorException(string message) : base(message) 
        { }
        public TagErrorException(string message, System.Exception inner) : base(message, inner) 
        { }  
    }
}
