using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawningScript : MonoBehaviour
{

    //Lets you adjust how many bugs you want on that branch through inspector
    public int numOfBugs;

    public BranchScript branch;

    void Start()
    {
        //GameObject holding spawned bugs
        Transform spawnedBugs = GameObject.Find("SpawnedBugs").transform;

        branch = GameObject.Find("Branch").GetComponent<BranchScript>();

        //Sets number of bugs based on difficulty
        string sceneName = SceneManager.GetActiveScene().name;
        string branchDifficulty = sceneName.Substring(0, 4);
        setNumWithDifficulty(branchDifficulty);

        //catches exceptions with the number of bugs
        if (numOfBugs > transform.childCount)
        {
            numOfBugs = transform.childCount;
        }
        else if (numOfBugs < 0)
        {
            numOfBugs = 0;
        }


        //keeps track of which bugs are already added to scene
        HashSet<Transform> alreadyAdded = new HashSet<Transform>();


        //Loop to add bugs
        for (int i = 0; i < numOfBugs; i++)
        {


            //randomly picks a spawnPoint
            int point = Random.Range(0, transform.childCount);
            Transform spawnPoint = transform.GetChild(point);

            //while loop checks if spawn point has been used
            //IF has been used, finds one that hasnt
            while(alreadyAdded.Contains(spawnPoint))
            {
                point = Random.Range(0, transform.childCount);
                spawnPoint = transform.GetChild(point);
            }

            //adds spawnpoint to hashset
            alreadyAdded.Add(spawnPoint);

            //randomly picks bug that can spawn from that spawn point
            int bugIndex = Random.Range(0, spawnPoint.childCount);
            Transform bug = spawnPoint.GetChild(bugIndex);

            //duplicates it and adds it to the spawnBugs
            Transform newBug = Instantiate(bug);

            //sets the bugs position
            Vector3 bugPosition = spawnPoint.position + newBug.position;
            newBug.transform.position = bugPosition;
            newBug.gameObject.SetActive(true);

            //adds the bug to scene and makes it visible
            newBug.parent = spawnedBugs;
            newBug.gameObject.SetActive(true);
            Utilities.ScaleBug(branch, newBug.gameObject);

        }

    }

    //Getters and Setters
    public int getNumOfBugs() { return numOfBugs; }
    public void setNumOfBugs(int value) { numOfBugs = value; }

    //Sets the number of bugs based off the difficulty of the level
    /*
     * Pass in name of file to see what level it is and picks a value associated with that level?
     */
    public void setNumWithDifficulty(string difficulty)
    {
        int randomNumber;
        if(difficulty == "Easy")
        {
           randomNumber =  Random.Range(3, 5);

        }else if(difficulty == "Medi")
        {
            randomNumber = Random.Range(4, 7);
        }
        else
        {
            randomNumber =  Random.Range(6, 10);
        }

        setNumOfBugs(randomNumber);
        Debug.Log(numOfBugs);
    }




}
