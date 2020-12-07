using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public void doExitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}

