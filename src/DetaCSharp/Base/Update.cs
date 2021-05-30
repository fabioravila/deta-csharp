
using DetaCSharp.Types;
using DetaCSharp.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace DetaCSharp.Base
{
    public class Update
    {
        internal Update(JsonNamingPolicy namePolicy)
        {
            Append = new JsonNamingPolicyDictionary(namePolicy);
            Delete = new JsonNamingPolicyCollection(namePolicy);
            Increment = new JsonNamingPolicyDictionary(namePolicy);
            Prepend = new JsonNamingPolicyDictionary(namePolicy);
            Set = new JsonNamingPolicyDictionary(namePolicy);
        }
        public IDictionary<string, object> Set { get; }
        public IDictionary<string, object> Increment { get; }
        public IDictionary<string, object> Append { get; }
        public IDictionary<string, object> Prepend { get; }
        public ICollection<string> Delete { get; }
        public IDictionary<string, object> this[ActionTypes action]
        {
            get
            {
                switch (action)
                {
                    case ActionTypes.Set: return Set;
                    case ActionTypes.Increment: return Increment;
                    case ActionTypes.Append: return Append;
                    case ActionTypes.Prepend: return Prepend;
                    case ActionTypes.Trim:
                    default:
                        throw new IndexOutOfRangeException(action.ToString());
                }
            }
        }


        public Update With<TEntity>(ActionTypes action, Expression<Func<TEntity, object>> propertyExpression, object value = null)
            => InnerWith(action, propertyExpression, value);
        public Update With(ActionTypes action, string key, object value = null) => InnerWith(action, key, value);
        public Update WithSet(string key, object value) => InnerWith(ActionTypes.Set, key, value);
        public Update WithSet<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
            => InnerWith(ActionTypes.Set, propertyExpression, value);
        public Update WithIncrement(string key, object value) => InnerWith(ActionTypes.Increment, key, value);
        public Update WithIncrement<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
         => InnerWith(ActionTypes.Increment, propertyExpression, value);
        public Update WithAppend(string key, object value) => InnerWith(ActionTypes.Append, key, value);
        public Update WithAppend<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
           => InnerWith(ActionTypes.Append, propertyExpression, value);
        public Update WithPrepend(string key, object value) => InnerWith(ActionTypes.Prepend, key, value);
        public Update WithPrepend<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
           => InnerWith(ActionTypes.Prepend, propertyExpression, value);
        public Update WithDelete(string key) => InnerWith(ActionTypes.Trim, key, null);
        public Update WithDelete<TEntity>(Expression<Func<TEntity, object>> propertyExpression)
            => InnerWith(ActionTypes.Trim, propertyExpression, null);

        Update InnerWith<TEntity>(ActionTypes action, Expression<Func<TEntity, object>> propertyExpression, object value = null)
            => InnerWith(action, GetPropertyName(propertyExpression), value);
        Update InnerWith(ActionTypes action, string key, object value = null)
        {
            if (action == ActionTypes.Trim)
            {
                Delete.Add(key);
            }
            else if (action == ActionTypes.Prepend || action == ActionTypes.Append)
            {
                //Keep append and ans preppend sequenciallia use
                if (!this[action].ContainsKey(key))
                {
                    this[action][key] = new List<object>();
                }

                var list = (List<object>)this[action][key];


                if (value is IEnumerable)
                {
                    list.AddRange((IEnumerable<object>)value);
                }
                else
                {
                    list.Add(value);
                }
            }
            else
            {
                this[action][key] = value;
            }

            return this;
        }

        static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression) => ((MemberExpression)propertyExpression.Body).Member.Name;
    }
}
