using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace sapra.silvercoin_project
{
    public class FinalManager : MonoBehaviour
    {
        public TextMeshProUGUI timer;
        private void Start() {
            if(MenuManager.instance != null)
                timer.SetText(MenuManager.instance.getTime().ToString());    
        }
    }
}