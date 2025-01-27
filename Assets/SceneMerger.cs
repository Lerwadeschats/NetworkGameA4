using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMerger : MonoBehaviour
{
    public static SceneMerger instance;
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
        SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
    }
    public void MergeScene()
    {
        SceneManager.MergeScenes(SceneManager.GetActiveScene(), SceneManager.GetSceneByName(sceneName));  
    }
}
