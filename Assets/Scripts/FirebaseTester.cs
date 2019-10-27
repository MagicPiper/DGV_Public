using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTester : MonoBehaviour
{
    private bool firebaseInitiated;

    public FirebaseDatabase FBdata { get; private set; }

    private FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        InitFirebase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.
                firebaseInitiated = true;
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://discgolfvalley.firebaseio.com/");
                FBdata = FirebaseDatabase.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError(String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.

                // Get the root reference location of the database.                  
            }
        });
    }
}
