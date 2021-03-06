using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace sapra.silvercoin_project
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
        public Slider horiSlider;
        public Slider verticalSlider;
        private float horizontalSens;
        private float verticalSens;
        private float Time;

        public float getTime()
        {
            return Time;
        }
        public float getHori()
        {
            return horizontalSens;
        }
        public float getVert()
        {
            return verticalSens;
        }

        void Start()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
                Destroy(this.gameObject);

            onHorizontalChange();
            onVerticalChange();
        }
        
        public void OnPlay()
        {
            SceneManager.LoadScene("World");
        }
        public void OnExit()
        {
            Application.Quit();
        }
        public void onHorizontalChange()
        {
            horizontalSens = horiSlider.value*30;
        }
        public void onVerticalChange()
        {
            verticalSens = verticalSlider.value*30;
        }
        public void OnComplete(float time)
        {
            this.Time = time;
            SceneManager.LoadScene("LevelComplete");
        }
    }
}
