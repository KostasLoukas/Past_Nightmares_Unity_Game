using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsControl : MonoBehaviour
{
    public string mainMenuScene;
    [SerializeField] private GameObject returningToMainMenuWarningText;
    private Animator creditsRollingAnimator;




    void Awake()
    {
        creditsRollingAnimator = GameObject.FindGameObjectWithTag("Credits").GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
        returningToMainMenuWarningText.SetActive(false);
        StartCoroutine(WaitForCreditsToEnd());
    }


    IEnumerator WaitForCreditsToEnd()
    {
        AnimationClip[] clips = creditsRollingAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(clips[0].length);

        returningToMainMenuWarningText.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync(mainMenuScene);
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuScene);
    }
    
}
