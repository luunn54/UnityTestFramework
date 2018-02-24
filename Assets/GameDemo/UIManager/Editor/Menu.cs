using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class Menu
{
    [MenuItem("UIManager/New Screen")]
    private static void NewScreen()
    {
        CreateType(typeof(UI.Screen));
    }

    [MenuItem("UIManager/New Popup")]
    private static void NewPopup()
    {
        CreateType(typeof(UI.Popup));
    }

    [MenuItem("UIManager/New Element")]
    private static void NewElement()
    {
        CreateType(typeof(UI.Element));
    }

    [MenuItem("UIManager/New ScreenTransition")]
    private static void ScreenTransition()
    {
        CreateType(typeof(UI.ScreenTransition));
    }

    [MenuItem("UIManager/New Loading")]
    private static void ScreenLoading()
    {
        CreateType(typeof (UI.Loading));
    }

    private static void CreateType(Type type)
    {
        var typeName = type.Name;

        InputName(string.Format("New {0}", typeName), string.Format("{0} Name:", typeName), name =>
        {
            var gamePath = Path.Combine(Application.dataPath, UIConfig.GamePath);

            var targetScriptDirectory = string.Format("{0}/Script/UI", gamePath);
            Directory.CreateDirectory(targetScriptDirectory);

            var targetScript = string.Format("{0}/{1}.cs", targetScriptDirectory, name);

            if (!File.Exists(targetScript))
            {
                var uicontrollerPath = Directory.GetFiles(Application.dataPath, typeof(UIManager).Name + ".cs", SearchOption.AllDirectories)[0];
                var directory = new FileInfo(uicontrollerPath).Directory;

                if (directory != null)
                {
                    var uicontrollerDirectory = directory.FullName;

                    var templateScript = string.Format("{0}/Template/{1}Template.cs", uicontrollerDirectory, typeName);
                    var reader = File.OpenText(templateScript);
                    var text = reader.ReadToEnd();
                    reader.Close();
                    text = text.Replace(string.Format("{0}Template", typeName), name);
                    File.WriteAllText(targetScript, text);

                    var templatePrefab = string.Format("{0}/Template/{1}Template.prefab", uicontrollerDirectory, typeName);
                    var targetPrefabDirectory = string.Format("{0}/Resources/UI", gamePath);
                    Directory.CreateDirectory(targetPrefabDirectory);
                    var targetPrefab = string.Format("{0}/{1}.prefab", targetPrefabDirectory, name);
                    File.Copy(templatePrefab, targetPrefab, true);

                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("Directory null");
                }
            }
            else
            {
                Debug.LogError("File existed: " + targetScript);
            }
        });
    }

    private static void InputName(string title, string label, Action<string> callback)
    {
        var dialog = ScriptableWizard.DisplayWizard<InputNameWizard>(title);
        dialog.SetLabel(label);
        dialog.SetCallback(callback);
    }
}

public class InputNameWizard : ScriptableWizard
{
    private string _name = string.Empty;
    private string _label = string.Empty;
    private Action<string> _callback;

    private void OnGUI()
    {
        GUILayout.Label(_label);
        _name = EditorGUILayout.TextField(_name);

        if (GUILayout.Button("OK"))
        {
            if (string.IsNullOrEmpty(_name))
            {
                Debug.LogError("Please input name");
            }
            else
            {
                Close();
                _callback.Execute(_name);
            }
        }
    }

    public void SetLabel(string label)
    {
        _label = label;
    }

    public void SetCallback(Action<string> callback)
    {
        _callback = callback;
    }
}