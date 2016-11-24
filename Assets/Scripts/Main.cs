using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	public GameObject[] cardPrefabs;
	public GameObject wonSound;
	public GameObject lostSound;
	public GameObject matchSound;
	public GameObject notMatchSound;

	public GameObject resultPanel;
	public Button pauseButton;
	public Text finalScoreText;
	public Text cardsRevealedText;
	public Text elapsedTimeText;
	public Text winLostText;
	public Text scoreText;
	public Text dataText;

	private GameObject[] cards;
	private GameObject firstClicked;
	private GameObject secondClicked;
	private int clicks = 0;
	private bool enableClick = true;
	private float startTime;
	private float elapsedTime;
	private int cardsRevealed = 0;
	private int score = 1000;

	// Use this for initialization
	void Start ()
	{
		Time.timeScale = 1; // Make sure the game is unpaused
		startTime = Time.time; // Set start time
		cards = new GameObject[18]; // Create an array of gameobjects to hold cards
		// This for loop instantiates the card prefabs
		for (int i = 0; i < 18; i++)
		{
			cards[i] = Instantiate (cardPrefabs [i]) as GameObject;
		}
		// This calls the ShuffleCards function to shuffle the cards
		ShuffleCards ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		elapsedTime = Time.time - startTime; // This calculates the time elapsed since the beginning of the game
		scoreText.text = "Score: " + score.ToString(); // Converts score into a string to display on screen
		dataText.text = cardsRevealed.ToString() + " card(s) revealed in " + elapsedTime.ToString("##.00") + " seconds"; // Converts the number of cards revealed and the elapsed time to string to display on the screen
		if (Input.GetMouseButtonDown (0) && clicks < 2 && enableClick) // If left mouse button is clicked and the number of clicks is less than 2 and clicks are allowed then...
		{
			if (clicks <= 1 ) // If clicks is less than 1 then...
			{
				CheckCards (); // Check if the user clicked a card
			}
		}
		else if (clicks == 2 && !enableClick) // If clicks is equal to 2 and the user is not allowed to click then...
		{
			FlipUp (firstClicked); // Flip up the first clicked card
			FlipUp (secondClicked); // Flip up the second clicked card
			if (CompareCards ()) // Compare the cards and if they are the same then...
			{
				cardsRevealed += 2; // Increment cards revealed by 2
				PlayMatchSound(); // Play winning sound for finding a matching pair
				StartCoroutine (DisableAfterAnimation ()); // Disable the cards after animation is done
			}
			else // If the cards are not equal then...
			{
				score -= 40; // Decrement the score by 40
				PlayNotMatchSound(); // Play losing sound for not finding a matching pair
				StartCoroutine (DelayFlipBack ()); // Give some time to the user to memorize the cards before flipping back
			}
			enableClick = true; // Allow the user to click
		}
		CheckGameProgress (); // Check the game progress (checks if the game is done or not)
	}

	// This function compares two cards by their names
	bool CompareCards()
	{
		firstClicked.name = firstClicked.name.Replace ("(Clone)", "");
		secondClicked.name = secondClicked.name.Replace ("(Clone)", "");

		if (firstClicked.name.Contains (" 1"))
			firstClicked.name = firstClicked.name.Replace (" 1", "");
		if (secondClicked.name.Contains (" 1"))
			secondClicked.name = secondClicked.name.Replace (" 1", "");

		if (firstClicked.name == secondClicked.name) // If the cards have the same name then return true
			return true;
		else // Otherwise, return false
			return false;
	}

	// This function checks if the user clicked a card or not
	void CheckCards()
	{
		GameObject clickedCard = GetClickedGameObject (); // Gets the clicked game object
		if (clickedCard != null && clickedCard.tag == "Clickable") // If the clicked game object is not null and has a tag of "Clickable" then...
		{
			if (clicks == 0) // If clicks are equal to 0 then...
			{
				clicks++; // Increment the number of clicks
				firstClicked = clickedCard; // Set the first clicked card
				enableClick = true; // Allow the user to click another card
			}
			else if (clicks == 1 && clickedCard != firstClicked) // If clicks are equal to 1 and the clicked card does not equal the first clicked card then...
			{
				clicks++; // Increment the number of clicks
				secondClicked = clickedCard; // Set the second clicked card
				enableClick = false; // Do not allow the user to click anymore
			}
		}
	}

	// This function shuffles the cards by randomly swapping the positions of two cards in the array
	void ShuffleCards()
	{
		int randIndex; // Holds the random index
		for (int i = 0; i < 18; i++)
		{
			randIndex = Random.Range (0, 18); // Gets a random index between 0 to 17 all inclusive
			// Swap the ith element's position with the element's position in the random index
			Vector3 temp = cards [i].transform.position;
			cards [i].transform.position = cards [randIndex].transform.position;
			cards [randIndex].transform.position = temp;
		}
	}

	// This function checks if the game is done or not
	void CheckGameProgress()
	{
		if (score == 0) // If the score is equal to 0 then...
		{
			// Set the following texts to the current score, cards revealed and time elapsed
			finalScoreText.text = scoreText.text;
			cardsRevealedText.text = "Cards Revealed: " + cardsRevealed.ToString ();
			elapsedTimeText.text = "Time Elapsed: " + elapsedTime.ToString ("##.00") + " seconds";
			// Deactivate the score text, data text and the pause button
			scoreText.gameObject.SetActive (false);
			dataText.gameObject.SetActive (false);
			pauseButton.gameObject.SetActive (false);
			// Display that the user lost
			winLostText.text = "You Lost...";
			// Delay the results panel so there is time for the final animation to complete
			StartCoroutine (DelayResults (false));
		}
		else if (cardsRevealed == 18)
		{
			// Set the following texts to the current score, cards revealed and time elapsed
			finalScoreText.text = scoreText.text;
			cardsRevealedText.text = "Cards Revealed: " + cardsRevealed.ToString ();
			elapsedTimeText.text = "Time Elapsed: " + elapsedTime.ToString ("##.00") + " seconds";
			// Deactivate the score text, data text and the pause button
			scoreText.gameObject.SetActive (false);
			dataText.gameObject.SetActive (false);
			pauseButton.gameObject.SetActive (false);
			// Display that the user won
			winLostText.text = "You Won!";
			// Delay the results panel so there is time for the final animation to complete
			StartCoroutine (DelayResults (true));
		}
	}

	// This function flips up the card
	void FlipUp(GameObject card)
	{
		card.GetComponent<Animator> ().enabled = true;
		string animName = "Flip" + card.name.Replace("(Clone)","");
		if (animName.Substring (animName.Length - 2) == " 1")
			animName = animName.Replace (" 1", "");
		card.GetComponent<Animator> ().CrossFade (animName, 0f);
	}

	// This function flips down the card
	void FlipDown(GameObject card)
	{
		string animName = "FlipBack" + card.name.Replace("(Clone)","");
		if (animName.Substring (animName.Length - 2) == " 1")
			animName = animName.Replace (" 1", "");
		card.GetComponent<Animator> ().CrossFade (animName, 0f);
	}

	// This function plays the final win sound
	void PlayWonSound()
	{
		AudioSource player = wonSound.GetComponent<AudioSource> ();
		player.PlayDelayed(0.6f);
	}

	// This function plays the final lost sound
	void PlayLostSound()
	{
		AudioSource player = lostSound.GetComponent<AudioSource> ();
		player.PlayDelayed(0.6f);
	}

	// This function plays the win sound when found a matching pair
	void PlayMatchSound()
	{
		AudioSource player = matchSound.GetComponent<AudioSource> ();
		player.PlayDelayed(0.6f);
	}

	// This function plays the lost sound when found a non-matching pair
	void PlayNotMatchSound()
	{
		AudioSource player = notMatchSound.GetComponent<AudioSource> ();
		player.PlayDelayed(0.5f);
	}

	// This functions gets the clicked game object
	GameObject GetClickedGameObject ()
	{
		// Building a ray
		RaycastHit hit;
		Ray point = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Casts the ray and gets the first GameObject that is hit
		if (Physics.Raycast (point, out hit, 10f))
			return hit.transform.gameObject;
		else
			return null;
	}

	// This function delays disabling the cards
	IEnumerator DisableAfterAnimation()
	{
		yield return new WaitForSeconds(2f); // This will wait 1 second
		firstClicked.SetActive (false);
		secondClicked.SetActive (false);
		clicks = 0;
	}

	// This function delays the card flipping back
	IEnumerator DelayFlipBack()
	{
		yield return new WaitForSeconds(2f); // This will wait 1 second
		FlipDown (firstClicked);
		FlipDown (secondClicked);
		clicks = 0;
	}

	// This function delays the results panel and plays the appropriate sound depending on the results
	IEnumerator DelayResults(bool win)
	{
		yield return new WaitForSeconds (2f);
		Time.timeScale = 0;
		resultPanel.SetActive (true);
		if (win)
			PlayWonSound ();
		else
			PlayLostSound ();
	}
}
