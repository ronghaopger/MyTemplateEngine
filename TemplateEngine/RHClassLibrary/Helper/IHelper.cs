using System;
using System.Collections.Generic;
using System.Text;


namespace RHClassLibrary
{
    /// <summary>
    /// 助手类接口
    /// </summary>
    public interface IHelper
    {
        string Name { get; set; }

        string Description { get; set; }

        string Root { get; set; }
    }
}
