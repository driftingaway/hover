using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public AudioSource a;
    public AudioClip[] clip;
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
        
        a.PlayOneShot(clip[0]);
        yield return new WaitForSeconds(1.0f);
        a.PlayOneShot(clip[1]);
        yield return new WaitForSeconds(1.0f);
        a.PlayOneShot(clip[2]);
        yield return new WaitForSeconds(1.0f);
        a.PlayOneShot(clip[3]);
        yield return new WaitForSeconds(1.0f);
        a.PlayOneShot(clip[4]);
        yield return new WaitForSeconds(1.0f);
        a.PlayOneShot(clip[5]);
        yield return new WaitForSeconds(0.5f);
        a.PlayOneShot(clip[6]);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo2", true);
        yield return new WaitForSeconds(0.5f);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo2", false);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo", true);
        yield return new WaitForSeconds(0.5f);

        // switch scenes
        asyncLoad.allowSceneActivation = true;
    }
}
