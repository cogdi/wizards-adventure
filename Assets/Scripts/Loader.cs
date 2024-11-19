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
    
    private Dictionary<Scene, int> sceneIndices = new Dictionary<Scene, int>();

    public enum Scene
    {
        Tavern,
        Dungeon,
        WitchLayer
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        CacheSceneIndices();
    }

    private void CacheSceneIndices()
    {
        foreach (Scene scene in Enum.GetValues(typeof(Scene)))
        {
            int index = SceneUtility.GetBuildIndexByScenePath(scene.ToString());
            if (index != -1)
            {
                sceneIndices[scene] = index;
            }
            else
            {
                Debug.LogError($"Scene '{scene}' not found in build settings.");
            }
        }
    }

    public void PerformSceneTransition()
    {
        // This method performs logical transitions between scenes.
        // I.e. it performs following one-way transitions: Tavern -> Dungeon -> WitchLayer.

        //string currentScene = SceneManager.GetActiveScene().ToString();
        //if (currentScene.Equals(Scene.Tavern.ToString()))
        //{
        //    StartCoroutine(LoadSceneAsync(sceneIndices[Scene.Dungeon]));
        //}

        //else if (currentScene.Equals(Scene.Dungeon.ToString()))
        //{
        //    StartCoroutine(LoadSceneAsync(sceneIndices[Scene.WitchLayer]));
        //}

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex < 2)
        {
            StartCoroutine(LoadSceneAsync(buildIndex + 1));
        }
    }

    //public void LoadScene(Scene scene)
    //{
    //    StartCoroutine(LoadSceneAsync(sceneIndices[scene]));
    //}

    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        tempAnimator.SetTrigger("Crossfade");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(buildIndex);
    }
}
