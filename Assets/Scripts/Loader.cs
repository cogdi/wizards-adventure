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
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SCENE_COUNT = SceneManager.sceneCountInBuildSettings;
    }

    public void PerformSceneTransition()
    {
        /* This method performs one-way transitions between scenes in the following order:
           Tavern -> Dungeon -> WitchLayer. */

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
