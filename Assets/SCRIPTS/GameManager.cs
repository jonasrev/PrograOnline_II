using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] dontDestroy;

    private void Awake()
    {
        foreach (GameObject go in dontDestroy)
        {
            DontDestroyOnLoad(go);
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseScene(int sceneNum)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(sceneNum);

        //Debug.Log($"Escena: {scene.name}");
        //Debug.Log($"Está cargada: {scene.isLoaded}");

        //if (!scene.isLoaded)
        //{
        //    Debug.LogWarning($"La escena {sceneNum} no está cargada.");
        //    return;
        //}

        SceneManager.UnloadSceneAsync(sceneNum);
    }
}
