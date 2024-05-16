using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Linq;
using System.Threading;


namespace TotalDialogue.Core.Collections
{
    /// <summary>
    /// ITDFListは、アイテムの追加、削除、変更を監視するためのインターフェースです。
    /// 型に依存しない部分のみを定義しています。
    /// </summary>
    public interface ITDFList
    {
        /// <summary>
        /// リストが変更されたときに発生するイベント。
        /// </summary>
        event Action OnChange;

        /// <summary>
        /// 指定したインデックスのアイテムを削除します。
        /// </summary>
        /// <param name="index">削除するアイテムのインデックス。</param>
        void RemoveAt(int index);

        /// <summary>
        /// リスト内のアイテム数を取得します。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// シリアライズ中かどうかを示す値を取得します。
        /// </summary>
        public bool Serializing{get;}
    }
    /// <summary>
    /// ITDFListGenericは、アイテムの追加、削除、変更を監視するためのインターフェースのうち、型に依存する部分を定義しています。
    /// </summary>
    public interface ITDFListGeneric<T>
    {
        /// <summary>
        /// アイテムが追加されたときに発生するイベント。
        /// </summary>
        event Action<int, T> OnAdd;

        /// <summary>
        /// アイテムが削除されたときに発生するイベント。
        /// </summary>
        event Action<int, T> OnRemove;

        /// <summary>
        /// アイテムが変更されたときに発生するイベント。
        /// </summary>
        event Action<int, T> OnChangeItem;

        /// <summary>
        /// アイテムをリストに追加します。
        /// </summary>
        /// <param name="item">追加するアイテム。</param>
        void Add(T item);

        /// <summary>
        /// アイテムをリストから削除します。
        /// </summary>
        /// <param name="item">削除するアイテム。</param>
        void Remove(T item);

        /// <summary>
        /// 指定したインデックスのアイテムを取得または設定します。
        /// </summary>
        /// <param name="index">アイテムのインデックス。</param>
        T this[int index] { get; set; }

    }
    /// <summary>
    /// TDFListは、スレッドセーフなリストを提供します。
    /// シリアライズとデシリアライズのサポートも含まれています。
    /// リストの内容が変更されたときにイベントを発生させることができます。
    /// </summary>
    [Serializable]
    public class TDFList<T> : ITDFList ,ITDFListGeneric<T>,IEnumerable<T>,ISerializationCallbackReceiver
    {
        /// <summary>
        /// リスト本体、protectedだが、インスペクタによって内容が変更されることがある
        /// </summary>
        [SerializeField]
        protected List<T> list;
        /// <summary>
        /// ロックオブジェクト。
        /// </summary>
        protected readonly object lockObject = new();

        /// <summary>
        /// リストがロックされているかどうかを示す値。
        /// </summary>
        protected bool isLocked = false;

        /// <summary>
        /// ロックを保持しているスレッドのID。
        /// </summary>
        protected int lockedThreadId = -1;

        /// <summary>
        /// シリアライズ中かどうかを示す値。
        /// </summary>
        protected bool m_serializing;
        /// <summary>
        /// シリアライズ中かどうかを示す値を取得します。
        /// </summary>
        public bool Serializing => m_serializing;
        /// <summary>
        /// ロックを開始します。ロックが取得できるまで待機します。
        /// </summary>
        public void BeginLock()
        {
            lock (lockObject)
            {
                try
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    isLocked = true;
                    lockedThreadId = Thread.CurrentThread.ManagedThreadId;
                }
                catch (Exception)
                {
                    isLocked = false;
                    lockedThreadId = -1;
                    throw;
                }
            }
        }
        /// <summary>
        /// ロックを終了します。ロックを保持しているスレッドのIDをリセットし、ロックオブジェクトのロックを解除します。
        /// </summary>
        public void EndLock()
        {
            lock (lockObject)
            {
                if (!isLocked && lockedThreadId <= 0) return;
                if (Thread.CurrentThread.ManagedThreadId != lockedThreadId) return;
                isLocked = false;
                lockedThreadId = -1;
                Monitor.PulseAll(lockObject);
            }
        }
        /// <summary>
        /// リストが変更されたときに発生するイベント。
        /// </summary>
        [field: NonSerialized]
        public event Action OnChange;
        /// <summary>
        /// アイテムが追加されたときに発生するイベント。
        /// </summary>
        [field: NonSerialized]
        public event Action<int, T> OnAdd;
        /// <summary>
        /// アイテムが削除されたときに発生するイベント。
        /// </summary>
        [field: NonSerialized]
        public event Action<int, T> OnRemove;
        /// <summary>
        /// アイテムが変更されたときに発生するイベント。
        /// </summary>
        [field: NonSerialized]
        public event Action<int, T> OnChangeItem;
        /// <summary>
        /// アイテムの設定時に呼び出されるバリデーションメソッド。
        /// </summary>
        /// <param name="arg1">設定しようとしている値</param>
        /// <param name="arg2">設定されている値</param>
        /// <returns>設定を許可する場合は引数1を、許可しない場合はnullを返す</returns>
        public Func<T,T,T> Validate { get; set; }
        /// <summary>
        /// リストを初期化します。
        /// </summary>
        public TDFList()
        {
            list = new List<T>();
        }
        /// <summary>
        /// リストを初期化します。
        ///   引数1:リスト
        /// </summary>
        public TDFList(List<T> list)
        {
            this.list = list;
        }
        /// <summary>
        /// アイテムをリストに追加します。
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <remarks>
        /// バリデーションメソッドが設定されている場合、バリデーションメソッドを呼び出してから追加します。
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// アイテムが追加されたときにOnAddイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Add(T item)
        {
            if (Validate != null && Validate(item,item) != null)
            {
                return;
            }
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Add(item);
                UpdateDuplicates();
                OnAdd?.Invoke(list.Count - 1, item);
                InvokeOnChange();
            }
        }
        /// <summary>
        /// アイテムをリストから削除します。
        /// </summary>
        /// <param name="item">削除するアイテム</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// アイテムがリストに存在しない場合は削除しません。
        /// アイテムが削除されたときにOnRemoveイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Remove(T item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                var index = list.IndexOf(item);
                if (index == -1)
                {
                    return;
                }
                list.Remove(item);
                UpdateDuplicates();
                InvokeOnChange();
                OnRemove?.Invoke(index, item);
            }
        }
        /// <summary>
        /// 指定したインデックスのアイテムを削除します。
        /// </summary>
        /// <param name="index">削除するアイテムのインデックス</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// インデックスが範囲外の場合は削除しません。
        /// アイテムが削除されたときにOnRemoveイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void RemoveAt(int index)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                if (index < 0 || index >= list.Count)
                {
                    return;
                }
                var item = list[index];
                list.RemoveAt(index);
                UpdateDuplicates();
                InvokeOnChange();
                OnRemove?.Invoke(index, item);
            }
        }
        /// <summary>
        /// リスト内のアイテム数を取得します。
        /// </summary>
        /// <returns>リスト内のアイテム数</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public virtual int Count
        {
            get
            {
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    return list.Count;
                }
            }
        }
        /// <summary>
        /// 指定したインデックスのアイテムを取得または設定します。
        /// </summary>
        /// <param name="index">アイテムのインデックス</param>
        /// <returns>指定したインデックスのアイテム</returns>
        /// <remarks>
        /// バリデーションメソッドが設定されている場合、バリデーションメソッドを呼び出してから設定します。
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// アイテムが設定されたときにOnChangeItemイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual T this[int index]
        {
            get
            {
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    return list[index];
                }
            }
            set
            {
                if (Validate != null && Validate(value,list[index]) != null)
                {
                    return;
                }
                lock (lockObject)
                {
                    while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                    {
                        Monitor.Wait(lockObject);
                    }
                    list[index] = value;
                    UpdateDuplicates();
                    InvokeOnChange();
                    OnChangeItem?.Invoke(index, value);
                }
            }
        }
        /// <summary>
        /// リストをクリアします。
        /// </summary>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストがクリアされたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Clear()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Clear();
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リストにアイテムを挿入します。
        /// </summary>
        /// <param name="index">挿入するインデックス</param>
        /// <param name="item">挿入するアイテム</param>
        /// <remarks>
        /// バリデーションメソッドが設定されている場合、バリデーションメソッドを呼び出してから挿入します。
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// アイテムが挿入されたときにOnAddイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Insert(int index, T item)
        {
            if (Validate != null && Validate(item,item) != null)
            {
                return;
            }
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Insert(index, item);
                UpdateDuplicates();
                InvokeOnChange();
                OnAdd?.Invoke(index, item);
            }
        }
        /// <summary>
        /// リストにアイテムを複数挿入します。
        /// </summary>
        /// <param name="collection">挿入するアイテム(複数)</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// バリデーションメソッドが設定されている場合、バリデーションメソッドを呼び出してから追加します。
        /// アイテムが追加されたときにOnAddイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                foreach (var item in collection)
                {
                    Add(item);
                }
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リストから数を指定してアイテムを削除します。
        /// </summary>
        /// <param name="index">削除するインデックス</param>
        /// <param name="count">削除するアイテム数</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// インデックスが範囲外の場合は削除しません。
        /// アイテムが削除されたときにOnRemoveイベントを発生させます。
        /// リストが変更されたときにOnChangeイベントを発生させます。
        /// リストが変更されたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void RemoveRange(int index, int count)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.RemoveRange(index, count);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムをソートします。
        /// </summary>
        /// <param name="comparison">比較メソッド</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストがソートされたときにOnChangeイベントを発生させます。
        /// リストがソートされたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Sort(Comparison<T> comparison)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Sort(comparison);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムをソートします。
        /// </summary>
        /// <param name="comparer">比較メソッド</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストがソートされたときにOnChangeイベントを発生させます。
        /// リストがソートされたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Sort(IComparer<T> comparer)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Sort(comparer);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムを範囲指定でソートします。
        /// </summary>
        /// <param name="index">ソートする開始インデックス</param>
        /// <param name="count">ソートするアイテム数</param>
        /// <param name="comparer">比較メソッド</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストがソートされたときにOnChangeイベントを発生させます。
        /// リストがソートされたときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Sort(index, count, comparer);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムを逆順にします。
        /// </summary>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストが逆順になったときにOnChangeイベントを発生させます。
        /// リストが逆順になったときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Reverse()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Reverse();
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムを範囲指定で逆順にします。
        /// </summary>
        /// <param name="index">逆順にする開始インデックス</param>
        /// <param name="count">逆順にするアイテム数</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// リストが逆順になったときにOnChangeイベントを発生させます。
        /// リストが逆順になったときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public virtual void Reverse(int index, int count)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.Reverse(index, count);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リスト内のアイテムをコピーします。
        /// </summary>
        /// <param name="array">コピー先の配列</param>
        /// <param name="arrayIndex">コピー開始インデックス</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.CopyTo(array, arrayIndex);
            }
        }
        /// <summary>
        /// リスト内の各要素に対してActionを実行します。
        /// </summary>
        /// <param name="action">実行するAction</param>
        /// <param name="invoveOnChange">OnChangeイベントを発生させるかどうか (デフォルト: false)</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// 処理が完了したときにOnChangeイベントを発生させるかどうかを指定します。
        /// </remarks>
        public virtual void ForEach(Action<T> action,bool invoveOnChange = false)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                foreach (var item in list)
                {
                    action(item);
                }
                UpdateDuplicates();
                if(invoveOnChange){
                    InvokeOnChange();
                }
            }

        }
        /// <summary>
        /// リストのEnumeratorを返します。
        /// </summary>
        /// <returns>リストのEnumerator</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public virtual List<T>.Enumerator GetEnumerator()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.GetEnumerator();
            }
        }
        /// <summary>
        /// リスト内のアイテムを範囲指定で取得します。
        /// </summary>
        /// <param name="index">取得する開始インデックス</param>
        /// <param name="count">取得するアイテム数</param>
        /// <returns>取得したアイテム</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public List<T> GetRange(int index, int count)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.GetRange(index, count);
            }
        }
        /// <summary>
        /// リスト内のアイテムを検索します。
        /// </summary>
        /// <param name="item">検索するアイテム</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int IndexOf(T item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.IndexOf(item);
            }
        }
        /// <summary>
        /// リスト内のアイテムを逆順に検索します。
        /// </summary>
        /// <param name="item">検索するアイテム</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int LastIndexOf(T item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.LastIndexOf(item);
            }
        }
        /// <summary>
        /// リスト内に指定したアイテムが含まれているかどうかを取得します。
        /// </summary>
        /// <param name="item">検索するアイテム</param>
        /// <returns>含まれているかどうか</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public bool Contains(T item)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.Contains(item);
            }
        }
        /// <summary>
        /// リスト内のアイテムを検索します。
        /// </summary>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテム</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public T Find(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.Find(match);
            }
        }
        /// <summary>
        /// リスト内のアイテムを検索します。
        /// </summary>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテム（複数）</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public List<T> FindAll(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindAll(match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを検索します。
        /// </summary>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindIndex(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindIndex(match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを検索します。
        /// </summary>
        /// <param name="startIndex">検索開始インデックス</param>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindIndex(startIndex, match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを検索します。
        /// </summary>
        /// <param name="startIndex">検索開始インデックス</param>
        /// <param name="count">検索するアイテム数</param>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindIndex(startIndex, count, match);
            }
        }
        /// <summary>
        /// リスト内のアイテムを逆順で検索します。
        /// </summary>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテム</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public T FindLast(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindLast(match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを逆順で検索します。
        /// </summary>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindLastIndex(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindLastIndex(match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを逆順で検索します。
        /// </summary>
        /// <param name="startIndex">検索開始インデックス</param>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindLastIndex(startIndex, match);
            }
        }
        /// <summary>
        /// リスト内のアイテムのインデックスを逆順で検索します。
        /// </summary>
        /// <param name="startIndex">検索開始インデックス</param>
        /// <param name="count">検索するアイテム数</param>
        /// <param name="match">検索条件</param>
        /// <returns>検索したアイテムのインデックス</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.FindLastIndex(startIndex, count, match);
            }
        }
        /// <summary>
        /// リストから一致するすべてのアイテムを削除します。
        /// </summary>
        /// <param name="match">削除条件</param>
        /// <returns>削除したアイテム数</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// 削除が発生したときにOnChangeイベントを発生させます。
        /// 削除が発生したときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>        
        public virtual int RemoveAll(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                int result = list.RemoveAll(match);
                UpdateDuplicates();
                if (result > 0) InvokeOnChange();
                return result;
            }
        }
        /// <summary>
        /// リストの要素を新しい配列にコピーします。
        /// </summary>
        /// <returns>新しい配列</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public T[] ToArray()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// リストの容量を、実際の要素数に合わせて最小限にします。
        /// </summary>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// Trimによって要素数が変化したときにOnChangeイベントを発生させます。
        /// Trimによって要素数が変化したときにInvokeOnChangeメソッドを呼び出します。
        /// </remarks>
        public void TrimExcess()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                int count = list.Count;
                list.TrimExcess();
                if (count != list.Count){
                    UpdateDuplicates();
                    InvokeOnChange();
                }
            }
        }
        /// <summary>
        /// リストのすべての要素が指定した述語によって定義される条件を満たすかどうかを判断します。
        /// </summary>
        /// <param name="match">判断条件</param>
        /// <returns>すべての要素が条件を満たす場合は true。それ以外の場合は false。</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public bool TrueForAll(Predicate<T> match)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.TrueForAll(match);
            }
        }
        /// <summary>
        /// リストの列挙子を返します。
        /// </summary>
        /// <returns>リストの列挙子</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.GetEnumerator();
            }
        }
        /// <summary>
        /// リストの列挙子を返します。
        /// </summary>
        /// <returns>リストの列挙子</returns>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                return list.GetEnumerator();
            }
        }
        /// <summary>
        /// リストに指定したコレクションの要素を追加します。
        /// </summary>
        /// <param name="collection">追加する要素のコレクション</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public virtual void AddRange(TDFList<T> collection)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                list.AddRange(collection);
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リストから指定したコレクションの要素をすべて削除します。
        /// </summary>
        /// <param name="collection">削除する要素のコレクション</param>
        /// <remarks>
        /// ロックされている場合、ロックが解除されるまで待機します。
        /// </remarks>
        public virtual void RemoveRange(TDFList<T> collection)
        {
            lock (lockObject)
            {
                while (isLocked && Thread.CurrentThread.ManagedThreadId != lockedThreadId)
                {
                    Monitor.Wait(lockObject);
                }
                foreach (var item in collection)
                {
                    list.Remove(item);
                }
                UpdateDuplicates();
                InvokeOnChange();
            }
        }
        /// <summary>
        /// リストの要素の型を取得します。
        /// </summary>
        /// <returns>リストの要素の型</returns>
        public Type GetItemType()
        {
            return typeof(T);
        }
        /// <summary>
        /// リスト内の重複要素の処理を行います
        /// </summary>
        /// <remarks>
        /// サブクラスのためのスタブメソッドであり、オーバーライドして使用します。
        /// リスト内に重複要素がある場合、重複要素を処理します。
        /// 要素の位置関係が変更されている可能性があるときに呼びだすことを推奨します。
        /// </remarks>
        public virtual void UpdateDuplicates(){
            // Do nothing(Stub)
        }
        /// <summary>
        /// シリアライズ前の処理を行います。
        /// </summary>
        public virtual void OnBeforeSerialize()
        {
            m_serializing = true;
            UpdateDuplicates();
           // Do nothing(Stub)
        }
        /// <summary>
        /// デシリアライズ後の処理を行います。
        /// </summary>
        public virtual void OnAfterDeserialize()
        {
            UpdateDuplicates();
            m_serializing = false;
           // Do nothing(Stub)
        }
        /// <summary>
        /// OnChangeイベントを発生させます。
        /// </summary>
        public void InvokeOnChange()
        {
            //if (!Serializing) OnChange?.Invoke();
            OnChange?.Invoke();
        }
        /// <summary>
        /// OnChangeItemイベントを発生させます。
        /// </summary>
        protected void InvokeOnChangeItem(int index, T item)
        {
            OnChangeItem?.Invoke(index, item);
        }
        /// <summary>
        /// OnAddイベントを発生させます。
        /// </summary>
        protected void InvokeOnAdd(int index, T item)
        {
            OnAdd?.Invoke(index, item);
        }
        /// <summary>
        /// OnRemoveイベントを発生させます。
        /// </summary>
        protected void InvokeOnRemove(int index, T item)
        {
            OnRemove?.Invoke(index, item);
        }
    }
}