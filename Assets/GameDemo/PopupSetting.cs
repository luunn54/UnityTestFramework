using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : UI.Popup
{

    public Slider sliderVolume;
    public Toggle togglePush;

	// Use this for initialization
	public void OnSave () {

        var setting = GameSettingService.CurrentGameSetting;
        setting.Volume = (int)sliderVolume.value;
	    setting.Push = togglePush.isOn;

        GameSettingService.SaveGameSetting();

		Close();
	}

    public override void OnActive(object data)
    {
        base.OnActive(data);
        var setting = GameSettingService.CurrentGameSetting;
        sliderVolume.value = setting.Volume;
        togglePush.isOn = setting.Push;
    }

    // Update is called once per frame
	void Update () {
		
	}
}
