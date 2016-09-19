using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetCoreLibrary.Core.Common.Utilities
{
    public class SimpleObjectMapper<TSource, TDestination> where TSource : new() where TDestination : new()
    {
        private readonly IDictionary<string, string> _propertiesMap = new Dictionary<string, string>(); 

        public SimpleObjectMapper<TSource, TDestination> MapProperty(
        Expression<Func<TSource, object>> source, Expression<Func<TDestination, object>> destination)
        {
            string sourcePropertyName = GetSourceMemberInfo(source);
            string destinationPropertyName = GetDestinationMemberInfo(destination);
            _propertiesMap.Add(sourcePropertyName, destinationPropertyName);

            return this; 
        }

        /// Code borrowed from https://github.com/TinyMapper/TinyMapper/blob/master/Source/TinyMapper/Bindings/BindingConfigOf.cs
        ///
        private static string GetSourceMemberInfo(Expression<Func<TSource, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }

                if (member == null)
                {
                    throw new ArgumentException("Expression is not a MemberExpression", nameof(expression));
                }
            }
            return member.Member.Name;
        }

        /// Code borrowed from https://github.com/TinyMapper/TinyMapper/blob/master/Source/TinyMapper/Bindings/BindingConfigOf.cs
        ///
        private static string GetDestinationMemberInfo(Expression<Func<TDestination, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }

                if (member == null)
                {
                    throw new ArgumentException("Expression is not a MemberExpression", nameof(expression));
                }
            }
            return member.Member.Name;
        }



        public TDestination Map(TSource source)
        {
            var destination = new TDestination();
            if (source == null) return destination;

            //Get all properties from the source type
            var sourceTypeProperties = source.GetType().GetProperties().Where(p => p.CanWrite).ToList();
            //Get all properties from the destination type
            var destinationTypeProperties = destination.GetType().GetProperties().Where(p => p.CanWrite).ToList();

            //Loop through all the properties of the source type and populate the target type for all matching properties.
            foreach (var prop in sourceTypeProperties)
            {
                PropertyInfo propertyInDestinationType = destinationTypeProperties.SingleOrDefault(p => p.Name == prop.Name);
                if (propertyInDestinationType == null) continue;
                object propValue = prop.GetValue(source);
                propertyInDestinationType.SetValue(destination, propValue);
            }

            //Now map extra fields which were specified manually using the MapProperty method
            foreach (KeyValuePair<string, string> entry in _propertiesMap)
            {
                PropertyInfo propertyInSourceType = sourceTypeProperties.SingleOrDefault(p => p.Name == entry.Key);
                object sourceObjectPropertyValue = propertyInSourceType.GetValue(source);
                PropertyInfo propertyInDestinationType = destinationTypeProperties.SingleOrDefault(p => p.Name == entry.Value);

                propertyInDestinationType.SetValue(destination, sourceObjectPropertyValue);
            }

            return destination;
        }

    }
}
