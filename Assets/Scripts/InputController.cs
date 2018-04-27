using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController {
    public Match match;
    public Canvas canvas;
    public EventSystem eventSys;
    public Button endTurnButton;
	
    public InputController(Match matchParam) {
        match = matchParam;
        GameObject canvasPrefab = Resources.Load<GameObject>("Prefabs/Canvas");
        GameObject eventSysPrefab = Resources.Load<GameObject>("Prefabs/EventSystem");
        GameObject endTurnButtonPrefab = Resources.Load<GameObject>("Prefabs/EndTurn Button");
        canvas = GameObject.Instantiate(canvasPrefab).GetComponent<Canvas>();
        eventSys = GameObject.Instantiate(eventSysPrefab).GetComponent<EventSystem>();
        endTurnButton = GameObject.Instantiate(endTurnButtonPrefab, canvas.transform).GetComponent<Button>();
        canvas.name = "Canvas";
        eventSys.name = "EventSystem";
        endTurnButton.name = "EndTurn Button";

        endTurnButton.onClick.AddListener(EndTurnButtonClicked);
    }

    public void EndTurnButtonClicked()
    {
        match.EndCurrentPlayerTurn();
    }
}
