using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	public GameObject pausePanel;
	public GameObject resultPanel;
	public Button pauseButton;

	// This function loads the main screen
	public void LoadMainScreen()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene ("__MainScreen");
	}

	// This function loads the game screen
	public void LoadGameScreen()
	{
		SceneManager.LoadScene ("_GameScreen");
	}

	// This function pauses the game
	public void PauseGame()
	{
		Time.timeScale = 0;
		pauseButton.gameObject.SetActive (false);
		pausePanel.SetActive (true);
	}

	// This function resume the game
	public void ResumeGame()
	{
		pauseButton.gameObject.SetActive (true);
		pausePanel.SetActive (false);
		Time.timeScale = 1;
	}

	// This function restarts the game
	public void RestartGame()
	{
		resultPanel.SetActive (false);
		Time.timeScale = 1;
		SceneManager.LoadScene ("_GameScreen");
	}

	// This function quits the game
	public void ExitGame()
	{
		Application.Quit ();
	}
}
