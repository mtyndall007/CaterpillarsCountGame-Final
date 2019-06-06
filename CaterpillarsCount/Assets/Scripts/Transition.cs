using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Transition : MonoBehaviour
{
    Text playerScoreText;
    Text maxScoreText;
    Text bugsClickedText;
    Text bugsIdentifiedText;
    Text measurementAccuracyText;

    Button continueButton;
    UnityAction continueAction;

    int playerScore;
    int maxScore;

    public static Sprite background;
    // Start is called before the first frame update
    void Start()
    {

        if(background != null){
           Image bground = GameObject.Find("Background").GetComponent<Image>();
           bground.sprite = background;
        }

        MagnifyGlass.DisableZoom();

        continueButton = GameObject.Find("Continue").GetComponent<Button>();
        continueAction += continueFunction;
        continueButton.onClick.AddListener(continueAction);

        bugsClickedText = GameObject.Find("BugsClicked").GetComponent<Text>();

        bugsIdentifiedText = GameObject.Find("BugsIdentified").GetComponent<Text>();

        measurementAccuracyText = GameObject.Find("MeasurementAccuracy").GetComponent<Text>();
        if(GameManager.instance.bugsClicked > 0){
          bugsClickedText.text = "Arthropods found: " + Mathf.Round(100 * (float)GameManager.instance.bugsClicked/GameManager.instance.totalBugs) + "%";
          bugsIdentifiedText.text = "Correctly identified: " + Mathf.Round(100 * (float)GameManager.instance.bugsCorrectlyIdentified/GameManager.instance.totalBugs) + "%";
          measurementAccuracyText.text = "Measurement error: " + Mathf.Round((float)(GameManager.instance.measurementDistance / GameManager.instance.bugsClicked)) + "mm";

        } else {
          bugsClickedText.text = "";
          bugsIdentifiedText.text = "";
          measurementAccuracyText.text = "No bugs were clicked on!";
        }

        playerScoreText = GameObject.Find("UserScore").GetComponent<Text>();
        playerScoreText.text = "Your Score: " + ScoreScript.levelScore + "/" + GameManager.instance.levelScore;

    }

    public static void setBackground(Sprite bground){
        background = bground;
    }

    private void continueFunction(){
        if(GameManager.instance.sceneIterator == 5){
          GameManager.instance.gameOverSubmission();
          SceneManager.LoadScene(GameManager.instance.currentScene);
        } else {


        GameManager.instance.ResetBugCounts();
        ScoreScript.ResetScore();
        //SceneManager.LoadScene(GameManager.instance.sceneIterator);
        SceneManager.LoadScene(GameManager.instance.levelSelector(GameManager.instance.sceneIterator));
      }
    }


}
