using System;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainMenuScript : MonoBehaviour
    {
        public MenuState CurrentState;
        private static GameObject _soundButton;
        private static GameObject _mainCamera;

        void Awake()
        {
            _mainCamera = GameObject.Find("Main Camera");
            var fg = GameObject.Find("/GUI");
            var statsButton = GameObject.Find("/GUI/StatsButton");
            _soundButton = GenerateMenuButton("Prefabs/SoundButton", fg.transform, Vector3.one, new Vector3(statsButton.transform.localPosition.x + 120,
                statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), null, 0, OnSoundButtonPressed);
            _soundButton.GetComponent<Image>().sprite = SoundEnabled
                ? Resources.LoadAll<Sprite>("SD/StatisticHeader")[16]
                : Resources.LoadAll<Sprite>("SD/StatisticHeader")[17];
            if (SoundEnabled)      
                _mainCamera.GetComponent<AudioSource>().Play();


        }

        public bool SoundEnabled
        {
            get
            {
                return GeneralSettings.SoundEnabled;
            }
            set
            {
                GeneralSettings.SoundEnabled = value;
                if (value)
                    _mainCamera.GetComponent<AudioSource>().Play();
                else
                    _mainCamera.GetComponent<AudioSource>().Pause();
            }
        }

        public void OnSoundButtonPressed()
        {
            Vibration.Vibrate();

            SoundEnabled = !SoundEnabled;

            _soundButton.GetComponent<Image>().sprite = SoundEnabled
                ? Resources.LoadAll<Sprite>("SD/StatisticHeader")[16]
                : Resources.LoadAll<Sprite>("SD/StatisticHeader")[17];
        }

        public void OnNavigationButtonClick(String scene)
        {
            Vibration.Vibrate();

            var gui = GameObject.Find("/GUI");
            var collection = gui.GetComponentsInChildren<Transform>();
            foreach (var component in collection)
            {
                if (gui != component.gameObject)
                    Destroy(component.gameObject);
            }
            var label = Instantiate(Resources.Load("Prefabs/LoadingLabel")) as GameObject;
            label.transform.SetParent(gui.transform);
            label.transform.localPosition = Vector3.zero;
            label.transform.localScale = Vector3.one;
            label.GetComponent<Text>().text = LanguageManager.Instance.GetTextValue("LoadingTitle");

            Application.LoadLevel(scene);
        }

        public static GameObject GenerateMenuButton(String prefabName, Transform parentTransform, Vector3 localScale, Vector3 localPosition, String buttonText
            , int textSize, UnityAction onClickCall, Color? buttonColor = null)
        {
            var button = Instantiate(Resources.Load(prefabName)) as GameObject;
            var rectTransform = button.transform as RectTransform;
            rectTransform.SetParent(parentTransform);
            rectTransform.localScale = localScale;
            rectTransform.localPosition = localPosition;

            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                if (onClickCall != null)
                    buttonComponent.onClick.AddListener(onClickCall);
                else
                    buttonComponent.interactable = false;
            }


            if (String.IsNullOrEmpty(buttonText)) return button;

            var textComponent = button.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = buttonText;
                textComponent.fontSize = textSize;
                if (buttonColor.HasValue) textComponent.color = buttonColor.Value;
            }

            return button;
        }
    }
}
