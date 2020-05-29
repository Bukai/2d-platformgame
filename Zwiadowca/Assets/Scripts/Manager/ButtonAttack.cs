using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAttack : MonoBehaviour
{
    public void onClick()
    {
        PlayerAttack.MyInstance.ButtonClick();
    }
}
