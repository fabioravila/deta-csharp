using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DetaCSharp.Utils
{
    internal class JsonNamingPolicyCollection : ICollection<string>
    {
        private readonly JsonNamingPolicy namingPolicy;
        private ICollection<string> inner;

        public JsonNamingPolicyCollection(JsonNamingPolicy namingPolicy)
        {
            this.namingPolicy = namingPolicy;
            inner = new List<string>();
        }

        public int Count => inner.Count;

        public bool IsReadOnly => inner.IsReadOnly;

        public void Add(string item) => inner.Add(ConvertName(item));

        public void Clear() => inner.Clear();

        public bool Contains(string item) => inner.Contains(ConvertName(item));

        public void CopyTo(string[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<string> GetEnumerator() => inner.GetEnumerator();

        public bool Remove(string item) => inner.Remove(ConvertName(item));

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

        string ConvertName(string source) => string.Join('.', source.Split('.').Select(namingPolicy.ConvertName));
    }
}
