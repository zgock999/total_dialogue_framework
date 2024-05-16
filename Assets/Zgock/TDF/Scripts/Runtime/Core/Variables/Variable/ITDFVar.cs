using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    /// <summary>
    /// Variableの基本となる抽象クラス
    /// 型に依存しない部分を定義します
    /// </summary>
    public interface ITDFVar
    {
        public string TypeName { get; }
        /// <summary>
        /// 変数のキーを取得します
        /// KeyValueStoreと必ずセットで使用されるので参照用に生成時に設定されます
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// 変数の値をデフォルト値に保存します
        /// </summary>
        public void StoreDefault();
        /// <summary>
        /// 変数の値をデフォルト値に戻します
        /// </summary>
        public void RestoreDefault();
        /// <summary>
        /// 変数の値を文字列で取得します
        /// </summary>
        public string StringValue { get; }
        /// <summary>
        /// 変数のインターフェースを取得します
        /// </summary>
        public ITDFVar Interface { get; }
    }
    /// <summary>
    /// Variableの基本となる抽象クラス
    /// 型に依存する部分を定義します
    /// </summary>
    public interface ITDFVar<T, TListener> where TListener : VariableListener<T>
    {
        /// <summary>
        /// 変数の値を取得します
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 変数のデフォルト値を取得します
        /// </summary>
        public T DefaultValue { get; set;}
        /// <summary>
        /// 変数のインターフェースを取得します
        /// </summary>
        public ITDFVar<T,TListener> GenericInterface { get; }
        /// <summary>
        /// 変数のリスナーを追加します
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void AddListener(TListener listener);
        /// <summary>
        /// 変数のリスナーを削除します
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void RemoveListener(TListener listener);
    }
}