using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class CustomManager : MonoBehaviour {



	//Please note - this is a starter kit for dressup games.
	//There are two main scripts - CustomManager.cs and CustomItem.cs
	//To handle the GUI/behavior/generation of objects - edit CustomManager.cs
	//To change the price of objects, handle how many objects are unlocked, edit CustomItem.cs

	public List<Sprite> shirtslist = new List<Sprite> ();
	public List<Sprite > pentslist = new List<Sprite> ();
	int  scores;
	int shirtscore=0;
	int pentscore=0;
    object obj;
    int total;
    Object OSAMA;
	//Declaring Images of Main Menu Buttons
	public Sprite imgOnButton;
	public Sprite imgOffButton;

	//Panels of Buy One and Buy All Items
	public GameObject buyScreenOne;
	public GameObject buyScreenAll;

	//Coins Text - array because it can be on more than one panel
	public Text[] lblCoins;

	//Very important - currentScreen string is used to determine which screen is on top of the screen - don't enable the GameScreen in the start.
	//Always start the game with MainScreen enabled and all other disabled
	public string currentScreen;


	//Default Item - to be used for every item in the menu
	public GameObject defaultItem;

	//A string to store which menu is selected at the time
	string selectedMenu;

	//A simple text label in the buy one screen to inform the user how much coins are required.
	public Text lblMessageBuyOne;

	//Current item selected
	int currentId = 0;

	//Selected Customitem
	public CustomItem currentItem;

	//Last item button clicked
	public Button lastItemClicked;
	bool flag;
    public Button btnfinish;
	//The actual game objects (with image script on them) on the scene - the images will be set on these objects
	public Image objShirts;
	public Image objPants;
	public Image objRShoes;
	public Image objLShoes;
	public Image objCurtain;
	public Image objMasks;
	public Image objLeftHands;
	public Image objRightHands;

	//List of type CustomItem to store the items of different types
	List<CustomItem> Shirts = new List<CustomItem>(); 
	List<CustomItem> Pants = new List<CustomItem>();
	List<CustomItem> RightShoes = new List<CustomItem>();
	List<CustomItem> LeftShoes = new List<CustomItem>();
	List<CustomItem> Curtains = new List<CustomItem>();
	List<CustomItem> Masks = new List<CustomItem>();
	List<CustomItem> LeftHands = new List<CustomItem>();
	List<CustomItem> RightHands = new List<CustomItem>();

	//An array of menu buttons that are used - inorder to make selected button active and others inactive
	public Button[] buttons;

	//Different Panels in the scene
	public GameObject mainPanel;
	public GameObject gamePanel;


	//Buy All function is called from the BuyAll Panel on the click of $3.99 button
	public void buyAll()
	{
		//Your inapp handling code here

		//After the purchase is completed, to enable all items:
		//PlayerPrefs.SetInt("allunlocked",1);
	}

	//Cross button is called from the Buy All/ Buy One Panels to close them
	public void crossButtonPressed()
	{

		if(buyScreenOne.activeSelf || buyScreenAll.activeSelf)
		{
			
			currentScreen = "choose";

			//Checks if any of the buy screen is active
			if(lastItemClicked != null)
			{
				itemClicked(lastItemClicked);
			}
			else
			{
				itemClicked(GameObject.Find("0").GetComponent<Button>());
			}

			//Reverts the item to the last item selected and closes the buy panel
			buyScreenOne.SetActive(false);
		}

		buyScreenAll.SetActive(false);

	}



	//Main function called when the user presses the Play button from the MainScreen panel
	public void startGame()
	{
		//CurrentScreen string is important to know which panel is on top - inorder to disable clicks
		currentScreen = "choose";
		mainPanel.SetActive(false);
		gamePanel.SetActive(true);

		//The user is given 1000 coins when he first opens the app
		if(!PlayerPrefs.HasKey("firstTime"))
		{
			PlayerPrefs.SetInt("firstTime",1);
			PlayerPrefs.SetInt("coins",0);
			//Debug.Log(	PlayerPrefs.SetInt("coins",0));
		}

		//First menu button click is called at the start
		menuButtonClicked(buttons[0]);

	}
	public void FixedUpdate(){
		if (flag == false) {

			//CurrentScreen string is important to know which panel is on top - inorder to disable clicks
			currentScreen = "choose";
			mainPanel.SetActive (false);
			gamePanel.SetActive (true);
		
			//The user is given 1000 coins when he first opens the app
			if (!PlayerPrefs.HasKey ("firstTime")) {
				PlayerPrefs.SetInt ("firstTime", 1);
				PlayerPrefs.SetInt ("coins", 0);
				//Debug.Log(	PlayerPrefs.SetInt("coins",0));
			}
		
			//First menu button click is called at the start
			menuButtonClicked (buttons [0]);
		//	Debug.Log("shirtdiplay");
			flag=true;
		}
		//Debug.Log ("total="+(shirtscore+pentscore));


	}
	

	//Start function is called at the start of the game when the user is on MainScreen
	void Start()
	{

	


		//PlayerPrefs.GetInt ("coins", 0);
		//Current item is set to 0
		currentId = 0;

		//Loaddata function is very important as it loads the sprites from the Resources folders 
		//Every list (shirts, curtains etc) is populated with customitem objects here
		loadData("Shirts",Shirts,true); 
		loadData("Curtains",Curtains,true); 
		loadData("Pants",Pants,true); 
		loadData("LeftShoes",LeftShoes,true); 
		loadData("RightShoes",RightShoes,true); 
		loadData("Masks",Masks,true);
		loadData("LeftHands",LeftHands,false);
		loadData("RightHands",RightHands,false);

		//Current screen is main
		currentScreen = "main";

		//Coins text of the user is updated to reflect his current coin score
		foreach(Text l in lblCoins)
		{

//Debug.Log(l.text);
		
			l.text = PlayerPrefs.GetInt("coins").ToString();
		//	Debug.Log(l.text);


		//	Debug.Log(l.text);
		}


	}


	//Load data function is very important as it creates the different customitem objects and adds them to the lists
	void loadData(string type, List<CustomItem> lst, bool withNull)
	{
		lst.Clear();
		currentId = 0;

		//All sprites in the folder "type" are loaded 
		foreach(Sprite spr in Resources.LoadAll<Sprite>(type))
		{
			//A new custom item object is created and sprites are added to these
			lst.Add(new CustomItem(currentId,type,spr,500,true));
			currentId ++;
		}
	}


	//This function is called whenever the menu button is clicked (from the GameScreen)
	public void menuButtonClicked(Button thisButton)
	{
		//If the current screen is buy/buy all, then don't apply the clicks.
		if (currentScreen != "choose")
			return;

		//A menu button was clicked, change the sprites of all buttons to inactive
		switchoffButtons ();

		//All generated items earlier are destroyed (found using their tags)
		destroyItems ();


		//These conditions check which button is clicked (from the name of the button)
		if (thisButton.name.Contains ("Shirts"))
			generateItems (Shirts);
	
		else if(thisButton.name.Contains("Pant"))
			generateItems(Pants);
		else if(thisButton.name.Contains("RightHand"))
			generateItems(RightHands);
		else if(thisButton.name.Contains("LeftHand"))
			generateItems(LeftHands);
		else if(thisButton.name.Contains("LeftShoe"))
			generateItems(LeftShoes);
		else if(thisButton.name.Contains("RightShoe"))
			generateItems(RightShoes);
		else if(thisButton.name.Contains("Masks"))
			generateItems(Masks);
		else if(thisButton.name.Contains("Shirt"))
			generateItems(Shirts);
		else if(thisButton.name.Contains("Pant"))
			generateItems(Pants);
		else if(thisButton.name.Contains("Curtain"))
			generateItems(Curtains);


		//These couple of lines give a couple of coins to the user for every click in the game
		if (selectedMenu != thisButton.name) {
		
			addCoins(Random.Range(1,3));

		//	Debug.Log("click");
		}
		//The menu that is active is now assigned
		selectedMenu = thisButton.name;

		//Now the selected button sprite should be changed to active
		thisButton.GetComponent<Image>().sprite = imgOnButton;

		lastItemClicked = null;

	}

	//The regenerate button is used as a replica of the above function
	public void regenerate(string btn)
	{
		
		if(currentScreen != "choose")
			return;
		
		
		switchoffButtons();
		destroyItems();
		
		if(btn.Contains("Masks"))
			generateItems(Masks);
		else if(btn.Contains("LeftHand"))
			generateItems(LeftHands);
		else if(btn.Contains("RightHand"))
			generateItems(RightHands);
		else if(btn.Contains("LeftShoe"))
			generateItems(LeftShoes);
		else if(btn.Contains("RightShoe"))
			generateItems(RightShoes);
		else if(btn.Contains("Shirt"))
			generateItems(Shirts);
		else if(btn.Contains("Curtain"))
			generateItems(Curtains);
		else if(btn.Contains("Pant"))
			generateItems(Pants);
	
	}


	//This function is called from the Buy One screen when the user has enough coins to purchase the item
	public void unlockThisItem()
	{
		//Buy one screen is closed
		buyScreenOne.SetActive(false);

		//Coins are subtracted from user's total coins
		PlayerPrefs.SetInt("coins",PlayerPrefs.GetInt("coins")-currentItem.price);

		//The item is set to unlocked
		PlayerPrefs.SetInt(currentItem.categoryId + "" + currentItem.id,1);

		//The screen is now back to choose
		currentScreen = "choose";

		//The items are refreshed to reflect the new unlocked item
		regenerate(selectedMenu);

		//Coins text is updated
		updateCoins();


	}




	public void itemClicked(Button thisButton)
	{

		//If buy screen is active, don't entertain clicks on background
		if(currentScreen != "choose")
			return;

		//By default the item is unlcocked
		bool isThisItemLocked = false;

		//Checks if a lock exists - that means the item is locked
		if(thisButton.gameObject.transform.FindChild("lockicon").GetComponent<Image>().enabled == true)
		{
			isThisItemLocked = true;
		}

		Image sprrenderer = null;
		List<CustomItem> lst = null;

		//Checks which menu is selected
		//The appropriate scene object is selected in sprrenderer image variable
		if(selectedMenu.Contains("Masks"))
		{
			lst = Masks;
			sprrenderer = objMasks; 
		}
		else if(selectedMenu.Contains("LeftHand"))
		{
			lst = LeftHands;
			sprrenderer = objLeftHands;
		}
		else if(selectedMenu.Contains("RightHand"))
		{
			lst= RightHands;
			sprrenderer = objRightHands;

		}
		else if(selectedMenu.Contains("RightShoe"))
		{
			lst= RightShoes;
			sprrenderer = objRShoes;
			
		}
		else if(selectedMenu.Contains("LeftShoe"))
		{
			lst= LeftShoes;
			sprrenderer = objLShoes;
			
		}

		else if(selectedMenu.Contains("Shirt"))
		{
			lst= Shirts;
			sprrenderer = objShirts;

		}

		else if(selectedMenu.Contains("Curtain"))
		{
			lst= Curtains;
			sprrenderer = objCurtain;
			
		}

		else if(selectedMenu.Contains("Pant"))
		{
			lst= Pants;
			sprrenderer = objPants;
			
		}

		//Assigns the select sprite to the scene object
		int id = int.Parse(thisButton.name);
		sprrenderer.sprite = lst[id].spr;

		////Gives coins to the user for clicking
		//Debug.Log (lst[id].spr);

        if (currentItem != lst[id])
        {
            //			if(lst[id].spr==GameObject.FindWithTag()){
            //
            //
            //			}if(lst[id].spr==GameObject.FindWithTag()){
            //				
            //				
            //			}


            //Debug.Log(shirtslist[0]);
            if (lst[id].spr == shirtslist[0])
            {
                addCoins(3);
                shirtscore = 3;
                Debug.Log("whiteshirt");
            }
            if (lst[id].spr == shirtslist[1])
            {
                addCoins(4);
                shirtscore = 4;
                Debug.Log("redshirt");

            } if (lst[id].spr == shirtslist[2])
            {
                addCoins(5);
                shirtscore = 5;
                Debug.Log("blueshirt");
            }
            if (lst[id].spr == pentslist[0])
            {
                addCoins(3);
                pentscore = 3;
                Debug.Log("whitepent");
            } if (lst[id].spr == pentslist[1])
            {
                addCoins(4);
                pentscore = 4;
                Debug.Log("orangepent");
            } if (lst[id].spr == pentslist[2])
            {
                addCoins(5);
                pentscore = 5;
                Debug.Log("redpent");
            }


          
                Debug.Log("enter into");

                btnfinish.gameObject.SetActive(true);



            //addCoins (Random.Range (1, 3));
        }
       

  //      Debug.Log(lst[id]);
      //  DontDestroyOnLoad()
        total = shirtscore + pentscore;
       // Debug.Log(total);
	//	Debug.Log (shirtscore+pentscore);
        PlayerPrefs.SetInt("score", total);

		
		//Current item is updated
		currentItem = lst[id];
		//Debug.Log (currentItem.price);

		//Is the item locked? then show the buy screen
		if(isThisItemLocked)
		{
			//If the user does not have adequate coins, ask him to buy using inapp in buy all screen
			if(PlayerPrefs.GetInt("coins") < currentItem.price )
			{
				buyScreenAll.SetActive(true);
				return;
			}

			//The user has enough coins, show him the buy one screen and update the label
			buyScreenOne.SetActive(true);
			lblMessageBuyOne.text = "YOU NEED " + currentItem.price + " COINS TO BUY THIS ITEM";

			//Now the current screen is buy - not choose. So items/menus wont work
			currentScreen = "buy";
		}
		else
		{
			buyScreenOne.SetActive(false);
			currentScreen = "choose";
			lastItemClicked = thisButton;
		}


	}

	//function to add coins to the users prefs
	public void addCoins(int c)
	{
		PlayerPrefs.SetInt("coins",PlayerPrefs.GetInt("coins")+c);
		updateCoins();
	}

	//function to update coins label everywhere
	public void updateCoins()
	{
		foreach(Text l in lblCoins)
		{
			l.text = PlayerPrefs.GetInt("coins").ToString();
		}
	}

	//changes sprites of all buttons to inactive sprite imgOffButton
	void switchoffButtons()
	{
		foreach(Button b in buttons)
		{
			b.GetComponent<Image>().sprite =  imgOffButton;
		}
	}


	//A crucial function to instantiate items buttons when a menu is selected.
	//Buttons are generated according to the list of custom items passed
	void generateItems(List<CustomItem> lst)
	{
		int i = 0;
		Vector3 currentPosition = defaultItem.transform.localPosition;

		//Hardcoded starting y position of default item
		currentPosition.y = 229.504f;

		//Traverses through the list of customitems passed
		foreach (CustomItem c in lst)
		{

			//A new instance of default item is generated
			GameObject g = GameObject.Instantiate(defaultItem) as GameObject;
			g.transform.FindChild("sprite").GetComponent<Image>().sprite = c.spr;
			g.transform.parent = defaultItem.transform.parent;
			g.transform.localPosition = currentPosition;
			g.transform.localScale = defaultItem.transform.localScale;

			//the positions and scale of the new object is adjusted

			//is the item locked? then enable the lock icon image - otherwise disable it.
			if(c.isLocked())
			{
				g.transform.FindChild("lockicon").GetComponent<Image>().enabled = true;
			}
			else
			{
				g.transform.FindChild("lockicon").GetComponent<Image>().enabled = false;
			}

			//The object is made visible
			g.SetActive(true);

			//The tag of the object is changed to item so that it can be destroyed via GameObject.FindWithTag
			g.tag = "item";
			g.name = c.id.ToString();

			//Current position is subtracted by 250 to add an object - not necessary with vertical group layout
			currentPosition.y-=250;
			i++;


		}
	}


	//All generated objects are destroyed before new are created
	void destroyItems()
	{
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("item"))
		{
			Destroy(g);
		}
	}








}
