using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class URLButton : MonoBehaviour
{
  public Button urlButton;

  void Start()
  {
      urlButton.onClick.AddListener(() => ButtonClicked());

  }

  private void ButtonClicked()
  {
      Application.OpenURL("https://caterpillarscount.unc.edu/pdfs/ArthropodIDGuide.pdf");

  }
}
