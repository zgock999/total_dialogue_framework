using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TotalDialogue;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    
    [SerializeField]
    private ViewVariables variables = new();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("DialogueTest");
        Test().Forget();
    }
    async UniTaskVoid Test(){
        for(int id =  0;;id = (id + 1) % 2){
            Debug.Log("Test Started");
            await UniTask.WaitForSeconds(0.5f);
            variables.Variables.SetBool(TDFConst.clearKey + "0",true);
            variables.Variables.SetBool(TDFConst.nextableKey + "0",true);
            variables.Variables.SetBool(TDFConst.cancelableKey + "0",true);
            variables.Variables.SetBool(TDFConst.skippableKey + "0",true);
            Debug.Log("Enter Window Open");
            variables.Variables.SetInt(TDFConst.windowKey + "0",1);
            await UniTask.WaitUntil(() => variables.Variables.GetInt(TDFConst.windowKey + "0") == 2);
            Debug.Log("Enter Write Dialogue");
            variables.Variables.SetString(TDFConst.nameKey + "0","Name0");
            variables.Variables.SetString(TDFConst.textKey + "0","Hello World");
            variables.Variables.SetBool(TDFConst.next,false);
            variables.Variables.SetBool(TDFConst.writingKey + "0",true);
            await UniTask.WaitUntil(() => variables.Variables.GetBool(TDFConst.writingKey + "0") == false);
            Debug.Log("Enter Next Wait");
            await UniTask.WaitUntil(() => variables.Variables.GetBool(TDFConst.next) == true);
            Debug.Log("Enter Window Close");
            variables.Variables.SetInt(TDFConst.windowKey + "0",3);
            await UniTask.WaitUntil(() => variables.Variables.GetInt(TDFConst.windowKey + "0") == 0);
            Debug.Log("Finish");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            variables.Variables.SetBool(TDFConst.next,true);
        }
    }
}
