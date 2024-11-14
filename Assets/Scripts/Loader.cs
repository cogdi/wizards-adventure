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

    public void LoadScene(Scene scene)
    {
        StartCoroutine(LoadSceneAsync(sceneIndices[scene]));
    }

    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        tempAnimator.SetTrigger("Crossfade");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(buildIndex);
    }
}
