using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppLogEditor;
using TestFramework;
using UnityEngine;

public class HttpService
{
    public static IEnumerator HTTPPost(string url, Dictionary<string, object> body,
        Action<Dictionary<string, object>> OnSuccess, Action<int, string> OnError)
    {
        var bodyString = Json.Serialize(body);
        var fullUrl = "http://127.0.0.1:8080" + url;
        //var www = new WWW(fullUrl, Encoding.ASCII.GetBytes(bodyString), new Dictionary<string, string>());
        var www = AutoWWW.Create(fullUrl, Encoding.ASCII.GetBytes(bodyString), new Dictionary<string, string>());

        Debug.Log(fullUrl + "->" + bodyString);

        while (!www.isDone)
        {
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(fullUrl + "->" + www.error);
            OnError(404, www.error);
        }
        else
        {
            var response = www.text;
            Debug.LogError(fullUrl + "->" + response);
            var dic = Json.Deserialize(response) as Dictionary<string, object>;

            var status = JsonParser.GetInt(dic, "status", 200);
            var data = JsonParser.GetDict(dic, "data", new Dictionary<string, object>());
            if (status == 200)
            {
                OnSuccess(data);
            }
            else
            {
                OnError(status, JsonParser.GetString(data, "msg", "Unknow Error!"));
            }
        }
    }
}
