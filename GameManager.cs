using UnityEngine;

namespace ToExport.Scripts
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    
    }
}
