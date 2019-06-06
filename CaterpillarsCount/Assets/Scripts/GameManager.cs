using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Script for controlling levels, scores, etc.
//Has the UI as a child object

public class GameManager : MonoBehaviour
{

    //An instance of the game manager that can be invoked. Should only be one instance at a time
    #region Singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //Calls a utility method that selects the level for a given playthrough. Store these in an array of scenes.
            //spawnedScenes = LevelSpawner.SpawnScenes();
            sceneIterator = 2;
            TimerScript.SetCurrentTime(findTime(sceneIterator));
            SceneManager.LoadScene(levelSelector(sceneIterator));
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    //Used for tracking what scenes are loaded
    //spawnedScenes are the scene's pathnames
    private string[] spawnedScenes;
    public int sceneIterator;
    public int currentScene;

    //Action declarations for callbacks
    private UnityAction submitAction;
    private UnityAction playAgainAction;
    private UnityAction returnAction;
    public UnityAction<GameObject> bugClicked;
    public UnityAction returnZoom;
    public UnityAction<string> bugIdentified;

    public GameObject rulerPrefab;

    Button levelSubmitButton;
    Button playAgainButton;
    Button returnButton;
    //Button bugUISubmitButton;

    //Vars for keeping track of player stats per level
    private int playerScore;
    public int levelScore;
    private int totalScore;
    public int bugsCorrectlyIdentified;
    public int bugsClicked;
    public int totalBugs;
    public float measurementDistance;

    private string selectedBug;


    //Private vars for the zooming effect once a bug has been clicked
    private float defaultFOV;
    private Vector3 defaultCameraPosition;
    private float zoomedFOV;
    private bool zoomingIn;
    private bool zoomingOut;
    private float zoomInSpeed = 5f; //5f
    private float zoomOutSpeed = 6f;
    private bool bugHasBeenCategorized = false;
    private bool measurementGiven = false;

    GameObject gameOver;
    GameObject returnObject;
    GameObject bugSelectionUI;
    GameObject bugButtons;
    GameObject bugSelectionText;
    GameObject ruler;
    Bug currentBugScript;

    GameObject lengthUI;
    GameObject lengthElements;
    GameObject lengthSubmit;

    GameObject otherUI;

    InputField measurementInput;

    // Start is called before the first frame update
    void Start()
    {
        selectedBug = null;
        bugsClicked = 0;
        bugsCorrectlyIdentified = 0;
        measurementDistance = 0;

        //ruler = GameObject.Find("Ruler");
        //ruler.SetActive(true);

        returnObject = GameObject.Find("Return");
        returnObject.SetActive(false);

        defaultFOV = Camera.main.orthographicSize;
        defaultCameraPosition = Camera.main.transform.position;
        zoomedFOV = defaultFOV / 4f;

        //Finds the submit button from the scene and adds an event listener
        levelSubmitButton = GameObject.Find("LevelSubmit").GetComponent<Button>();
        submitAction += Submit;
        levelSubmitButton.onClick.AddListener(submitAction);

        //Callback function for when a bug has been clicked by the user
        bugClicked += BugClicked;

        //Find the gameover UI
        gameOver = GameObject.Find("GameOver");
        //Make the gameover screen invisible
        gameOver.SetActive(false);

        bugButtons = GameObject.Find("BugButtons");
        bugSelectionText = GameObject.Find("BugSelectionText");

        //Hide the bug selection UI at startup
        bugSelectionUI = GameObject.Find("BugSelectionUI");
        bugSelectionUI.SetActive(false);
        bugButtons.SetActive(false);
        bugSelectionText.SetActive(false);

        lengthUI = GameObject.Find("LengthUI");
        lengthElements = GameObject.Find("LengthElements");
        lengthSubmit = GameObject.Find("LengthSubmit");
        ruler = GameObject.Find("Ruler");

        lengthUI.SetActive(false);

        otherUI = GameObject.Find("OtherSelection");
        otherUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && measurementInput != null) {
           EvaluateMeasurement(measurementInput);
           measurementInput.DeactivateInputField();
        }

        if (Input.GetMouseButtonDown(0)) {
            MagnifyGlass.DisableZoom();
        }

        if (Input.GetMouseButtonDown(1) && !MagnifyGlass.IsZoomable()) {
            MagnifyGlass.ResetCounter();
            MagnifyGlass.EnableZoom();
        }

        if (TimerScript.GetCurrentTime() <= 0)
        {
            Submit();
        }

        //Might want to make these into coroutines to delay the zoom a bit
        //Linearly interpolates between the default camera view and the zoomed view. Updates each frame for a smoother zoom effect
        if (zoomingIn)
        {

            if (Camera.main.orthographicSize <= zoomedFOV)
            {
                zoomingIn = false;
                Camera.main.orthographicSize = zoomedFOV;
            } else {
                Camera.main.orthographicSize += Time.deltaTime * -zoomInSpeed;
            }

        }

        if (zoomingOut)
        {

          if (Camera.main.orthographicSize >= defaultFOV)
          {
              zoomingOut = false;
              Camera.main.orthographicSize = defaultFOV;
          } else {
              Camera.main.orthographicSize += Time.deltaTime * zoomOutSpeed;
          }

        }
    }

    //Public method called by timer once it hits 0
    public static void TimerSubmit() => GameManager.instance.Submit();

    //Public method for a bug to call once it has been clicked
    public void BugClicked(GameObject bug)
    {
        bugsClicked++;
        //Zooms camera in on bug
        Camera.main.orthographic = true;
        Camera.main.transform.position = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y - Mathf.Floor(Screen.height/ 24), Input.mousePosition.z)); //20
        zoomingIn = true;

        levelSubmitButton.gameObject.SetActive(false);
        bugSelectionUI.SetActive(true);
        bugButtons.SetActive(true);
        bugSelectionText.SetActive(true);

        Utilities.PauseBugs();
        TimerScript.PauseTime();
        //MagnifyGlass.DisableZoom();

        currentBugScript = bug.GetComponent<Bug>();
        selectedBug = currentBugScript.classification;
    }

    //Checks if user correctly identified the highlighted bug. Displays the result as a text popup
    public void BugSelectionUI(string bugName)
    {

        if (selectedBug != null && currentBugScript != null)
        {
            if (selectedBug == bugName)
            {
                bugsCorrectlyIdentified++;
                ScoreScript.AddScore(currentBugScript.points);
                currentBugScript.SetCorrectColor();
                StartCoroutine(Utilities.PopupMessage("Correct!", 2));
                HandleLengthUI();

            } else if (bugName == "Other")
            {
                bugButtons.SetActive(false);
                bugSelectionText.SetActive(false);
                otherUI.SetActive(true);

                InputField other = otherUI.GetComponentInChildren<InputField>();

                otherUI.GetComponentInChildren<Button>().onClick.AddListener(() => OtherSubmit(other));


            }
            else
            {
                currentBugScript.SetIncorrectColor();
                StartCoroutine(Utilities.PopupMessage("Incorrect. The right answer was " + selectedBug, 3));
                HandleLengthUI();
            }
        }

        bugHasBeenCategorized = true;
        bugButtons.SetActive(false);
        bugSelectionText.SetActive(false);


    }

    void OtherSubmit(InputField other)
    {
        lengthUI.SetActive(false);
        if (other.text == selectedBug)
        {
            bugsCorrectlyIdentified++;
            ScoreScript.AddScore(currentBugScript.points);
            StartCoroutine(Utilities.PopupMessage("Correct!", 2));
            otherUI.SetActive(false);
            currentBugScript.SetCorrectColor();
        }
        else
        {
            StartCoroutine(Utilities.PopupMessage("Incorrect. The right answer was " + selectedBug, 3));
            otherUI.SetActive(false);
            currentBugScript.SetIncorrectColor();
        }
        HandleLengthUI();
    }

    void HandleLengthUI()
    {
        bugButtons.SetActive(false);
        bugSelectionText.SetActive(false);

        BranchScript branch = GameObject.Find("Branch").GetComponent<BranchScript>();

        lengthUI.SetActive(true);
        GameObject ruler = GameObject.Find("Ruler");
        Utilities.ScaleRuler(branch, currentBugScript);

        measurementInput = lengthUI.GetComponentInChildren<InputField>();
        measurementInput.onEndEdit.AddListener(delegate { EvaluateMeasurement(measurementInput); });
        if (lengthSubmit != null)
        {
            lengthSubmit.GetComponent<Button>().onClick.AddListener(BugUISubmit);
        }
    }

    //For when the user is done with a branch, also called when timer runs out
    void Submit()
    {
        Sprite background = GameObject.Find("Branch").GetComponent<SpriteRenderer>().sprite;
        Transition.setBackground(background);
        //Iterate to get the next scene
        sceneIterator++;
        //Score is persistant between levels for now, but might want to change this
        levelScore = calcLevelScore();
        totalScore += levelScore;

        TimerScript.SetCurrentTime(findTime(sceneIterator));
        selectedBug = null;

        TimerScript.PauseTime();
        SceneManager.LoadScene(1);

    }

    public void gameOverSubmission(){
      playerScore = ScoreScript.scoreValue;

      //Hide the game interface
      GameObject mainInterface = GameObject.Find("LevelUI");
      mainInterface.SetActive(false);

      //Make the gameover screen visible
      gameOver.SetActive(true);

      //Update the score value and display it to the game over screen
      Text scoreText = GameObject.Find("YourScore").GetComponent<Text>();
      scoreText.text += playerScore.ToString();

      Text totalScoreText = GameObject.Find("TotalScore").GetComponent<Text>();
      totalScoreText.text += totalScore.ToString() + " possible points";

      Text feedbackText = GameObject.Find("Feedback").GetComponent<Text>();
      getFeedback(playerScore, feedbackText);

      //Finds the play again button from the scene and adds an event listener
      playAgainButton = GetComponentInChildren<Button>();
      playAgainAction += PlayAgain;
      playAgainButton.onClick.AddListener(playAgainAction);
    }

    void PlayAgain()
    {
        //Resets the score and goes back to the first scene.
        //Creates new instance of the game manager. Levels should be random again on replay
        ScoreScript.scoreValue = 0;
        ScoreScript.ResetScore();
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    //Can be used for return button as well as after submitting a bug
    private void ReturnFromClick()
    {
        //Reset camera
        //Camera.main.orthographicSize = defaultFOV;
        zoomingIn = false;
        zoomingOut = true;
        Camera.main.transform.position = defaultCameraPosition;

        //Hide bug selection screen and bring back normal UI
        lengthUI.GetComponentInChildren<InputField>().ActivateInputField();
        bugButtons.SetActive(true);
        bugSelectionUI.SetActive(false);
        levelSubmitButton.gameObject.SetActive(true);
        returnObject.SetActive(false);
        TimerScript.ResumeTime();
        Utilities.ResumeBugs();
        //MagnifyGlass.EnableZoom();
        //MagnifyGlass.ResetCounter();
        currentBugScript = null;

        bugHasBeenCategorized = false;
        measurementGiven = false;
    }

    public int levelSelector(int iterator){
        if(iterator == 2){
          currentScene = (int)Mathf.Floor(Random.Range(2,5));
        }
        else if(iterator == 3){
          currentScene = (int)Mathf.Floor(Random.Range(5,8));
        }
        else {
          currentScene = (int)Mathf.Floor(Random.Range(8,11));
        }
        return currentScene;
    }

    //Helper method that iterates through all the bugs on the screen and calculates their potential score value
    private int calcLevelScore()
    {
        int tempScore = 0;
        GameObject bugs = GameObject.Find("SpawnedBugs");//GameObject.FindObjectsOfType<Bug>();
        totalBugs = bugs.transform.childCount;

        string sceneName = SceneManager.GetActiveScene().name;
        string branchDifficulty = sceneName.Substring(0, 4);

        return setScore(branchDifficulty);
    }

    private int setScore(string difficulty)
    {
        int randomNumber;
        if(difficulty == "Easy")
        {
           return 300;

        }else if(difficulty == "Medi")
        {
            return 600;
        }
        else
        {
            return 900;
        }
    }

    private void EvaluateMeasurement(InputField input){
        GameObject[] rulers = GameObject.FindGameObjectsWithTag("Ruler");
        foreach(GameObject ruler in rulers){
          GameObject.Destroy(ruler);
        }

        float approximatedBugLength = float.Parse(input.text);
        float actualBugLength = Mathf.Round(currentBugScript.lengthInMM);
        float measurementError = Mathf.Abs(Mathf.Round(approximatedBugLength - actualBugLength));
        measurementDistance += Mathf.Abs(actualBugLength - approximatedBugLength);
        measurementDistance += measurementError;

        float minBound = 0;
        float maxBound = actualBugLength * 2;
        int scoreValue = 0;

        if(approximatedBugLength >= maxBound){
          //Do nothing
        } else if (approximatedBugLength >= actualBugLength){

          float distance = maxBound - approximatedBugLength;
          float accuracyPercent = distance/actualBugLength;
          scoreValue = (int)Mathf.Round(accuracyPercent * (float)currentBugScript.points);

        } else if (approximatedBugLength > minBound){

          float distance = approximatedBugLength;
          float accuracyPercent = distance/actualBugLength;
          scoreValue = (int)Mathf.Round(accuracyPercent * (float)currentBugScript.points);
        }

        StartCoroutine(Utilities.PopupMessage("Actual size: " + actualBugLength + "mm" + "\n" +
                                              "Your measurement: " + approximatedBugLength + "mm" + "\n" +
                                            "Points awarded: " + scoreValue, 3));
        measurementGiven = true;

        input.text = "Length";
        input.DeactivateInputField();
        measurementInput = null;
        ScoreScript.AddScore(scoreValue);
    }

    private void BugUISubmit(){
      if(measurementGiven && bugHasBeenCategorized){
        lengthUI.SetActive(false);
        ReturnFromClick();
      } else {
        //Might want to send an alert to the user eventually
        //StartCoroutine(Utilities.PopupMessage("Must select a bug type and measurement", 2));
      }
    }

    private void getFeedback(int score, Text feedback){
      string tmp;
      if(score >= 1600){
        feedback.text = "Great job! You're ready to get outside and conduct some real surveys!";
      } else if (score >= 1300){
        feedback.text = "You are a budding entomologist! Can you get a perfect score?";
      } else if (score >= 800){
        feedback.text = "Keep practicing! Finding more bugs will bring you eternal happiness!";
      } else if (score >= 500){
        feedback.text = "Review the Arthropod ID Guide, and don't forget to use the magnifying glass to help you search!";
      } else {
        feedback.text = "Um, you do know what an arthropod is, don't you?";
      }

    }

    private int findTime(int iterator){
      if(iterator == 2){
        return 60;
      }
      if(iterator == 3){
        return 60;
      }
      if(iterator == 4){
        return 60;
      } else {
        return 60;
      }
    }

    public void ResetBugCounts(){
      if(bugsClicked != null){
        bugsClicked = 0;
      }
      if(totalBugs != null){
        totalBugs = 0;
      }
      if(bugsCorrectlyIdentified != null){
        bugsCorrectlyIdentified = 0;
      }
      if(measurementDistance != null){
        measurementDistance = 0;
      }
    }

}
