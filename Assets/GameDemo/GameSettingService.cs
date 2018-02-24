using System.Collections;
using System.Collections.Generic;
using System.IO;
using AppLogEditor;
using UnityEngine;

public class GameSettingService
{
    public class GameSetting
    {
        public int Volume;
        public bool Push;
    }

    private static GameSetting shareSetting;

    public static GameSetting CurrentGameSetting 
    {
        get
        {
            if (shareSetting == null)
            {
                LoadGameSetting();
            }

            return shareSetting;
        }
    }

    private static string PathSetting
    {
        get { return Path.Combine(Application.persistentDataPath, "Setting.json"); }
    }
    private static void LoadGameSetting()
    {
        shareSetting = new GameSetting()
        {
            Volume = 2,
            Push = true
        };

        if (File.Exists(PathSetting))
        {
            var content = File.ReadAllText(PathSetting);
            var dic = Json.Deserialize(content) as Dictionary<string, object>;

            shareSetting.Volume = JsonParser.GetInt(dic, "Volume", 2);
            shareSetting.Push = JsonParser.GetBool(dic, "Push", true);
        }
    }

    public static void SaveGameSetting()
    {
        var dic = new Dictionary<string, object>()
        {
            {"Volume", shareSetting.Volume},
            {"Push", shareSetting.Push},
        };

        File.WriteAllText(PathSetting, Json.Serialize(dic));
    }
}
