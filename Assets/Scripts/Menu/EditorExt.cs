#if UNITY_EDITOR

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class EditorExt
{
    public static async Task<string> PostFormUrlEncoded(this HttpWebRequest request, string data) => await request.Post("application/x-www-form-urlencoded", data);
    public static async Task<string> Post(this HttpWebRequest request, string contentType, string data)
    {
        request.Method = "POST";
        request.ContentType = contentType;
        using (var stream = request.GetRequestStream())
            stream.Write(Encoding.UTF8.GetBytes(data));

        using (var response = await request.GetResponseAsync())
        {
            using (var stream = response.GetResponseStream())
                return stream.ReadToEnd(Encoding.UTF8);
        }
    }

    public static string ReadToEnd(this Stream stream, Encoding encoding = null) => new StreamReader(stream, encoding ?? Encoding.UTF8).ReadToEnd();
    public static void Write(this Stream stream, byte[] data) => stream.Write(data, 0, data.Length);

    public static Dictionary<string, string> ParseQueryString(this Uri uri) => uri?.Query?.Split('?', '&').Select((pair) => {
        return pair.Split(new[] { '=' }, 2);
    }).Where((pair) => !string.IsNullOrEmpty(pair.FirstOrDefault())).ToDictionary((pair) => pair.FirstOrDefault(), (pair) => pair.ElementAtOrDefault(1));
}

#endif