using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TotalDialogue;
using UnityEngine;

public class DriverTest : MonoBehaviour
{
    TDFDriver driver;
    // Start is called before the first frame update
    void Start()
    {
        driver = GetComponent<TDFDriver>();
        Test().Forget();
/*
        if (driver != null)
        {
            Debug.Log("DriverTest");
            StartCoroutine(WaitForReadyAndTest());
        }
*/
    }

    private IEnumerator WaitForReadyAndTest()
    {
        // 他のMonoBehaviourがReadyになるまで待つ
        yield return new WaitUntil(() => driver.Ready/* 他のMonoBehaviourがReadyになったかどうかをチェックする条件式 */);

        // すべてのMonoBehaviourがReadyになったら、非同期処理を開始する
        Test().Forget();
    }
    public void StartTest(){
        Test().Forget();
    }
    async UniTaskVoid Test(){
        driver = GetComponent<TDFDriver>();
        for(int id =  0;;id = (id + 1) % 2){
            Debug.Log("Test Started");
            await UniTask.WaitForSeconds(0.5f);
            Debug.Log("Enter Window Open");
            await driver.OpenWindow(id:id);
            Debug.Log("Enter Write Dialogue");
            await driver.WriteDialogue("Name0", "Hello World", true, true, true, false,id:id);
            Debug.Log("Enter Window Close");
            await driver.CloseWindow(id:id);
            Debug.Log("Finish");
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            driver.Variables.SetBool(TDFConst.next,true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            driver.Variables.SetBool(TDFConst.next,false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            driver.Variables.SetBool(TDFConst.cancel,true);
        }
        */
    }
}
