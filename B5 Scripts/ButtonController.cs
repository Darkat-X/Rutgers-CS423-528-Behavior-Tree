using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonController : MonoBehaviour
{
    private void Start()
    {
        
    }
    public void SetTrue()
    {
        GameObject.Find("Updater").GetComponent<MyBehaviorTree>().feedback = true;
    }

    public void SetFalse()
    {
        GameObject.Find("Updater").GetComponent<MyBehaviorTree>().feedback = false;
    }
}
