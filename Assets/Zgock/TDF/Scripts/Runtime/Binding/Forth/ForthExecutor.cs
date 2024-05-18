using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TotalDialogue.Core.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace TotalDialogue
{
    public class ForthExecutor : MonoBehaviour
    {
        public TDFDriver driver;
        public List<TextAsset> preExecuteFiles;
        public string runOnAwake;
        public string runOnSceneLoaded;
        public string runOnStart;
        protected TDFForth forth = null;
        void Awake()
        {
            //await driver.WaitUntilReady();
            forth = new TDFForth(driver);
            foreach (var file in preExecuteFiles)
            {
                forth.Execute(file.text).Forget();
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (!string.IsNullOrEmpty(runOnAwake))
            {
                Execute(runOnAwake);
            }
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //await driver.WaitUntilReady();
            if (!string.IsNullOrEmpty(runOnSceneLoaded))
            {
                Execute(runOnSceneLoaded);
            }
        }
        void Start()
        {
            if (!string.IsNullOrEmpty(runOnStart))
            {
                Execute(runOnStart);
            }
        }
        public async UniTask ExecuteUniTask(string command)
        {
            if (forth != null)
            {
                await UniTask.SwitchToMainThread();
                await forth.Execute(command);
            }
        }
        public void Execute(string command) => UniTask.Void(async () =>
        {
            if (forth != null)
            {
                await UniTask.SwitchToMainThread();
                await forth.Execute(command);
            }
        });
    }
}
