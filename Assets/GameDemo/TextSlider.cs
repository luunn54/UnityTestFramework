using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    public Slider slider;
    private Text text;

	// Use this for initialization
	void Start ()
	{
	    text = GetComponent<Text>();
	    OnValueChanged(slider.value);

	    slider.onValueChanged.AddListener(OnValueChanged);
	}

    private void OnValueChanged(float value)
    {
        text.text = slider.value.ToString();
    }
}
