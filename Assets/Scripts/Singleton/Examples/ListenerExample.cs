using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Singleton.Example
{
    public class ListenerExample : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("Subscribed");
            EventExample.Instance.ActionExample += MyAction;
            //example.ActionExample = example.ActionExample + MyAction;
            EventExample.Instance.ActionExample += MyOtherAction;

        }

        private void OnInventoryClosed(int myFirstParam, int mySecondParam)
        {
            Debug.Log($"My params: {myFirstParam} {mySecondParam}");
        }

        private void OnDisable()
        {
            Debug.Log("Unsubscribed.");
            EventExample.Instance.ActionExample -= MyAction;
            EventExample.Instance.ActionExample -= MyOtherAction;
        }
        private void MyAction(int x)
        {
            Debug.Log("Action performed");
        }
        private void MyOtherAction(int x)
        {
            Debug.Log("Other Action performed");
        }
    }
}