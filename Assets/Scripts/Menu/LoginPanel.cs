using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class LoginPanel : MenuPane
    {
        public TMP_Text connectionErrorText;
        public Button retryConnectButton;
        public Button signInButton;
        public StartMenu start;

        public override void Show()
        {
            base.Show();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("no internet");
                connectionErrorText.text = "No internet connection detected";
                retryConnectButton.gameObject.SetActive(true);
                signInButton.gameObject.SetActive(false);
            }
            else
            {
                connectionErrorText.text = "Sign in with a Google account to play!";
                retryConnectButton.gameObject.SetActive(false);
                signInButton.gameObject.SetActive(true);
                start.playerSave.FirebaseManager.InitFirebase();
            }
        }

        public void RetryInternetConnectivity()
        {
            Show();
        }

        public void SignIn()
        {
            start.SignInUser();
        }

        public void Offline()
        {
            start.PlayOffline();
        }

        public void PrivacyPolicy()
        {
            Application.OpenURL("https://github.com/Perwahl/DGVPrivacyPolicy/blob/master/README.md");
        }

        public override void Back()
        {
            Debug.Log("quit");
            Application.Quit();
        }
    }
}