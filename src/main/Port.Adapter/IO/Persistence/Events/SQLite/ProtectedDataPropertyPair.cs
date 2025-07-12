using System;
using System.Linq.Expressions;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    // TODO:0 transfer to an appropriate namespace
    public class ProtectedDataPropertyPair<T, TData>
    {
        public ProtectedDataPropertyPair(
            Expression<Func<T, TData>> dataProperty,
            Expression<Func<T, bool>> isDataProtectedProperty
        )
        {
            this.DataProperty = new PropertyExpression<T, TData>(dataProperty);
            this.IsDataProtectedProperty = new PropertyExpression<T, bool>(isDataProtectedProperty);
        }

        public PropertyExpression<T, TData> DataProperty { get; set; }
         
        public PropertyExpression<T, bool> IsDataProtectedProperty { get; set; }
    }
}
