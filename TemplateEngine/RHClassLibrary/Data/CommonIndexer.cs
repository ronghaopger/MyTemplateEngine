using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RHClassLibrary.Data
{
    public class CommonIndexer
    {
        public string this[string name]
        {
            set
            {
                PropertyInfo pi = this.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (pi != null && !string.IsNullOrEmpty(value))
                {
                    Type propertyType = pi.PropertyType;
                    if (propertyType.IsGenericType)     //如果此属性是泛型（这里判断原因是防止有类似int?的类型）
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }
                    pi.SetValue(this, Convert.ChangeType(value, propertyType), null);
                }
            }
            get
            {
                PropertyInfo pi = this.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (pi != null)
                {
                    Type Ts = this.GetType();
                    object o = Ts.GetProperty(name).GetValue(this, null);
                    return Convert.ToString(o);
                }
                return null;
            }
        }
    }
}
