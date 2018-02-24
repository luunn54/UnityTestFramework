using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
using UnityEngine.Events;

public class DragAndDrop : MonoBehaviour
{
    public UnityEvent OnStartMove;
    public UnityEvent OnEndMove;
    public UnityEvent OnHold;

    //public float moveSpeed;
    public float offset = 0.05f;
    public float holdOffset = 0.05f;
    public int FrameCheckHold = 60;

    private bool following;
    private bool holding;

    private Vector3 lastPosition;
    private int countFrameSinceLastPosition;

    // Use this for initialization
    void Start()
    {
        following = false;
        //offset += 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (!following && Input.GetMouseButtonDown(0))
        {
            var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = transform.position.z;
            var length = (touchPosition - transform.position).magnitude;

            //Debug.LogError("Length " + length);
            if (length <= offset)
            {
                following = true;
                holding = false;

                lastPosition = transform.position;
                countFrameSinceLastPosition = 0;
                OnStartMove.Invoke();
            }
        }

        if (following)
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = transform.position.z;
            transform.position = position;

            countFrameSinceLastPosition++;
            if (countFrameSinceLastPosition > FrameCheckHold)
            {
                countFrameSinceLastPosition = 0;
                if ((position - lastPosition).magnitude < holdOffset)
                {
                    if (!holding)
                    {
                        holding = true;
                        Debug.LogError("Hold");
                        OnHold.Invoke();
                    }
                }else{
                    if (holding)
                    {
                        OnStartMove.Invoke();
                        holding = false;
                    }
                }

                lastPosition = position;
            }
        }
    }

    void LateUpdate()
    {
        if (following && Input.GetMouseButtonUp(0))
        {
            Debug.LogError("Stop move");
            following = false;
            OnEndMove.Invoke();
        }
    }
}