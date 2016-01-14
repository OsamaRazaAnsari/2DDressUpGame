using UnityEngine;
using System.Collections;

public class CustomItem  {


	//Private data members of a CustomItem 
	public int id;
	public string categoryId;
	public Sprite spr;
	public int price;


	//Prices of every type of object are set here.
	int getCategoryPrice(string categoryName)
	{
		
		int price = 0;
		
		switch (categoryName.ToLower())
		{
		case "shirts":
			price = 100;
			break;
		case "pants":
			price = 200;
			break;
		case "leftshoes":
			price = 100;
			break;
		case "rightshoes":
			price = 100;
			break;
		case "curtains":
			price = 500;
			break;
		case "masks":
			price = 250;
			break;
		case "righthands":
			price = 500;
			break;
		case "lefthands":
			price = 500;
			break;
			
		}
		
		return price;
		
	}

	
	//This function returns how many objects from the current category are unlocked
	int categoryLocked(string categoryName)
	{

		int totalUnlocked = 0;

		switch (categoryName.ToLower())
		{
			case "shirts":
			//Debug.Log(true);
			totalUnlocked = 15;
			break;
		case "pants":
			//Debug.Log(true);
			totalUnlocked = 4;
			break;
		case "rightshoes":
			totalUnlocked = 5;
			break;
		case "leftshoes":
			totalUnlocked = 5;
			break;
		case "masks":
			totalUnlocked = 3;
			break;
		case "lefthands":
			totalUnlocked = 6;
			break;
		case "righthands":
			totalUnlocked = 6;
			break;
		case "curtains":
			totalUnlocked = 3;
			break;
			
		}

		return totalUnlocked;

	}

	//The overloaded constructor
	public CustomItem(int id, string category, Sprite spr, int price, bool locked)
	{
		this.id = id;
		this.categoryId = category;
		this.spr = spr;
		this.price = getCategoryPrice(category);

	
		//This checks if the buy all option has been purchased? if it is, the locking mechanism is skipped.	 
		if(PlayerPrefs.HasKey("allunlocked"))
		{
			PlayerPrefs.SetInt(category+id,1);
		}
		else if(id<categoryLocked(category))
		{
			//Unlocks the number of items allowed in this category
			PlayerPrefs.SetInt(category+id,1);
		}

	}

	//Returns the status of the item
	public bool isLocked()
	{
		return (!PlayerPrefs.HasKey(this.categoryId+this.id));
	}
}
