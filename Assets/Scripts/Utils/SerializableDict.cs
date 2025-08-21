using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Encoding：UTF-8
[Serializable]
public class SerializableDictionary<K, V> : IEnumerable
{
    [Serializable]
    public struct Ele
    {
        public K key;
        public V value;
    }
    [SerializeField]
    private List<Ele> list = new List<Ele>();
    private Lazy<Dictionary<K, V>> dic;

    SerializableDictionary()
    {
        dic = new Lazy<Dictionary<K, V>>(Init);
    }

    public Dictionary<K, V> Init()
    {
        Dictionary<K, V> dictionary = new Dictionary<K, V>(list.Count);
        for (int i = 0; i < list.Count; ++i)
            dictionary.Add(list[i].key, list[i].value);
        return dictionary;
    }

    public V this[K key] { get => dic.Value[key]; set{ dic.Value[key] = value; }}

    public int Count => dic.Value.Count;

    public bool ContainsKey(K key)
    {
        return dic.Value.ContainsKey(key);
    }

    public bool TryGetValue(K key, out V value)
    {
        return dic.Value.TryGetValue(key, out value);
    }

    public IEnumerator GetEnumerator()
    {
        return dic.Value.GetEnumerator();
    }
    public Dictionary<K, V>.KeyCollection Keys
    {
        get
        {
            return dic.Value.Keys;
        }
    }
}