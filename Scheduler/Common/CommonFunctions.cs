using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Scheduler
{
   public class CommonFunctions
    {
        public static Func<TType, TProperty> GetPropertyValue<TType, TProperty>(string propertyName)
        {
            var sourceObjectParam = Expression.Parameter(typeof(TType));
            var prop = Expression.Property(sourceObjectParam, propertyName);
            return Expression.Lambda<Func<TType, TProperty>>(prop, sourceObjectParam).Compile();
        }
    }
}
