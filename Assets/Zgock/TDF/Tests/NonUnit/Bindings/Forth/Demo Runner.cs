using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TotalDialogue;
using UnityEngine;

public class DemoRunner : MonoBehaviour
{
    ForthExecutor executor;
    public GameObject StartButton;
    public string command;

    public void Demo() => UniTask.Void(async () =>
    {
        StartButton.SetActive(false);
        await executor.ExecuteUniTask(command);
        StartButton.SetActive(true);
    });
    // Start is called before the first frame update
    void Start()
    {
        executor = GetComponent<ForthExecutor>();
        StartButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
