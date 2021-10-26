namespace System.Collections.Generic
{
    public class Dictionary<TKey, TValue>
    {
        public TValue this[TKey key]
        {
            get
            {
                return values[keys.IndexOf(key)];
            }
            set
            {
                values[keys.IndexOf(key)] = value;
            }
        }

        List<TKey> keys;
        List<TValue> values;

        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public void Remove(TKey key) 
        {
            values.Remove(values[keys.IndexOf(key)]);
            keys.Remove(key);
        }

        public Dictionary()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public void Add(TKey key, TValue value)
        {
            keys.Add(key);
            values.Add(value);
        }
    }
}
