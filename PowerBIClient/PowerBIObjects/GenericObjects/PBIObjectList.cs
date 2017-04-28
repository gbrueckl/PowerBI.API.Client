using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    [JsonObject]
    public class PBIObjectList<T> : IList<T>
    {
        #region Private Properties for Serialization
        [JsonProperty(PropertyName = "@odata.context", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        private string ODataContext;

        public T this[int index] { get => ((IList<T>)Items)[index]; set => ((IList<T>)Items)[index] = value; }

        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public List<T> Items { get; set; }

        public int Count => ((IList<T>)Items).Count;

        public bool IsReadOnly => ((IList<T>)Items).IsReadOnly;

        public void Add(T item)
        {
            ((IList<T>)Items).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)Items).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)Items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)Items).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)Items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)Items).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)Items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)Items).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)Items).GetEnumerator();
        }
        #endregion
    }
}
