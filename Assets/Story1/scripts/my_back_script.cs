using UnityEngine;
using System.Collections;

public class my_back_script : MonoBehaviour {

	public GameObject txt;
	public GameObject score;
    public GameObject token;

	public void my_back()
	{
		txt.SetActive (true);
		score.SetActive (true);
        token.SetActive(true);
        
    }
}
