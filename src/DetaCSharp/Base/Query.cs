using DetaCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DetaCSharp.Base
{
    public class Query
    {
        JsonNamingPolicyDictionary queries;
        internal Query(JsonNamingPolicy propertyNamingPolicy)
        {
            queries = new JsonNamingPolicyDictionary(propertyNamingPolicy);
        }

        public IDictionary<string, object> Queries => queries;


        public Query WithEqual(string key, object value) => InnerWith(key, QueryOperation.Equal, value);
        public Query WithNotEqual(string key, object value) => InnerWith(key, QueryOperation.NotEqual, value);
        public Query WithLessThan(string key, object value) => InnerWith(key, QueryOperation.LessThan, value);
        public Query WithGreaterThen(string key, object value) => InnerWith(key, QueryOperation.GreaterThan, value);
        public Query WithLessThanOrEqual(string key, object value) => InnerWith(key, QueryOperation.LessThanOrEqual, value);
        public Query WithGreaterThanOrEqual(string key, object value) => InnerWith(key, QueryOperation.GreaterThanOrEqual, value);
        public Query WithPrefix(string key, object value) => InnerWith(key, QueryOperation.Prefix, value);
        public Query WithRange(string key, object value1, object value2)
        {
            queries[$"{key}{QueryOperation.Range}"] = new[] { value1, value2 };
            return this;
        }
        public Query WithContains(string key, string value) => InnerWith(key, QueryOperation.Contains, value);
        public Query WithNotContains(string key, string value) => InnerWith(key, QueryOperation.NotContains, value);


        public Query WithEqual<T>(string key, object value) => InnerWith(key, QueryOperation.Equal, value);
        public Query WithNotEqual<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.NotEqual, value);
        public Query WithLessThan<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.LessThan, value);
        public Query WithGreaterThen<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.GreaterThan, value);
        public Query WithLessThanOrEqual<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.LessThanOrEqual, value);
        public Query WithGreaterThanOrEqual<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.GreaterThanOrEqual, value);
        public Query WithPrefix<T>(Expression<Func<T, object>> propertyExpression, object value) => InnerWith<T>(propertyExpression, QueryOperation.Prefix, value);
        public Query WithRange<T>(Expression<Func<T, object>> propertyExpression, object value1, object value2)
        {
            queries[$"{GetPropertyName(propertyExpression)}{QueryOperation.Range}"] = new[] { value1, value2 };
            return this;
        }
        public Query WithContains<T>(Expression<Func<T, object>> propertyExpression, string value) => InnerWith<T>(propertyExpression, QueryOperation.Contains, value);
        public Query WithNotContains<T>(Expression<Func<T, object>> propertyExpression, string value) => InnerWith<T>(propertyExpression, QueryOperation.NotContains, value);



        Query InnerWith(string key, string operation, object value)
        {
            queries[$"{key}{operation}"] = value;
            return this;
        }

        Query InnerWith<T>(Expression<Func<T, object>> propertyExpression, string operation, object value)
        {
            queries[$"{GetPropertyName(propertyExpression)}{operation}"] = value;
            return this;
        }

        static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression) => ((MemberExpression)propertyExpression.Body).Member.Name;
    }
}
