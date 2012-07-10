using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OpenUO.Core.Data;

namespace OpenUO.Core.PresentationFramework.ComponentModel.Design
{
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");

            PropertyInfo property = memberExpression.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");

            MethodInfo getMethod = property.GetGetMethod(true);

            if (getMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");

            return memberExpression.Member.Name;
        }

        public static bool TryGetColumnName<T>(Expression<Func<T>> propertyExpression, out string columnName)
        {
            columnName = string.Empty;

            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");

            PropertyInfo property = memberExpression.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");

            ColumnAttribute[] columnAttributes = Attribute.GetCustomAttributes(property, true).Where(c => c is ColumnAttribute).Cast<ColumnAttribute>().ToArray();

            if (columnAttributes.Length == 0)
                return false;

            columnName = columnAttributes[0].Name;

            return true;
        }
    }
}
