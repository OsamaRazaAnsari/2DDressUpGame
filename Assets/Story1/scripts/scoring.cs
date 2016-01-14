using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class scoring : MonoBehaviour
{
    
    public Text sc;
    int getscore;
    bool flag;

    // Update is called once per frame

    void Start() {
        //flag = true;
        //Debug.Log("start");
       // qd.x = 0;
       // qd.y = 0;
       sc.text= "LovePoints:0" ;
        //PlayerPrefs.DeleteAll();
    }
    void Update()
    {
        if (Application.loadedLevelName == "Story1")
        {
            if (qd.x == 1)
            {
              //  Debug.Log("happy");
                //getscore = 5;
               // if (!flag)
              //  {
                 //   Debug.Log("happyscoreupdate");
                    getscore = 5;
                    
                  //  flag = true;
             //   }
                    sc.text = "LovePoints:" + (getscore);

            } if(qd.y==2)
            {
                //      //getscore = 5;

                //getscore = getscore - 5;
                // if (!flag)
                // {
               // Debug.Log("sadscoreupdate");
                getscore = -5;
                //    flag = true;
                //  }
                sc.text = "LovePoints:" + (getscore);

            }
             
           PlayerPrefs.SetInt("getscore", getscore);
            }

        if (Application.loadedLevelName == "story2") {

            //Debug.Log(PlayerPrefs.GetInt("score"));
            int scene1score = PlayerPrefs.GetInt("getscore");
            int totalscore=PlayerPrefs.GetInt("score");

            
            int i=totalscore;
          //  Debug.Log(i);
            
            if (totalscore < 7)
            {
                totalscore= 5;
               // Debug.Log("The girl will say Nice Clothes");
                GameObject.Find("msg").GetComponent<Text>().text = "Nice Clothes";
                

            }
            if(totalscore>=7 && totalscore<=9)
            {
               // Debug.Log(true);
                totalscore =  10;
              //  Debug.Log("The girl will say very Nice clothes");
                GameObject.Find("msg").GetComponent<Text>().text = " Very Nice Clothes";
            }
            if (totalscore > 9) 
            {

                totalscore = 15;
               // Debug.Log("The girl say you look fabulous");
                GameObject.Find("msg").GetComponent<Text>().text = " You look fabulous";
            }


            sc.text = "LovePoints:" + (scene1score + totalscore);
        
        }



    }
    
}
