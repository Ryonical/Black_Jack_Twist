using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Deck : MonoBehaviour {

    public List<GameObject> toDeal;
    public List<GameObject> discard;
    private System.Random rnd = new System.Random();
    //deals one card
    public GameObject Deal1()
    {
        //created so can both return and remove 
        GameObject temp;
        //picks a random card
        int num = rnd.Next(toDeal.Count);
        temp = toDeal[num];
        //removes chosen card from the list
        toDeal.RemoveAt(num);
        //returns chosen card
        return temp;
    }

    //adds a card to the discard list
    public void AddDiscard(GameObject oldCard)
    {
        discard.Add(oldCard);
    }

    
} 
