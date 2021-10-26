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
