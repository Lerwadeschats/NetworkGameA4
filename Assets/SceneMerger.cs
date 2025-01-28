using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMerger : MonoBehaviour
{
    public static SceneMerger instance;
    public bool IsClient;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);

    }
    
    public string sceneName;

    private void Start()
    {
        if(IsClient) SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    public void MergeScene()
    {
        SceneManager.MergeScenes(SceneManager.GetActiveScene(), SceneManager.GetSceneByName(sceneName));
    }
    public IEnumerator CreateAndMergeSceneServerSide(ulong seed)
    {
        AsyncOperation aop = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        yield return new WaitUntil(() => aop.isDone);
        SceneManager.MergeScenes(SceneManager.GetActiveScene(), SceneManager.GetSceneByName(sceneName));
        DCMapGen.instance.Regenerate(seed);
    }
    

}
