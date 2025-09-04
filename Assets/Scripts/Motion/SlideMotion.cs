using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SlideMotion : MonoBehaviour
{
    [Header("滑铲")]
    public float slideSpeed = 12f;
    public float slideForce = 500f;
    public float slideDuration = .25f;
    public float slideHeight; //滑铲时高度
    public bool isSlide = false;
    private float slideTimer;

    private Rigidbody rb;
    private MovementManager mM;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mM = GetComponent<MovementManager>();
        if(mM == null)
        {
            Debug.LogError("MovementManager组件未找到");
        }
        if(rb == null)
        {
            Debug.LogError("Rigidbody组件未找到");
        }
        slideHeight= .5f * this.transform.localScale.y;
    }
    public void Slide(Vector3 moveDir) //触发滑铲
    {
        transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        isSlide = true;
        mM.currentSpeed= slideSpeed;
        slideTimer = slideDuration;
        StartCoroutine(SlideHandler(moveDir));
    }
    private IEnumerator SlideHandler(Vector3 slideDir)
    {
        rb.AddForce(slideDir.normalized * slideForce, ForceMode.Force);
        //yield return new WaitForSeconds(slideDuration);
        yield return new WaitUntil(() => //等待条件
        {
            slideTimer = SlideDelay(slideTimer);
            return (slideTimer < 0 || mM.isJump);
        });
        transform.localScale = new Vector3(transform.localScale.x, mM.playerHeight, transform.localScale.z);
        isSlide = false;
    }
    private float SlideDelay(float timer)
    {
        return timer -= Time.deltaTime;
    }

}
