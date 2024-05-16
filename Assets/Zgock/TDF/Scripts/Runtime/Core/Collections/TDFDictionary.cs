using System;
using System.Collections.Generic;
using System.Threading;
using TotalDialogue.Core.Variables;

namespace TotalDialogue.Core.Collections
{
    /// <summary>
    /// このクラスは、TDFKeyValuePairを継承し、辞書としての機能を提供します。
    /// TDFKeyValuePairのリストを内部に持ち、キーと値のペアを管理します。
    /// このクラスは、System標準のDictionaryと同じように動作しますが、シリアライズ可能です。
    /// TDFListのスレッドセーフ/ロック機能とイベント機能を持ちます。
    /// TDFListの要素数を取得するには、ListCountプロパティを使用してください。
    /// 
    /// </summary>
    [Serializable]
    public class TDFDictionary<TKey, TValue> : TDFKeyValuePairs<TKey, TValue>,IDictionary<TKey,TValue>
    {
        /// <summary>
        /// 辞書用のロックオブジェクト
        /// </summary>
        private readonly object syncRoot = new();
        /// <summary>
        /// 内部の辞書
        /// </summary>
         [NonSerialized]
        private Dictionary<TKey, TValue> internalDictionary = new Dictionary<TKey, TValue>();
        /// <summary>
        /// 辞書を取得してGeneric.Dictionaryとして使用します。
        /// </summary>
        public Dictionary<TKey, TValue> AsDictionary => internalDictionary;
        /// <summary>
        /// キーのコレクションを取得します。
        /// </summary>
        public ICollection<TKey> Keys => internalDictionary.Keys;
        /// <summary>
        /// 値のコレクションを取得します。
        /// </summary>
        public ICollection<TValue> Values => internalDictionary.Values;
        /// <summary>
        /// 辞書としての要素数を取得します。
        /// </summary>
        /// <value>要素数</value>
        /// <returns>要素数</returns>
        /// <remarks>
        /// このプロパティは、内部の辞書の要素数を返します。
        /// TDFListの要素数を取得するには、ListCountプロパティを使用してください。
        /// </remarks>
        public override int Count => internalDictionary.Count;
        /// <summary>
        /// TDFListの要素数を取得します。
        /// </summary>
        /// <value>要素数</value>
        /// <returns>要素数</returns>
        /// <remarks>
        /// このプロパティは、TDFListの要素数を返します。
        /// 辞書の要素数を取得するには、Countプロパティを使用してください。
        /// </remarks>
        public virtual int ListCount => list.Count;
        /// <summary>
        /// KeyValuePairのコレクションからTDFDictionaryを初期化します。
        /// </summary>
        /// <param name="pairs">KeyValuePairのコレクション</param>
        /// <remarks>
        /// このコンストラクタは、KeyValuePairのコレクションからTDFDictionaryを初期化します。
        /// </remarks>
        public TDFDictionary(List<KeyValuePair<TKey, TValue>> pairs) : base()
        {
            foreach (var pair in pairs)
            {
                list.Add(new TDFKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
            UpdateDuplicates();
        }
        /// <summary>
        /// TDFDictionaryの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="list">TDFKeyValuePairのリスト</param>
        /// <remarks>
        /// このコンストラクタは、TDFKeyValuePairのリストからTDFDictionaryを初期化します。
        /// </remarks>
        /// <seealso cref="TDFKeyValuePair{TKey, TValue}"/>
        public TDFDictionary(List<TDFKeyValuePair<TKey, TValue>> list) : base(list)
        {
            UpdateDuplicates();
        }
        /// <summary>
        /// TDFDictionaryの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks>
        /// このコンストラクタは、TDFKeyValuePairのリストを初期化します。
        /// </remarks>
        public TDFDictionary() : base()
        {
        }

        /// IDictionary<TKey, TValue>の実装ですが、このメソッドは使用しないでください。
        IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs
        {
            get
            {
                foreach (var pair in internalDictionary)
                {
                    yield return pair;
                }
            }
        }
        /// <summary>
        /// 辞書が読み取り専用かどうかを取得します。
        /// </summary>
        /// <value>読み取り専用の場合はtrue、それ以外はfalse</value>
        /// <returns>読み取り専用の場合はtrue、それ以外はfalse</returns>
        /// <remarks>
        /// このプロパティは、辞書が読み取り専用かどうかを取得します。
        /// このプロパティは常にfalseを返します。(読み取り専用ではありません)
        /// </remarks>
        public bool IsReadOnly => false;

        /// <summary>
        /// 標準DictionaryのIEnumerableを取得します。
        /// </summary>
        /// <value>標準Dictionaryのコレクション</value>
        /// <returns>標準Dictionaryのコレクション</returns>
        /// <remarks>
        /// このプロパティは、標準Dictionaryのコレクションを返します。
        /// </remarks>
        public new Dictionary<TKey,TValue>.Enumerator GetEnumerator()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return internalDictionary.GetEnumerator();
            }
        }
        /// <summary>
        /// Generic DictionaryからTDFKeyValuePairのリストを取得します。
        /// </summary>
        /// <param name="dictionary">System標準のDictionary</param>
        public TDFDictionary(Dictionary<TKey, TValue> dictionary) : base()
        {
            foreach (var pair in dictionary)
            {
                list.Add(new TDFKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
            UpdateDuplicates();
        }
        /// <summary>
        /// キーが存在するかどうかを取得します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>キーが存在する場合はtrue、それ以外はfalse</returns>
        /// <remarks>
        /// このメソッドは、指定されたキーが存在するかどうかを取得します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public override bool ContainsKey(TKey key)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return internalDictionary.ContainsKey(key);
            }
        }
        /// <summary>
        /// 値が存在するかどうかを取得します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>値が存在する場合はtrue、それ以外はfalse</returns>
        /// <remarks>
        /// このメソッドは、指定された値が存在するかどうかを取得します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return internalDictionary.ContainsValue(value);
            }
        }
        /// <summary>
        /// キーと値のペアが存在するかどうかを取得します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値(out)</param>
        /// <returns>キーと値のペアが存在する場合はtrue、それ以外はfalse</returns>
        /// <remarks>
        /// このメソッドは、指定されたキーを持つ値のペアが存在するかどうかを取得します。
        /// きーと値のペアが存在する場合は、値をoutパラメータで返します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return internalDictionary.TryGetValue(key, out value);
            }
        }
        /// <summary>
        /// キーと値のペアを取得または設定します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        /// <remarks>
        /// このインデクサは、指定されたキーに対応する値を取得または設定します。
        /// 設定時、キーが存在しない場合は新しいキーと値のペアを追加します。
        /// キーが存在した場合はOnChangeItemイベントを発生させます。
        /// キーが存在しない場合はOnAddイベントを発生させます。
        /// 共通でOnChangeイベントを発生させます。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    return internalDictionary[key];
                }
            }
            set
            {
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    int index = IndexOfKey(key);
                    var item = new TDFKeyValuePair<TKey, TValue>(key, value);
                    if (index >= 0)
                    {
                        list[index] = item;
                        internalDictionary[key] = value;
                        InvokeOnChangeItem(index, item);
                    }
                    else
                    {
                        index = list.Count;
                        list.Add(item);

                        if (!indexDictionary.ContainsKey(key))
                        {
                            indexDictionary[key] = new List<int>();
                        }
                        indexDictionary[key].Add(list.Count - 1);
                        if (!IsDuplicateIndex(index))
                        {
                            internalDictionary.Add(item.Key, item.Value);
                        }
                        InvokeOnAdd(index,item);
                    }
                    InvokeOnChange();
                }
            }
        }
        /// <summary>
        /// キーと値のペアを追加します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        /// <remarks>
        /// このメソッドは、指定されたキーと値のペアを追加します。
        /// 追加された結果、重複している場合はそれをマークし、辞書には追加しません。
        /// OnAdd/OnChangeイベントを発生させます。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            Add(new TDFKeyValuePair<TKey, TValue>(key, value));
        }
        public override void Add(TDFKeyValuePair<TKey, TValue> item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                int index = list.Count;
                list.Add(item);

                TKey key = item.Key;

                if (!indexDictionary.ContainsKey(key))
                {
                    indexDictionary[key] = new List<int>();
                }

                indexDictionary[key].Add(ListCount - 1);
                if (!IsDuplicateIndex(index))
                {
                    internalDictionary.Add(item.Key, item.Value);
                }
                InvokeOnChange();
            }
        }
        /// <summary>
        /// 重複情報を更新して、辞書を再構築します。
        /// </summary>
        /// <remarks>
        /// このメソッドは、重複情報を更新して、辞書を再構築します。
        /// </remarks>
        public override void UpdateDuplicates()
        {
            lock (syncRoot)
            {
                base.UpdateDuplicates();

                internalDictionary.Clear();
                for (int i = 0; i < ListCount; i++)
                {
                    if (!IsDuplicateIndex(i))
                    {
                        var pair = this[i];
                        internalDictionary[pair.Key] = pair.Value;
                    }
                }
            }
        }
        /// <summary>
        /// Generic Dictionaryに型変換します。
        /// </summary>
        public static implicit operator Dictionary<TKey, TValue>(TDFDictionary<TKey, TValue> pairs)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var pair in pairs)
            {
                dictionary[pair.Key] = pair.Value;
            }
            return dictionary;
        }
        /// <summary>
        /// Generic DictionaryからTDFDictionaryに型変換します。
        /// </summary>
        public static implicit operator TDFDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            var pairs = new TDFDictionary<TKey, TValue>();
            foreach (var pair in dictionary)
            {
                pairs.Add(pair.Key, pair.Value);
            }
            return pairs;
        }

        /// <summary>
        /// DictionaryからTDFDictionaryのインスタンスを生成します。
        /// </summary>
        /// <param name="dictionary">System標準のDictionary</param>
        /// <returns>TDFDictionaryのインスタンス</returns>
        /// <remarks>
        /// このメソッドは、System標準のDictionaryからTDFDictionaryのインスタンスを生成します。
        /// </remarks>
        public static TDFDictionary<TKey, TValue> FromDictionary(Dictionary<TKey, TValue> dictionary)
        {
            return (TDFDictionary<TKey, TValue>)dictionary;
        }
        /// <summary>
        /// TDFDictionaryからDictionaryのインスタンスを生成します。
        /// </summary>
        /// <param name="pairs">TDFDictionary</param>
        /// <returns>System標準のDictionary</returns>
        /// <remarks>
        /// このメソッドは、TDFDictionaryからSystem標準のDictionaryのインスタンスを生成します。
        /// </remarks>
        public static Dictionary<TKey, TValue> ToDictionary(TDFDictionary<TKey, TValue> pairs)
        {
            return (Dictionary<TKey, TValue>)pairs;
        }
        /// <summary>
        /// DictionaryインターフェースからTDFDictionaryのインスタンスを生成します。
        /// </summary>
        /// <param name="dictionary">System標準のDictionary</param>
        /// <returns>TDFDictionaryのインスタンス</returns>
        /// <remarks>
        /// このメソッドは、System標準のDictionaryからTDFDictionaryのインスタンスを生成します。
        /// </remarks>
        public static TDFDictionary<TKey, TValue> FromDictionary(IDictionary<TKey, TValue> dictionary)
        {
            return (TDFDictionary<TKey, TValue>)dictionary;
        }

        /// <summary>
        /// generic KetValuePairから要素を追加します
        /// </summary>
        /// <param name="item">追加する要素</param>
        /// <remarks>
        /// このメソッドは、KeyValuePairから要素を追加します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        /// <summary>
        /// キーと値のペアが存在するかどうかを取得します。
        /// </summary>
        /// <param name="item">キーと値のペア</param>
        /// <returns>キーと値のペアが存在する場合はtrue、それ以外はfalse</returns>
        /// <remarks>
        /// このメソッドは、指定されたキーと値のペアが存在するかどうかを取得します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return IndexOf(new TDFKeyValuePair<TKey,TValue>(item.Key,item.Value)) >= 0;
        }

        /// <summary>
        /// キーと値のペアを配列にコピーします。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in internalDictionary)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// キーと値のペアを削除します。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>ペアが存在して値が削除されたらtrue、存在せず削除が行われなかった場合はfalse</returns>
        /// <remarks>
        /// このメソッドは、指定されたキーと値のペアを削除します。
        /// リストがロックされている場合は、待機します。
        /// </remarks>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = IndexOf(new TDFKeyValuePair<TKey, TValue>(item.Key, item.Value));
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        /// IDictionary<TKey, TValue>の実装ですが、このメソッドは使用しないでください。
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }
        public void AddNew(object key,Type type)
        {
            Add(new TDFKeyValuePair<TKey, TValue>((TKey)key, (TValue)Activator.CreateInstance(type)));
        }
    }
}