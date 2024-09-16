using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRobot : MonoBehaviour
{
    [SerializeField]
    List<AnimateToTarget> hands = new();

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    HideEyes();
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    ShowEyes();
        //}
    }

    public void HideEyes()
    {
        hands.ForEach(h => h.MoveToTarget());
    }

    public void ShowEyes()
    {
        hands.ForEach(h => h.MoveToDefault());
    }
}
