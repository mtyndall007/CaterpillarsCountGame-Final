
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchScript : MonoBehaviour
{

    public GameObject branch;
    public float inGameBranchWidth;
    public int branchWidthInMM;

    private float millimetersToInGameUnits;

    private const int rulerInMM = 56;
    private float rulerWidthHeightRatio;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBranchUnits();
        //ResizeRuler();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void spawnBugs(int bugCount)
    {
        for (int i = 0; i < bugCount; i++)
        {

        }
    }

    private void InitializeBranchUnits()
    {

        branch = gameObject;
        RectTransform branchRect = gameObject.AddComponent(typeof(RectTransform)) as RectTransform;
        SpriteRenderer temp = branch.GetComponentInChildren<SpriteRenderer>();
        Sprite tempSprite = temp.sprite;
        if (tempSprite)
          {
              rulerWidthHeightRatio = tempSprite.rect.width/tempSprite.rect.height;
              //This should normally be temtempSprite.rect.width, but the image was rotated 90 degrees in original position
              inGameBranchWidth = tempSprite.rect.height;
              Debug.Log("Branch width in game: " + inGameBranchWidth);
              millimetersToInGameUnits = branchWidthInMM / inGameBranchWidth;
              Debug.Log("MM to ingame: " + millimetersToInGameUnits);
          }
        else
          {
              Debug.Log("Need a branch sprite");
          }
    }

    private void ResizeRuler(){
      GameObject ruler = GameObject.Find("Ruler");
      RectTransform rulerSprite = ruler.GetComponent<RectTransform>();
      //Ruler was measured at 57mm
      float inGameRulerWidth = (rulerInMM/millimetersToInGameUnits);
      rulerSprite.sizeDelta = new Vector2(inGameRulerWidth, inGameRulerWidth/rulerWidthHeightRatio);
      Debug.Log("In game ruler width: " + inGameRulerWidth);


      Debug.Log("Ruler in game:" + rulerSprite.rect.width);
    }

}
