using System.Collections;
using UnityEngine;

public class LoadingMenu : MonoBehaviour
{
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
        yield return new WaitForSeconds(4f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map01");
    }
}
