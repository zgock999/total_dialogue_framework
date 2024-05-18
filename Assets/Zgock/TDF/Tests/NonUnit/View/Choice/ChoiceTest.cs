using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TotalDialogue;
using UnityEngine;

public class ChoiceTest : MonoBehaviour
{
    
    TDFDriver driver;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ChoiceTest");
        driver = GetComponent<TDFDriver>();
        Test().Forget();
    }
    async UniTaskVoid Test(){
        Debug.Log("Test Started");
        for (;;){
            await UniTask.WaitForSeconds(0.5f);
            await driver.OpenWindow(id:0);
            await driver.WriteDialogue("Name0", "Which do you choice?", false, false, false, true,id:0);
            await driver.Choice("aaaa,bbbb,cccc", id:0);
            int selection = driver.Variables.GetInt("_Choice0");
            switch (selection)
            {
                case 0:
                    await driver.WriteDialogue("Name0", "You choose aaaa", true, true, true, false,id:0);
                    break;
                case 1:
                    await driver.WriteDialogue("Name0", "You choose bbbb", true, true, true, false,id:0);
                    break;
                case 2:
                    await driver.WriteDialogue("Name0", "You choose cccc", true, true, true, false,id:0);
                    break;
                default:
                    await driver.WriteDialogue("Name0", "You choose nothing", true, true, true, false,id:0);
                    break;
            }
            await driver.WriteDialogue("Name0", "Which do you choice(Cancelable)?", false, false, false, true,id:0);
            await driver.Choice("aaaa,bbbb,cccc", id:0,cancelable:true);
            selection = driver.Variables.GetInt("_Choice0");
            switch (selection)
            {
                case 0:
                    await driver.WriteDialogue("Name0", "You choose aaaa", true, true, true, false,id:0);
                    break;
                case 1:
                    await driver.WriteDialogue("Name0", "You choose bbbb", true, true, true, false,id:0);
                    break;
                case 2:
                    await driver.WriteDialogue("Name0", "You choose cccc", true, true, true, false,id:0);
                    break;
                default:
                    await driver.WriteDialogue("Name0", "You choose nothing", true, true, true, false,id:0);
                    break;
            }
            await driver.WriteDialogue("Name0", "Which do you choice(Multiple)?", false, false, false, true,id:0);
            driver.Variables.SetString("_Chooser2","dddd,eeee,ffff");
            driver.Variables.SetString("_Chooser3","ggggg,hhhhh,iiiii");
            await driver.Choice("aaaa,bbbb,cccc", id:1,depth:2);
            int choice1 = driver.Variables.GetInt("_Choice1");
            int choice2 = driver.Variables.GetInt("_Choice2");
            int choice3 = driver.Variables.GetInt("_Choice3");
            await driver.WriteDialogue("Name0", $"Your choice is ({choice1},{choice2},{choice3})", true, true, true, false,id:0);

            await driver.CloseWindow(id:0);
        }
        //Debug.Log("Finish");
    }
}
