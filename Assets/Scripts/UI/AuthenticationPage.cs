using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthenticationPage : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalEvents.OnUserLoggedIn += GlobalEvents_OnUserLoggedIn;
        GlobalEvents.OnUserRegistered += GlobalEvents_OnUserRegistered;
        GlobalEvents.OnSignOut += GlobalEvents_OnSignOut;
    }

    private void OnDisable()
    {
        GlobalEvents.OnUserLoggedIn -= GlobalEvents_OnUserLoggedIn;
        GlobalEvents.OnUserRegistered -= GlobalEvents_OnUserRegistered;
    }
    private void OnDestroy()
    {
        GlobalEvents.OnSignOut -= GlobalEvents_OnSignOut;
    }

    private void GlobalEvents_OnSignOut(string signOut)
    {
        gameObject.SetActive(true);
    }
    private void GlobalEvents_OnUserLoggedIn(string username)
    {
        gameObject.SetActive(false);
    }
    private void GlobalEvents_OnUserRegistered(string username)
    {
        gameObject.SetActive(false);
    }
}
