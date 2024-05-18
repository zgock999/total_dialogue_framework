using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TotalDialogue.Core.Collections;
using UnityEngine;

public class ObservableCompornent : MonoBehaviour
{
    public ObservableScriptable observableScriptable;
    
    public TMP_InputField keyInput;
    public TMP_InputField valueInput;
    public TMP_Text ListCountText;
    [SerializeField] public TDFList<int> intList = new();
    [SerializeField] public TDFList<float> floatList = new();
    [SerializeField] public TDFList<string> stringList = new();
    [SerializeField] public TDFList<Vector3> vector3List = new();
    [SerializeField] public TDFKeyValuePairs<int, string> keyValuePair = new();
    [SerializeField] public TDFDictionary<string, int> dictionary = new();

    private void Dump(){
        if (ListCountText == null) return;
        ListCountText.text = $"intList: {intList.Count}\nfloatList: {floatList.Count}\nstringList: {stringList.Count}\nvector3List: {vector3List.Count}\nkeyValuePair: {keyValuePair.Count}";
        ListCountText.text += $"\ndictionary: {dictionary.Count}";
        ListCountText.text += $"\ndictionaryList: {dictionary.ListCount}";
        if (observableScriptable != null)
        {
            ListCountText.text += $"\n\nScriptable:";
            ListCountText.text += $"\nintList: {observableScriptable.intList.Count}\nfloatList: {observableScriptable.floatList.Count}\nstringList: {observableScriptable.stringList.Count}\nvector3List: {observableScriptable.vector3List.Count}\nkeyValuePair: {observableScriptable.keyValuePair.Count}";
            ListCountText.text += $"\ndictionary: {observableScriptable.dictionary.Count}";
            ListCountText.text += $"\ndictionaryList: {observableScriptable.dictionary.ListCount}";
        }
    }
    public void DoGet(){
        string key = keyInput.text;
        if (observableScriptable != null)
        {
            if (observableScriptable.dictionary.ContainsKey(key))
            {
                valueInput.text = observableScriptable.dictionary[key].ToString();
            }
        } else {
            if (dictionary.ContainsKey(key))
            {
                valueInput.text = dictionary[key].ToString();
            }
        }
    }
    public void DoSet(){
        string key = keyInput.text;
        int value = int.Parse(valueInput.text);
        if (observableScriptable != null)
        {
            observableScriptable.dictionary[key] = value;
        } else {
            dictionary[key] = value;
        }
    }
    public void DoAdd(){
        string key = keyInput.text;
        int value = int.Parse(valueInput.text);
        if (observableScriptable != null)
        {
            observableScriptable.dictionary.Add(key, value);
        } else {
            dictionary[key] = value;
        }
    }
    public void DoRemove(){
        string key = keyInput.text;
        if (observableScriptable != null)
        {
            observableScriptable.dictionary.Remove(key);
        } else {
            dictionary.Remove(key);
        }
    }
    public void AddTo()
    {
        intList.Add(intList.Count);
        floatList.Add(floatList.Count);
        stringList.Add(stringList.Count.ToString());
        vector3List.Add(new Vector3(vector3List.Count, vector3List.Count, vector3List.Count));
        keyValuePair.Add(new TDFKeyValuePair<int, string>(keyValuePair.Count, "keyValuePair"));
        dictionary.Add(dictionary.ListCount.ToString(), dictionary.ListCount);
        if (observableScriptable != null)
        {
            observableScriptable.intList.Add(observableScriptable.intList.Count);
            observableScriptable.floatList.Add(observableScriptable.floatList.Count);
            observableScriptable.stringList.Add(observableScriptable.stringList.Count.ToString());
            observableScriptable.vector3List.Add(new Vector3(observableScriptable.vector3List.Count, observableScriptable.vector3List.Count, observableScriptable.vector3List.Count));
            observableScriptable.keyValuePair.Add(new KeyValuePair<int, string>(observableScriptable.keyValuePair.Count, "keyValuePair"));
            observableScriptable.dictionary.Add(observableScriptable.dictionary.ListCount.ToString(), dictionary.ListCount);
        }
    }
    public void KeyValueDump()
    {
        if (keyInput == null || valueInput == null) return;
        string key = keyInput.text;
        bool existing = false;
        int value = -1;
        if (observableScriptable != null)
        {
            if (observableScriptable.dictionary.ContainsKey(key))
            {
                existing = true;
                value = observableScriptable.dictionary[key];
                
            }
        } else {
            if (dictionary.ContainsKey(key))
            {
                existing = true;
                value = dictionary[key];
            }
        }
        if (existing)
        {
            valueInput.textComponent.color = Color.black;
            valueInput.text = value.ToString();
            Debug.Log($"Key: {key}, Value: {value}");
        } else {
            valueInput.textComponent.color = Color.red;
            valueInput.text = "Not Found";
            Debug.Log($"Key: {key} is not found.");
        }
    }
    public void RemoveFromLast()
    {
        if (intList.Count > 0)
        {
            intList.RemoveAt(intList.Count - 1);
        }
        if (floatList.Count > 0)
        {
            floatList.RemoveAt(floatList.Count - 1);
        }
        if (stringList.Count > 0)
        {
            stringList.RemoveAt(stringList.Count - 1);
        }
        if (vector3List.Count > 0)
        {
            vector3List.RemoveAt(vector3List.Count - 1);
        }
        if (keyValuePair.Count > 0)
        {
            keyValuePair.RemoveAt(keyValuePair.Count - 1);
        }
        if (dictionary.ListCount > 0)
        {
            dictionary.RemoveAt(dictionary.ListCount - 1);
        }
        if (observableScriptable != null)
        {
            if (observableScriptable.intList.Count > 0)
            {
                observableScriptable.intList.RemoveAt(observableScriptable.intList.Count - 1);
            }
            if (observableScriptable.floatList.Count > 0)
            {
                observableScriptable.floatList.RemoveAt(observableScriptable.floatList.Count - 1);
            }
            if (observableScriptable.stringList.Count > 0)
            {
                observableScriptable.stringList.RemoveAt(observableScriptable.stringList.Count - 1);
            }
            if (observableScriptable.vector3List.Count > 0)
            {
                observableScriptable.vector3List.RemoveAt(observableScriptable.vector3List.Count - 1);
            }
            if (observableScriptable.keyValuePair.Count > 0)
            {
                observableScriptable.keyValuePair.RemoveAt(observableScriptable.keyValuePair.Count - 1);
            }
            if (observableScriptable.dictionary.ListCount > 0)
            {
                observableScriptable.dictionary.RemoveAt(observableScriptable.dictionary.ListCount - 1);
            }
        }
    }
    public void RemoveFromTop()
    {
        if (intList.Count > 0)
        {
            intList.RemoveAt(0);
        }
        if (floatList.Count > 0)
        {
            floatList.RemoveAt(0);
        }
        if (stringList.Count > 0)
        {
            stringList.RemoveAt(0);
        }
        if (vector3List.Count > 0)
        {
            vector3List.RemoveAt(0);
        }
        if (keyValuePair.Count > 0)
        {
            keyValuePair.RemoveAt(0);
        }
        if (dictionary.ListCount > 0)
        {
            dictionary.RemoveAt(0);
        }
        if (observableScriptable != null)
        {
            if (observableScriptable.intList.Count > 0)
            {
                observableScriptable.intList.RemoveAt(0);
            }
            if (observableScriptable.floatList.Count > 0)
            {
                observableScriptable.floatList.RemoveAt(0);
            }
            if (observableScriptable.stringList.Count > 0)
            {
                observableScriptable.stringList.RemoveAt(0);
            }
            if (observableScriptable.vector3List.Count > 0)
            {
                observableScriptable.vector3List.RemoveAt(0);
            }
            if (observableScriptable.keyValuePair.Count > 0)
            {
                observableScriptable.keyValuePair.RemoveAt(0);
            }
            if (observableScriptable.dictionary.ListCount > 0)
            {
                observableScriptable.dictionary.RemoveAt(0);
            }
        }
    }

    public void ClearList()
    {
        intList.Clear();
        floatList.Clear();
        stringList.Clear();
        vector3List.Clear();
        keyValuePair.Clear();
        dictionary.Clear();
        if (observableScriptable != null)
        {
            observableScriptable.intList.Clear();
            observableScriptable.floatList.Clear();
            observableScriptable.stringList.Clear();
            observableScriptable.vector3List.Clear();
            observableScriptable.keyValuePair.Clear();
            observableScriptable.dictionary.Clear();
        }
    }
    private bool dirty = false;
    private void SetDirty(string name)
    {
        Debug.Log("SetDirty from " + name);
        dirty = true;
    }
    void DumpAll()
    {
        Dump();
        KeyValueDump();
    }
    private Action DirtyfromIntList => () => SetDirty("intList");
    private Action DirtyfromFloatList => () => SetDirty("floatList");
    private Action DirtyfromStringList => () => SetDirty("stringList");
    private Action DirtyfromVector3List => () => SetDirty("vector3List");
    private Action DirtyfromKeyValuePair => () => SetDirty("KeyValuePair");
    private Action DirtyfromDictionary => () => SetDirty("Dictionary");
    private Action DirtyfromObservableIntList => () => SetDirty("observableScriptable.intList");
    private Action DirtyfromObservableFloatList => () => SetDirty("observableScriptable.floatList");
    private Action DirtyfromObservableStringList => () => SetDirty("observableScriptable.stringList");
    private Action DirtyfromObservableVector3List => () => SetDirty("observableScriptable.vector3List");
    private Action DirtyfromObservableKeyValuePair => () => SetDirty("observableScriptable.keyValuePair");
    private Action DirtyfromObservableDictionary => () => SetDirty("observableScriptable.dictionary");
    void OnEnable()
    {
        intList.OnChange += DirtyfromIntList;
        floatList.OnChange += DirtyfromFloatList;
        stringList.OnChange += DirtyfromStringList;
        vector3List.OnChange += DirtyfromVector3List;
        keyValuePair.OnChange += DirtyfromKeyValuePair;
        dictionary.OnChange += DirtyfromDictionary;
        if (observableScriptable != null)
        {
            observableScriptable.intList.OnChange += DirtyfromObservableIntList;
            observableScriptable.floatList.OnChange += DirtyfromObservableFloatList;
            observableScriptable.stringList.OnChange += DirtyfromObservableStringList;
            observableScriptable.vector3List.OnChange += DirtyfromObservableVector3List;
            observableScriptable.keyValuePair.OnChange += DirtyfromObservableKeyValuePair;
            observableScriptable.dictionary.OnChange += DirtyfromObservableDictionary;
        }
    }
    void Start()
    {
        DumpAll();
    }
    private bool IsSerializing(){
        if (intList.Serializing) return true;
        if (floatList.Serializing) return true;
        if (stringList.Serializing) return true;
        if (vector3List.Serializing) return true;
        if (keyValuePair.Serializing) return true;
        if (dictionary.Serializing) return true;
        if (observableScriptable != null){
            if (observableScriptable.intList.Serializing) return true;
            if (observableScriptable.floatList.Serializing) return true;
            if (observableScriptable.stringList.Serializing) return true;
            if (observableScriptable.vector3List.Serializing) return true;
            if (observableScriptable.keyValuePair.Serializing) return true;
            if (observableScriptable.dictionary.Serializing) return true;
        }
        return false;
    }
    void Update()
    {
        //if (IsSerializing()) return;
        if (dirty)
        {
            DumpAll();
            dirty = false;
        }
    }
    void OnDisable()
    {
        intList.OnChange -= DirtyfromIntList;
        floatList.OnChange -= DirtyfromFloatList;
        stringList.OnChange -= DirtyfromStringList;
        vector3List.OnChange -= DirtyfromVector3List;
        keyValuePair.OnChange -= DirtyfromKeyValuePair;
        dictionary.OnChange -= DirtyfromDictionary;
        if (observableScriptable != null)
        {
            observableScriptable.intList.OnChange -= DirtyfromObservableIntList;
            observableScriptable.floatList.OnChange -= DirtyfromObservableFloatList;
            observableScriptable.stringList.OnChange -= DirtyfromObservableStringList;
            observableScriptable.vector3List.OnChange -= DirtyfromObservableVector3List;
            observableScriptable.keyValuePair.OnChange -= DirtyfromObservableKeyValuePair;
            observableScriptable.dictionary.OnChange -= DirtyfromObservableDictionary;
        }
    }
}
