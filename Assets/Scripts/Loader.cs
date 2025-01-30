using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public static Loader Instance { get; private set; }

    [SerializeField] private Animator tempAnimator;
    private float transitionTime = 3f;
    private int SCENE_COUNT;

    //private Dictionary<Scene, int> sceneIndices = new Dictionary<Scene, int>();

    //public enum Scene // Is it necessary when scenes' path is already calculated?
    //{
    //    Tavern,
    //    Dungeon,
    //    WitchLayer
    //}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SCENE_COUNT = SceneManager.sceneCountInBuildSettings;

        //CacheSceneIndices();
    }

    //private void CacheSceneIndices()
    //{
    //    foreach (Scene scene in Enum.GetValues(typeof(Scene)))
    //    {
    //        int index = SceneUtility.GetBuildIndexByScenePath(scene.ToString());
    //        if (index != -1)
    //        {
    //            sceneIndices[scene] = index;
    //        }
    //        else
    //        {
    //            Debug.LogError($"Scene '{scene}' not found in build settings.");
    //        }
    //    }

    //    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
    //    {
    //        if (i != -1)
    //        {
    //            sceneIndices
    //        }
    //    }
    //}

    public void PerformSceneTransition()
    {
        // This method performs logical transitions between scenes.
        // I.e. it performs following one-way transitions: Tavern -> Dungeon -> WitchLayer.

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex < SCENE_COUNT - 1)
        {
            Debug.Log("Changing scene...");
            StartCoroutine(LoadSceneAsync(++buildIndex));
        }

        Debug.Log("Currently this is the final scene!");
    }

    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        tempAnimator.SetTrigger("Crossfade");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(buildIndex);
    }
}
