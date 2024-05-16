using UnityEngine;
using System.Collections.Generic;

namespace TotalDialogue.Core.Collections
{
    /// <summary>
    /// TDFKeyValuePairは、キーと値のペアを表します。
    /// このクラスは、KeyValuePairと同じように動作しますが、シリアライズ可能です。
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">値の型</typeparam>
    [System.Serializable]
    public class TDFKeyValuePair<TKey, TValue>
    {
        /// <summary>
        /// キー(シリアライズ用)
        /// </summary>
        [SerializeField]
        private TKey key;
        /// <summary>
        /// 値(シリアライズ用)
        /// </summary>
        [SerializeField]
        private TValue value;
        /// <summary>
        /// インスペクタ上での分割比率(HideInInspector)
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected float splitRatio; // 初期値はDrawerで設定される
        /// <summary>
        /// インスペクタ上でのドラッグ開始フラグ(HideInInspector)
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected bool isDragging = false; // ドラッグ中かどうかを示すフラグ
        /// <summary>
        /// キーを取得または設定します。
        /// </summary>
        public TKey Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public TValue Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// TDFKeyValuePairの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public TDFKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
/*
        /// <summary>
        /// TDFKeyValuePairをKeyValuePairに変換します。
        /// </summary>
        /// <param name="skvp">変換するTDFKeyValuePair</param>
        public static implicit operator KeyValuePair<TKey, TValue>(TDFKeyValuePair<TKey, TValue> skvp)
        {
            return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
        }
*/
        /// <summary>
        /// KeyValuePairをTDFKeyValuePairに変換します。
        /// </summary>
        /// <param name="kvp">変換するKeyValuePair</param>
        public static implicit operator TDFKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
        {
            return new TDFKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
        }
    }
}