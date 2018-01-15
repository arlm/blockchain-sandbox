using System;
using System.Reflection;
using CommandLine;

namespace BlockChain.CLI.Verbs
{
    public static class CommandLineExtensions
    {
        public static (PropertyInfo, TAttribute) RetrieveOptionProperty<TAttribute>(this object target, string uniqueName)
                where TAttribute : BaseOptionAttribute
        {
            var key = new Tuple<Type, object>(typeof(Tuple<PropertyInfo, BaseOptionAttribute>), target);

            if (target == null)
            {
                return (null, null);
            }

            var propertiesInfo = target.GetType().GetProperties();

            foreach (var property in propertiesInfo)
            {
                if (property == null || (!property.CanRead || !property.CanWrite))
                {
                    continue;
                }

                var setMethod = property.GetSetMethod();
                if (setMethod == null || setMethod.IsStatic)
                {
                    continue;
                }

                var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                var optionAttr = (TAttribute)attribute;
                if (optionAttr == null || string.CompareOrdinal(uniqueName, optionAttr.LongName) != 0)
                {
                    continue;
                }

                var found = new Tuple<PropertyInfo, TAttribute>(property, (TAttribute)attribute);

                return (found.Item1, found.Item2);
            }

            return (null, null);
        }
    }
}
