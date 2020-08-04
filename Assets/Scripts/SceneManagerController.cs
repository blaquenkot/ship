using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerController: MonoBehaviour
{
    private Animator Animator;
    private int CurrentSceneIndex = 0;
    
    void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void GoToNexScene()
    {
        if(CurrentSceneIndex < SceneManager.sceneCount)
        {
            CurrentSceneIndex += 1;
            Animator.SetTrigger("FadeOut");
        }
    }

    public void OnFadeOutFinished()
    {
        SceneManager.LoadScene(CurrentSceneIndex);
    }    
}