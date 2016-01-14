using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class qd : MonoBehaviour {

	//public GameObject empty_qd;
	public Text score_txt;
	public static int x;
	public static int y;
	void OnMouseDown()
	{

		float a =transform.parent.gameObject.transform.position.y;
        
		if (a == -0.45f) 
		{
            

			x=1;
			y=0;
			//Debug.Log ("happy");
		}
		else

		{
			y=2;
			x=0;
		//	Debug.Log ("sad");

		}
	}
}