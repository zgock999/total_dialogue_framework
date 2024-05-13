#if UNITY_EDITOR
using UnityEditor;
#endif
using TotalDialogue;
using UnityEngine;
using System.Collections.Generic;
using TotalDialogue.Core.Collections;
[CreateAssetMenu(fileName = "ObservableScriptable", menuName = "TDF/Test/ObservableScriptable")]
public class ObservableScriptable : ScriptableObject
{
    public TDFList<int> intList = new();
    public TDFList<float> floatList = new();
    public TDFList<string> stringList = new();
    public TDFList<Vector3> vector3List = new();
    public TDFDictionary<string, int> dictionary = new();
    public TDFDictionary<GameObject, List<Vector3>> gemeObjects = new();

    [System.Serializable]
    public class MyClass : TDFList<TDFKeyValuePair<int,string>>
    {
        public void AddNew()
        {
            Add(new TDFKeyValuePair<int,string>(Count, Count.ToString()));
        }
    }
    public MyClass myClass = new();

    //public ObservableList<EditableKeyValuePair<int, string>> keyValuePair = new();
    public TDFKeyValuePairs<int, string> keyValuePair = new();

    void OnChange()
    {
        Debug.Log("Change");
    }

    void OnEnable()
    {
        intList.OnChange += OnChange;
        floatList.OnChange += OnChange;
        stringList.OnChange += OnChange;
        vector3List.OnChange += OnChange;
    }
}