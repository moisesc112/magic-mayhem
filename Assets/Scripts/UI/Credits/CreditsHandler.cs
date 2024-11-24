using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CreditsHandler : MonoBehaviour
{
    // based on logic from https://www.youtube.com/watch?v=_kWvZ7_m78U&t=1538s 

    [SerializeField] GameObject background;
    [SerializeField] string path = "Assets/Scripts/UI/Credits/Credits.txt";
    [SerializeField] Font font;
    [SerializeField] Color headerColor = Color.red;
    [SerializeField] Color nameColor = Color.white;
    [SerializeField] int headerSize = 35;
    [SerializeField] int nameSize = 25;
    [SerializeField] float scrollSpeed = 150;
    [SerializeField] int screenSpaceInDivision = 8;

    List<string> headers = new List<string>();
    List<List<string>> titles = new List<List<string>>();
    List<GameObject> creditTexts = new List<GameObject>();
    bool isEnded = false;

    public event EventHandler<GenericEventArgs<bool>> CreditsEnded;

    private void StartCredits()
    {
        background.SetActive(true);
        var lastPosition = new Vector3(Screen.width * .5f, 0, 0);
        for (var i = 0; i < headers.Count; i++)
        {
            var headerTextGameObject = NewTextGameObject(headers[i], true);
            var nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / screenSpaceInDivision), 0);
            headerTextGameObject.transform.position = nextPosition;
            lastPosition = nextPosition;
            creditTexts.Add(headerTextGameObject);
            for (var j = 0; j < titles[i].Count; j++)
            {
                nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / screenSpaceInDivision), 0);
                var textGameObject = NewTextGameObject(titles[i][j], false);
                textGameObject.transform.position = nextPosition;
                lastPosition = nextPosition;
                creditTexts.Add(textGameObject);
            }
        }
        isEnded = false;
    }

    private void Awake()
    {
        StreamReader streamReader = new StreamReader(path);
        var currentLine = string.Empty;
        while ((currentLine = streamReader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(currentLine))
            {
                continue;
            }
            var firstCharacter = currentLine.Substring(0, 1);
            var isIgnore = firstCharacter == "#";
            var isHeader = firstCharacter == "!";
            if (isIgnore)
            {
                continue;
            }
            if (isHeader)
            {
                headers.Add(currentLine.Substring(1));
                titles.Add(new List<string>());
            }
            else
            {
                titles[titles.Count - 1].Add(currentLine);
            }
        }
        streamReader.Close();
    }

    public void OnStartCredits()
    {
        creditTexts.Clear();
        StartCredits();
    }

    private GameObject NewTextGameObject(string labelText, bool isHeader)
    {
        var textGameObject = new GameObject(labelText);
        textGameObject.transform.SetParent(transform);
        var textComponent = textGameObject.AddComponent<Text>();
        textComponent.text = labelText;
        textComponent.font = font;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        textComponent.alignment = TextAnchor.MiddleCenter;
        if (isHeader)
        {
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.color = headerColor;
            textComponent.fontSize = headerSize;
        }
        else
        {
            textComponent.color = nameColor;
            textComponent.fontSize = nameSize;
        }
        textGameObject.transform.localScale = Vector3.one;
        textGameObject.transform.position = new Vector3(Screen.width * .5f, Screen.height * .25f, 0);
        return textGameObject;
    }

    private void Update()
    {
        foreach (var text in creditTexts)
        {
            if (text == null) continue;

            text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y + scrollSpeed * Time.deltaTime, 0);
            if (text.transform.position.y > Screen.height * 1.2f)
            {
                Destroy(text);
            }
        }
        creditTexts.RemoveAll(x => x.transform.position.y > Screen.height * 1.2f);
        if (!isEnded && creditTexts.Count == 0)
        {
            isEnded = true;
            background.SetActive(false);
            CreditsEnded?.Invoke(sender: this, new GenericEventArgs<bool>(true));
        }
    }
}
