using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Finish_Button : MonoBehaviour {
	//UtageUguiTitle nextscence;
	//UtageUguiTitle obj;
	// Use this for initialization
	void Start () {
	//	 obj=nextscence.GetComponent<UtageUguiTitle> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Finishbutton(){

        
		Application.LoadLevel("story2");

}
}
