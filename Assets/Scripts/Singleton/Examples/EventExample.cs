using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Singleton.Example
{
    public class EventExample : SingletonBehaviour<EventExample>
    {
        public Action<int> ActionExample;

        public delegate void InventoryClosed();
        public event InventoryClosed OnInventoryClosed;



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ActionExample?.Invoke(5);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                OnInventoryClosed?.Invoke();
            }
        }
    }
}
