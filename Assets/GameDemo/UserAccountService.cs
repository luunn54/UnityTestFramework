using System.Collections;
using System.Collections.Generic;
using System.IO;
using AppLogEditor;
using UnityEngine;

public class UserAccountService
{
    public class UserAccount
    {
        public string User;
        public string FullName;
        public int Diamond;
    }

    public static UserAccount shareAccount;

    public static void UpdateAccount(Dictionary<string, object> dic)
    {
        if (shareAccount == null)
        {
            shareAccount = new UserAccount();
        }

        shareAccount.FullName = JsonParser.GetString(dic, "name", shareAccount.FullName);
        shareAccount.Diamond = JsonParser.GetInt(dic, "diamond", shareAccount.Diamond);
    }

    public static void Logout()
    {
        shareAccount = null;
    }
}
