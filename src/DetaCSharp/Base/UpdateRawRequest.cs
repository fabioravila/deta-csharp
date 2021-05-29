
using DetaCSharp.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace DetaCSharp.Base
{
    public class UpdateRawRequest
    {
        readonly JsonNamingPolicy namePolicy;

        internal UpdateRawRequest(JsonNamingPolicy namePolicy)
        {
            Append = new KeyNamePolicyDictionary(namePolicy);
            Delete = new List<string>();
            Increment = new KeyNamePolicyDictionary(namePolicy);
            Prepend = new KeyNamePolicyDictionary(namePolicy);
            Set = new KeyNamePolicyDictionary(namePolicy);
            this.namePolicy = namePolicy;
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


        public UpdateRawRequest With<TEntity>(ActionTypes action, Expression<Func<TEntity, object>> propertyExpression, object value = null)
            => WithInner(action, propertyExpression, value);

        public UpdateRawRequest With(ActionTypes action, string key, object value = null) => WithInner(action, key, value);

        public UpdateRawRequest WithSet(string key, object value) => WithInner(ActionTypes.Set, key, value);
        public UpdateRawRequest WithSet<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
            => WithInner(ActionTypes.Set, propertyExpression, value);

        public UpdateRawRequest WithIncrement(string key, object value) => WithInner(ActionTypes.Increment, key, value);

        public UpdateRawRequest WithIncrement<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
         => WithInner(ActionTypes.Increment, propertyExpression, value);

        public UpdateRawRequest WithAppend(string key, object value) => WithInner(ActionTypes.Append, key, value);

        public UpdateRawRequest WithAppend<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
           => WithInner(ActionTypes.Append, propertyExpression, value);

        public UpdateRawRequest WithPrepend(string key, object value) => WithInner(ActionTypes.Prepend, key, value);


        public UpdateRawRequest WithPrepend<TEntity>(Expression<Func<TEntity, object>> propertyExpression, object value)
           => WithInner(ActionTypes.Prepend, propertyExpression, value);

        public UpdateRawRequest WithDelete(string key) => WithInner(ActionTypes.Trim, key, null);

        public UpdateRawRequest WithDelete<TEntity>(Expression<Func<TEntity, object>> propertyExpression)
            => WithInner(ActionTypes.Trim, propertyExpression, null);



        UpdateRawRequest WithInner<TEntity>(ActionTypes action, Expression<Func<TEntity, object>> propertyExpression, object value = null)
        {
            var prop = GetPropertyName(propertyExpression);

            if (action == ActionTypes.Trim)
            {
                Delete.Add(prop);
            }
            else
            {
                this[action][prop] = value;
            }


            return this;
        }

        UpdateRawRequest WithInner(ActionTypes action, string key, object value = null)
        {
            if (action == ActionTypes.Trim)
            {
                Delete.Add(key);
            }
            else
            {
                this[action][key] = value;
            }

            return this;
        }

        string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression) => ((MemberExpression)propertyExpression.Body).Member.Name;


        class KeyNamePolicyDictionary : IDictionary<string, object>
        {
            private IDictionary<string, object> inner;
            private readonly JsonNamingPolicy namingPolicy;

            public KeyNamePolicyDictionary(JsonNamingPolicy namingPolicy)
            {
                inner = new Dictionary<string, object>();
                this.namingPolicy = namingPolicy;
            }

            public ICollection<string> Keys => inner.Keys;

            public ICollection<object> Values => inner.Values;

            public int Count => inner.Count;

            public bool IsReadOnly => inner.IsReadOnly;

            public object this[string key] { get => this[ConvertName(key)]; set => this[ConvertName(key)] = value; }

            public void Add(string key, object value) => inner.Add(ConvertName(key), value);

            public bool ContainsKey(string key) => inner.ContainsKey(ConvertName(key));

            public bool Remove(string key) => inner.Remove(ConvertName(key));

            public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => inner.TryGetValue(ConvertName(key), out value);

            public void Clear() => inner.Clear();

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

            public bool Contains(KeyValuePair<string, object> item) => inner.Contains(item);

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException("This method is nos allowed,  Add(string key, object value) instead");
            }

            public bool Remove(KeyValuePair<string, object> item) => inner.Remove(item);


            string ConvertName(string source) => string.Join('.', source.Split('.').Select(namingPolicy.ConvertName));
        }


        class KeyNameCollection : ICollection<string>
        {
            private readonly JsonNamingPolicy namingPolicy;
            private ICollection<string> inner;

            public KeyNameCollection(JsonNamingPolicy namingPolicy)
            {
                this.namingPolicy = namingPolicy;
                this.inner = new List<string>();
            }

            public int Count => inner.Count;

            public bool IsReadOnly => inner.IsReadOnly;

            public void Add(string item) => inner.Add(ConvertName(item));

            public void Clear() => inner.Clear();

            public bool Contains(string item) => inner.Contains(ConvertName(item));

            public void CopyTo(string[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<string> GetEnumerator() => inner.GetEnumerator();

            public bool Remove(string item) => inner.Remove(ConvertName(item));

            IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

            string ConvertName(string source) => string.Join('.', source.Split('.').Select(namingPolicy.ConvertName));
        }
    }
}
