using Autofac.Core;
using System.Reflection;

namespace NPlatform
{
    /// <summary>
    /// 自动注入属性对象。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Autowired: Attribute
    {
    }
    /// <summary>
    /// 自动注入属性选择。
    /// </summary>
    public class AutowiredSelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            //需要一个判断的维度；
            return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(Autowired));

        }
    }
}
