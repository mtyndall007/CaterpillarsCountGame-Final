using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Generic script for bugs. Can be used as a generic parent class for more specific bugs, but not sure if it's necessary yet.
//Public variables so that we can tweak what color each bug is, their point value, etc.

[System.Serializable]
public class BugClickedEvent : UnityEvent<GameObject>
{
}

public class Bug : MonoBehaviour
{
    public GameObject bug;
    private bool clickable;
    public bool paused;
    public Color defaultColor;
    public int points;
    public string classification;
    public float lengthInMM;
    public BugClickedEvent bugClicked;
    public bool randomlyRotatable;

    public float minLengthInMM;
    public float desiredLengthInMM;
    public float maxLengthInMM;
    public float lengthAsProportionOfImageWidth;

    public static bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        bug = gameObject;
        clickable = true;
        paused = false;


        GameObject gameManagerObject = GameObject.Find("GameManager");
        GameObject SpawnedBugs = GameObject.Find("SpawnedBugs");

        string sceneName = SceneManager.GetActiveScene().name;
        string difficulty = sceneName.Substring(0, 4);
        int totalBugs = SpawnedBugs.transform.childCount;

        if (difficulty == "Easy")
        {
            points = 300/totalBugs/3;

        }
        else if (difficulty == "Medi")
        {
            points = 600 / totalBugs /3;
        }
        else
        {
            points = 900 / totalBugs /3;
        }



        //Will need something like this eventually, but also need to scale based on branch size
        //lengthInMM = lengthInMM * bug.transform.localScale.x;
        //Debug.Log(lengthInMM);

        if (bugClicked == null)
            bugClicked = new BugClickedEvent();

        bugClicked.AddListener(GameManager.instance.bugClicked);
    }

    // Update is called once per frame
    void Update()
    {

        checkForClick();

    }

    public void PauseBug()
    {
        isPaused = true;
        paused = true;
        //MagnifyGlass.DisableZoom();
        //Debug.Log(MagnifyGlass.IsZoomable());
    }

    public void ResumeBug()
    {
        isPaused = false;
        paused = false;


    }

    public void SetCorrectColor()
    {
        SpriteRenderer spriteRenderer = bug.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.blue;
    }

    public void SetIncorrectColor()
    {
        SpriteRenderer spriteRenderer = bug.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
    }

    public bool isClickable(){
      return clickable;
    }

    private void checkForClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pointClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D coll = bug.GetComponent<Collider2D>();

            if (coll.OverlapPoint(pointClicked) && clickable && !paused)
            {
                //Sends the bug name to game manager when clicked
                //Send game object to the game manager so it has a reference
                bugClicked.Invoke(bug);

                ScoreScript.scoreValue += points;
                ScoreScript.levelScore += points;
                clickable = false;

            }
        }
    }

    //Have not tested yet
    private void randomlyRotate(Bug buggy)
    {
        if (buggy.randomlyRotatable)
        { 
            buggy.transform.Rotate(0,0, Random.Range(0, 360));
        }
    }
}
