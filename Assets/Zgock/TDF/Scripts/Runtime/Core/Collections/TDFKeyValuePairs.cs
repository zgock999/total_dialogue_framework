using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Core.Collections
{
    /// <summary>
    /// TDFKeyValuePairsは、キーと値のペアのリストを表します。
    /// このクラスは、TDFListを継承しており、シリアライズ/スレッドセーフ/イベント発行機能を持ちます
    /// TDFListと同様に、インデックスを使用して要素を取得できますが、キーを使用して要素を取得することもできます
    /// （Dictionayとして使用可能なサブクラスとしてTDFDictionayが存在します）
    /// キーに重複がある場合、それを検出することができます
    /// キーに重複がある場合、キーを使用して要素を取得すると、最初に見つかった要素が返されます
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">値の型</typeparam>
    [Serializable]
    public class TDFKeyValuePairs<TKey, TValue> : TDFList<TDFKeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// インスペクタ上で折りたたむかどうかを示す値
        /// </summary>
        public bool folded = false;
        /// <summary>
        /// 重複を検出するためのDictionary、キーはTKey、値はインデックスのリストです
        /// </summary>
        [NonSerialized]
        protected readonly Dictionary<TKey, List<int>> indexDictionary = new Dictionary<TKey, List<int>>();
        /// <summary>
        /// TDFKeyValuePairsの新しいインスタンスを初期化します。
        /// </summary>
        public TDFKeyValuePairs() : base()
        {
        }
        /// <summary>
        /// TDFKeyValuePairのリストを使用して、TDFKeyValuePairsの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="list">TDFKeyValuePairのリスト</param>
        public TDFKeyValuePairs(List<TDFKeyValuePair<TKey, TValue>> list) : base(list)
        {
            UpdateDuplicates();
        }
        /// <summary>
        /// KeyValuePairのリストを使用して、TDFKeyValuePairsの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="pairs">KeyValuePairのリスト</param>
        public TDFKeyValuePairs(List<KeyValuePair<TKey, TValue>> pairs) : base()
        {
            foreach (var pair in pairs)
            {
                list.Add(new TDFKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
            UpdateDuplicates();
        }
        /// <summary>
        /// 指定したインデックスにある要素を取得または設定します。
        /// </summary>
        /// <param name="index">取得または設定する要素の0から始まるインデックス</param>
        /// <returns>指定したインデックスにある要素</returns>
        /// <remarks>
        /// リストの内容が変更された場合、OnChangeItem/OnChangeイベントが発行されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public override TDFKeyValuePair<TKey, TValue> this[int index]
        {
            get{
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    return base[index];
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
                    TKey oldKey = list[index].Key;
                    TKey newKey = value.Key;
                    list[index] = value;
                    if (!oldKey.Equals(newKey)) UpdateDuplicates();
                    InvokeOnChangeItem(index, value);
                    InvokeOnChange();
                }
            }
        }
        /// <summary>
        /// 指定したアイテムをリストの末尾に追加し、重複情報を更新します。
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <remarks>
        /// 追加された場合、OnAdd/OnChangeイベントが発行されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public override void Add(TDFKeyValuePair<TKey, TValue> item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Add(item);

                TKey key = item.Key;

                if (!indexDictionary.ContainsKey(key))
                {
                    indexDictionary[key] = new List<int>();
                }

                indexDictionary[key].Add(Count - 1);
                InvokeOnAdd(Count - 1, item);
                InvokeOnChange();
            }
        }
        /// <summary>
        /// 指定した位置にアイテムを挿入し、重複情報を更新します。
        /// </summary>
        /// <param name="index">アイテムを挿入する位置の0から始まるインデックス</param>
        /// <param name="item">挿入するアイテム</param>
        /// <remarks>
        /// 挿入された場合、OnAdd/OnChangeイベントが発行されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public override void Insert(int index, TDFKeyValuePair<TKey, TValue> item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                base.Insert(index, item);
                UpdateDuplicates();
                InvokeOnAdd(index, item);
                InvokeOnChange();
            }
        }
        /// <summary>
        /// 指定したインデックスにあるアイテムを削除し、重複情報を更新します。
        /// </summary>
        /// <param name="index">削除するアイテムの0から始まるインデックス</param>
        /// <remarks>
        /// 削除された場合、OnRemove/OnChangeイベントが発行されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public override void RemoveAt(int index)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                var item = this[index];
                TKey key = item.Key;
                indexDictionary[key].Remove(index);
                base.RemoveAt(index);
                UpdateDuplicates();
                InvokeOnRemove(index,item);
                InvokeOnChange();
            }
        }
        /// <summary>
        /// 指定したキーを持つ全てのアイテムを削除し、重複情報を更新します。
        /// </summary>
        /// <param name="index">削除するキー</param>
        /// <returns>削除された場合はtrue、それ以外の場合はfalse</returns>
        /// <remarks>
        /// 削除された場合、OnRemove/OnChangeイベントが発行されます
        /// (重複がある場合、OnRemoveイベントは複数回発行されます)
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public bool Remove(TKey key)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                if (ContainsKey(key) && indexDictionary[key].Count > 0)
                {
                    List<int> indices = indexDictionary[key];
                    for (int i = indices.Count - 1; i >= 0; i--)
                    {
                        var item = this[indices[i]];
                        list.RemoveAt(indices[i]);
                        InvokeOnRemove(indices[i], item);
                    }
                    UpdateDuplicates();
                    InvokeOnChange();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 指定した要素をと同じキーを持つ全てのアイテムを削除し、重複情報を更新します。
        /// (TDFListのRemoveメソッドをオーバーライドしており、挙動が異なるので注意してください)
        /// </summary>
        /// <param name="index">削除するアイテム</param>
        /// <remarks>
        /// 削除された場合、OnRemove/OnChangeイベントが発行されます
        /// (重複がある場合、OnRemoveイベントは複数回発行されます)
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public override void Remove(TDFKeyValuePair<TKey, TValue> item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                if (!Contains(item))
                {
                    return;
                }
                TKey key = item.Key;
                if(indexDictionary.ContainsKey(key) && indexDictionary[key].Count > 0)
                {
                    int index = indexDictionary[key][0];
                    indexDictionary[key].Remove(index);
                    base.RemoveAt(index);
                    UpdateDuplicates();
                    InvokeOnChange();
                }
            }
        }
        /// <summary>
        /// 指定したキーを持つアイテムがリストに含まれているかどうかを返します。
        /// </summary>
        /// <param name="key">検索するキー</param>
        /// <returns>リストに含まれている場合はtrue、それ以外の場合はfalse</returns>
        /// <remarks>
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public virtual bool ContainsKey(TKey key)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return indexDictionary.ContainsKey(key);
            }
        }
        /// <summary>
        /// 指定したキーを持つアイテムのインデックスを取得します。
        /// </summary>
        /// <param name="key">検索するキー</param>
        /// <returns>指定したキーを持つアイテムのインデックス</returns>
        /// <remarks>
        /// 重複がある場合、最初に見つかったアイテムのインデックスが返されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public int IndexOfKey(TKey key)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                if (indexDictionary.ContainsKey(key) && indexDictionary[key].Count > 0)
                {
                    return indexDictionary[key][0];
                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// 指定したキーが重複しているかどうかを返します。
        /// </summary>
        /// <param name="key">検索するキー</param>
        /// <returns>重複している場合はtrue、それ以外の場合はfalse</returns>
        /// <remarks>
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public bool IsDuplicate(TKey key)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return indexDictionary.ContainsKey(key) && indexDictionary[key].Count >= 2;
            }
        }
        /// <summary>
        /// 重複情報を更新します。
        /// </summary>
        /// <remarks>
        /// このメソッドはスレッドセーフではありません
        /// </remarks>
        public override void UpdateDuplicates()
        {
            indexDictionary.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                TKey key = list[i].Key;

                if (!indexDictionary.ContainsKey(key))
                {
                    indexDictionary[key] = new List<int>();
                }

                indexDictionary[key].Add(i);
            }
        }
        /// <summary>
        /// 指定したインデックスが重複しているかどうかを返します。
        /// </summary>
        /// <param name="index">検索するインデックス</param>
        /// <returns>重複している場合はtrue、それ以外の場合はfalse</returns>
        /// <remarks>
        /// 重複していても、指定したインデックスが最初に見つかったインデックスである場合はfalseが返されます
        /// リストがロックされている場合、スレッドがロック解除されるまで待機します
        /// </remarks>
        public bool IsDuplicateIndex(int index)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                TKey key = this[index].Key;

                if (indexDictionary.ContainsKey(key))
                {
                    List<int> indices = indexDictionary[key];
                    return indices[0] != index;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}