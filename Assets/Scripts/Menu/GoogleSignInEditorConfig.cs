
#if UNITY_EDITOR

namespace Google
{
    public static class GoogleSignInEditorConfig
    {
        public static string Secret;
    }
}

namespace Google.Impl
{
    using System;
    using System.Linq;
    using System.Text;

    using System.Net;
    using System.Net.NetworkInformation;

    using UnityEngine;

    using Newtonsoft.Json.Linq;

    internal class GoogleSignInImplEditor : ISignInImpl, FutureAPIImpl<GoogleSignInUser>
    {
        GoogleSignInConfiguration configuration;

        public bool Pending { get; private set; }

        public GoogleSignInStatusCode Status { get; private set; }

        public GoogleSignInUser Result { get; private set; }

        public GoogleSignInImplEditor(GoogleSignInConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void EnableDebugLogging(bool flag)
        {
            throw new NotImplementedException();
        }

        public Future<GoogleSignInUser> SignIn()
        {
            SigningIn();
            return new Future<GoogleSignInUser>(this);
        }

        public Future<GoogleSignInUser> SignInSilently()
        {
            SigningIn();
            return new Future<GoogleSignInUser>(this);
        }

        public void SignOut()
        {
            throw new NotImplementedException();
        }

        static HttpListener BindLocalHostFirstAvailablePort()
        {
            ushort minPort = 49215;
            var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            return Enumerable.Range(minPort, ushort.MaxValue - minPort).Where((i) => !listeners.Any((x) => x.Port == i)).Select((port) => {
                try
                {
                    var listener = new HttpListener();
                    listener.Prefixes.Add($"http://localhost:{port}/");
                    listener.Start();
                    return listener;
                }
                catch
                {
                    return null;
                }
            }).FirstOrDefault((listener) => listener != null);
        }

        void SigningIn()
        {
            Pending = true;
            var httpListener = BindLocalHostFirstAvailablePort();
            Application.OpenURL("https://accounts.google.com/o/oauth2/v2/auth?scope=email%20profile&response_type=code&redirect_uri=" + httpListener.Prefixes.FirstOrDefault() + "&client_id=" + configuration.WebClientId);
            httpListener.GetContextAsync().ContinueWith((task) => {
                var context = task.Result;

                string code;
                if (!context.Request.Url.ParseQueryString().TryGetValue("code", out code) || string.IsNullOrEmpty(code))
                {
                    Pending = false;
                    Status = GoogleSignInStatusCode.InvalidAccount;
                    Debug.Log("no code?");
                    return;
                }

                context.Response.StatusCode = 200;
                context.Response.OutputStream.Write(Encoding.UTF8.GetBytes("Can close this page"));
                context.Response.Close();

                HttpWebRequest.CreateHttp("https://www.googleapis.com/oauth2/v4/token").PostFormUrlEncoded("code=" + code + "&client_id=" + configuration.WebClientId + "&client_secret=" + GoogleSignInEditorConfig.Secret + "&redirect_uri=" + httpListener.Prefixes.FirstOrDefault() + "&grant_type=authorization_code").ContinueWith((dataTask) => {
                    var jobj = JObject.Parse(dataTask.Result);
                    Result = new GoogleSignInUser()
                    {
                        IdToken = (string)jobj.GetValue("id_token")
                    };

                    Status = GoogleSignInStatusCode.Success;
                    Pending = false;
                });
            });
        }
    }
}
#endif