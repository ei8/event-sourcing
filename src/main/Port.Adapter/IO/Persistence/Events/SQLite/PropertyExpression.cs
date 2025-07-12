using System;
using System.Linq.Expressions;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    // TODO:0 transfer to an appropriate namespace
    public class PropertyExpression<T, TValue>
    {
        public PropertyExpression(Expression<Func<T, TValue>> property)
        {
            this.Getter = ExpressionUtils.CreateGetter(property);
            this.Setter = ExpressionUtils.CreateSetter(property);
        }

        public Func<T, TValue> Getter { get; set; }
        public Action<T, TValue> Setter { get; set; }
    }
}
