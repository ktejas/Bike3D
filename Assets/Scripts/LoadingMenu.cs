using System.Collections;
using UnityEngine;

public class LoadingMenu : MonoBehaviour
{

    public float loadingTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMenuScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(loadingTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map01");
    }
}
