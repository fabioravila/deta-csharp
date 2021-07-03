using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace DetaCSharp.Utils
{

    class JsonNamingPolicyDictionary : IDictionary<string, object>
    {
        private IDictionary<string, object> inner;
        private readonly JsonNamingPolicy namingPolicy;

        public JsonNamingPolicyDictionary(JsonNamingPolicy namingPolicy)
        {
            inner = new Dictionary<string, object>();
            this.namingPolicy = namingPolicy;
        }

        public ICollection<string> Keys => inner.Keys;

        public ICollection<object> Values => inner.Values;

        public int Count => inner.Count;

        public bool IsReadOnly => inner.IsReadOnly;

        public object this[string key] { get => inner[ConvertKey(key)]; set => inner[ConvertKey(key)] = value; }

        public void Add(string key, object value) => inner.Add(ConvertKey(key), value);

        public bool ContainsKey(string key) => inner.ContainsKey(ConvertKey(key));

        public bool Remove(string key) => inner.Remove(ConvertKey(key));

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => inner.TryGetValue(ConvertKey(key), out value);

        public void Clear() => inner.Clear();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

        public bool Contains(KeyValuePair<string, object> item) => inner.Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotImplementedException();

        public void Add(KeyValuePair<string, object> item) => throw new NotImplementedException("This method is nos allowed,  Add(string key, object value) instead");

        public bool Remove(KeyValuePair<string, object> item) => inner.Remove(item);

        string ConvertKey(string source) => string.Join('.', source.Split('.').Select(namingPolicy.ConvertName));
    }
}
