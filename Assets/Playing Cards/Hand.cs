using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour {

    public Deck deck; //the deck you draw from
    public List<GameObject> myHand; //your current hand
    public List<GameObject> dealerHand; //the dealers current hand
    private Vector3 myPos;//the position for the players cards
    private Vector3 dealerPos;//the position for the dealers cards
    public Text text;//this is your total output
    public Text dealerText;//this is the dealer total output
    public Text resultText;//this is the win lose box output
    private int gameMode = 0; //this  indicates the current gamemode
    public GameObject newHand;//the new hand button
    public GameObject hit;//the hit button
    private System.Random rnd = new System.Random();//for picking the effect of gamemode
    private string bonusText = "";//the text added by the mode
    //these are the texts for the gamemodes
    private string[] angryModeText = {
    "Boy I hope you mom doesn’t find out how you played that hand",
    "My younger brother a calculator, could have done better than you",
    "Your mom sure dressed you funny today",
    "Could you play another game",
    "If you win this hand you can get a juice box",
    "Go ahead and make my day"};
    private string[] happyModeText = {
        "My, you are impressive!",
        "Your mom would be proud of you!",
        "You Rock!",
        "You can take the risk, you are lucky!",
        "I am so glad you are playing my game today!",
        "If you win big I won’t tell the IRS!"};
    private string[] workOutModeText = {
        "Do 5 Burpees",
        "Do 3 Pushups",
        "Do 7 Jumping Jacks",
        "Do 2 Squats",
        "Do 5 Situps",
        "Do 1 Pullup"};
        //this is 2d to have the answers too
    private string[,] questionModeText = new string[6, 2]
    {{"83*47=?", "3901"},
    {"Who wrote the Odyssey?", "homer"},
    {"What city used to be called Constantanopal?", "istanbul"},
    {"Lima is the capital of what country?", "peru"},
    {"What is the main character in Harry Potter’s first name?", "harry"},
    {"Who is Mario’s brother in Nintendo's video games", "luigi"},
    };
    private int questionAnswer = -1;

    //too see if you need to draw or hit
    public void CheckHit(bool isNew)
    {
        if(isNew)
        {
            Draw();
        }
        else
        {
            Hit();
        }
            
    }

    //draws a card
    public void Hit()
    {
        myHand.Add(deck.Deal1());
        //moves the card into position
        myPos.x += 5;
        myHand[myHand.Count-1].transform.position = myPos;
        //gamemodes
        GameMode();
        //tells you the total
        text.text = "Your Total: " + CheckCards(myHand).ToString();
        //if you win or lose
        if(CheckCards(myHand) >= 21)
        {
            Stand();
        }
        //tells you the dealers total needs to be here too for the gamemode
        dealerText.text = "Dealer Total: " + CheckCards(dealerHand).ToString() + " " + bonusText;
        //resets bonus text for alien mode
        bonusText = "";

    }

    //draws for dealer
    public void DealerHit()
    {
        dealerHand.Add(deck.Deal1());
        //moves the card into position
        dealerPos.x += 5;
        dealerHand[dealerHand.Count-1].transform.position = dealerPos;
        //tells you the dealers total
        dealerText.text = "Dealer Total: " + CheckCards(dealerHand).ToString() + " " + bonusText;
            
    }

    //draws two cards (called when new hand)
    public void Draw()
    {
        //gamemodes
        GameMode();
        //resets the hand doesn't matter that it is called on first run
        Reset();
        //makes it so you hit next needs to be before hit for the rare A+10
        newHand.SetActive(false);
        hit.SetActive(true);
        //hits twice for dealer needs to come before hit since it would call again after stand otherwise
        DealerHit();
        DealerHit();
        //hits twice for player allowing them to start  
        Hit();
        Hit();
        
        
    }

    //checks the total of the cards
    private int CheckCards(List<GameObject> hand)
    {
        int cardTotal = 0;//the total count
        int numAce = 0;//counts the aces
        //adds the total up
        for(int i = 0; i < hand.Count; i++)
        {
            cardTotal += hand[i].GetComponent<Cards>().cardNum;
            //adds num aces
            if(hand[i].GetComponent<Cards>().cardNum == 11)
            {
                numAce++;
            }
        }
        //to convert the ace's from 11 to 1 if needed
        for(int i = 0; i < numAce && cardTotal > 21; i++)
        {
            cardTotal -= 10;
        }
        return cardTotal;
    }


    //has a 10% chance to discard a card in your hand
    private void AlienMode(List<GameObject> hand)
    {
        int num = rnd.Next(4); //25% chance
        if(num == 1 && hand.Count > 0)
        {
            //picks card to be abducted
            num = rnd.Next(hand.Count);
            //tells what card abducted
            bonusText += " the aliens abducted " + hand[num].GetComponent<Cards>().cardNum.ToString();
            //removes card
            hand[num].transform.position = new Vector3(-1000, -1000, -1000);
            deck.AddDiscard(hand[num]);
            hand.RemoveAt(num);
        }
    }

    //resets the board
    public void Reset()
    {
        //resets the win text
        resultText.text = "";
        //sets buttons to start new hand
        newHand.SetActive(true);
        hit.SetActive(false);
        Vector3 pos = new Vector3(-1000, -1000, -1000);//off in the void
        //moves all the hand cards to discrd and moves them away
        while(myHand.Count > 0)
        {
            myHand[0].transform.position = pos;
            deck.AddDiscard(myHand[0]);
            myHand.RemoveAt(0);
        }
        while(dealerHand.Count > 0)
        {
            dealerHand[0].transform.position = pos;
            deck.AddDiscard(dealerHand[0]);
            dealerHand.RemoveAt(0);
        }
        myPos = new Vector3(-25, -5, 0);//resets pos to original position
        dealerPos = new Vector3(-25, 5, 0);//same
    }

    //for when you stand
    public void Stand()
    {
        //keeps the dealer drawing if he is less than 17 total per the rules
        if(CheckCards(myHand) <= 21)
        {
            while(CheckCards(dealerHand) < 17)
            {
                DealerHit();
            }
        }
        //sees who wins
        Result();
        //makes it so you draw a new hand next
        newHand.SetActive(true);
        hit.SetActive(false);
    }


    //outputs the result
    private void Result()
    {
        //these are to make the ifs more efficient than just calling CheckCards over and over
        int myHandVal = CheckCards(myHand);
        int dealerHandVal = CheckCards(dealerHand);

        
        if(myHandVal > 21 && dealerHandVal > 21)//both lose
        {
            resultText.text = "You Both Lose!";
        }
        else if(myHandVal > dealerHandVal && myHandVal <= 21)//player wins
        {
            resultText.text = "You Win!";
        }
        else if(dealerHandVal > myHandVal && dealerHandVal <= 21)//dealer wins
        {
            resultText.text = "Dealer Wins!";
        }
        else if(myHandVal > dealerHandVal && dealerHandVal <= 21)//you go over
        {
            resultText.text = "Dealer Wins!";
        }
        else if(dealerHandVal > myHandVal && myHandVal <= 21)//dealer goes over
        {
            resultText.text = "You Win!";
        }
        else if(myHandVal == dealerHandVal)//dealer wins if same number not 21
        {
            resultText.text = "Push!";
        }
    }

    //it would probably be better too just input a number from the button
    //sets game mode to No Mode
    public void SetMode0()
    {
        gameMode = 0;
    }

    //sets game mode to Angry Dealer Mode
    public void SetMode1()
    {
        gameMode = 1;
    }


    //sets game mode to Happy Dealer Mode
    public void SetMode2()
    {
        gameMode = 2;
    }

    //sets game mode to Weight Loss Dealer Mode
    public void SetMode3()
    {
        gameMode = 3;
    }

    //sets game mode to Alien Abduction Mode
    public void SetMode4()
    {
        gameMode = 4;
    }

    //sets game mode to Answer a Question Mode
    public void SetMode5()
    {
        gameMode = 5;
    }

    //activates gamemodes
    private void GameMode()
    {
        if(gameMode == 0)//no gamemode
        {
            bonusText = "";
        }
        else if(gameMode == 1)//angry gamemode
        {
            bonusText = angryModeText[rnd.Next(angryModeText.Length)];
        }
        else if(gameMode == 2)//happy gamemode
        {
            bonusText = happyModeText[rnd.Next(angryModeText.Length)];
        }
        else if(gameMode == 3)//weight loss gamemode
        {
            bonusText = workOutModeText[rnd.Next(angryModeText.Length)];
        }
        else if(gameMode == 4)//alien abduction gamemode
        {
            AlienMode(myHand);
            AlienMode(dealerHand);
        }
        else if(gameMode == 5)//question
        {
            questionAnswer =  rnd.Next(angryModeText.Length);
            bonusText = questionModeText[questionAnswer, 0];
            hit.SetActive(false);
        }
    }
    //reads the input from question mode
    public void ReadStringInput(string input)
    {
        //if answer wrong
        if(input.ToLower() != questionModeText[questionAnswer, 1] && CheckCards(myHand) < 21)
        {
            Hit();
        }
        if(CheckCards(myHand) < 21)//only do if still playable
        {
            hit.SetActive(true);
            newHand.SetActive(false);
        }
        if(CheckCards(myHand) >= 21)//to make sure that the right button is showing
        {
            Stand();
        }
    }
    
    //to make sure that the dealers text is up to date
    public void UpdateDealerText()
    {
        GameMode();
        dealerText.text = "Dealer Total: " + CheckCards(dealerHand).ToString() + " " + bonusText;
    }
}
