using System;
using ToExport.Scripts.Core;
using UnityEngine;

namespace Player
{
    public class InputManager : Singleton<InputManager>
    {
        public static event Action OnReloadKeyDown;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnReloadKeyDown?.Invoke();
            }
        }
    }
}