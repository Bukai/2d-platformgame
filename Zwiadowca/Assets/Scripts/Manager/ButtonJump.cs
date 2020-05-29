using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonJump : MonoBehaviour
{
    public void onPioterDown()
    {
        PlayerController.MyInstance.JumpDown();
    }

    public void onPointerUp()
    {
        PlayerController.MyInstance.JumpUp();
    }
}
