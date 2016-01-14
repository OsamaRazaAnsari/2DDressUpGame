using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class my_real_time : MonoBehaviour {
	public Text txt;

	// Update is called once per frame
	void Update () {
		string text = System.DateTime.Now.ToString("HH:mm:ss");
		txt.text = text;
	}
}
