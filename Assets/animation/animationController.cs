using UnityEngine;

public class animationController : MonoBehaviour
{
    private Animator mAnimator;
    bool mStarted = true;
    void Start()
    {
        mAnimator = GetComponent<Animator>();   
    }

    public void Open() {

        mAnimator.SetTrigger("open");
        mStarted = true;
    }
    public void Close() {

        mAnimator.ResetTrigger("open");
        mStarted = false;
    }



    public void toggle() { 
    
        if (mStarted)
            Close();
        else
            Open();
    
    }
}
