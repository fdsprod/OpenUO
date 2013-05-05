using System;
using System.Linq.Expressions;

namespace OpenUO.Core.PresentationFramework.ComponentModel.Design
{
    public abstract class PropertyChangedNotifierBase : NotifiableBase
    {
        protected virtual bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            return SetProperty(ref storage, value, propertyName);
        }

        protected void RaisePropertyChanged(params Expression<Func<object>>[] propertyExpressions)
        {
            for (int i = 0; i < propertyExpressions.Length; i++)
            {
                Expression<Func<object>> propertyExpression = propertyExpressions[i];
                OnPropertyChanged(PropertySupport.ExtractPropertyName(propertyExpression));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                OnPropertyChanged(propertyNames[i]);
            }
        }
    }
}