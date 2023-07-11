using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public int songIndex;
    public List<string> musicDatabase = new List<string>();

    public FMODUnity.EventReference Event;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadYourAsyncScene(scene));
    }

    public IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        FMODUnity.RuntimeManager.PlayOneShot(Event, transform.position);
        yield return new WaitForSeconds(6f);

        // switch scenes
        GameValues.songIndex = songIndex;
        GameValues.songName = musicDatabase[songIndex];
        asyncLoad.allowSceneActivation = true;
    }
}
