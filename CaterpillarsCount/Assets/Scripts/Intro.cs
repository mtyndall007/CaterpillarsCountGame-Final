using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    Button playButton;
    UnityAction playAction;

    void Start()
    {
      MagnifyGlass.DisableZoom();

      playButton = gameObject.GetComponentInChildren<Button>();
      playAction += Play;
      playButton.onClick.AddListener(playAction);
    }

    private void Play(){
      Debug.Log("Play button pressed");
      Destroy(gameObject);
      SceneManager.LoadScene(2);
    }
}
