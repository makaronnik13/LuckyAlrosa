using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestReplacer : MonoBehaviour {

	public List<GameObject> gameObjects = new List<GameObject>();
	private List<GameObject> selectedGameObjects = new List<GameObject>();

    public Scenario scenario;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Replace", 0, 4);
	}

	void Replace(){
		if(gameObjects.Count<2)
		{
			CancelInvoke ("Replace");
		}

		foreach(GameObject go in selectedGameObjects){
			FindObjectOfType<MaterialReplacer> ().DeselectItem (go);
			gameObjects.Remove (go);
		}
		FindObjectOfType<MaterialReplacer> ().SelectItem (gameObjects[0]);
		selectedGameObjects.Clear ();
		selectedGameObjects.Add (gameObjects[0]);
	}

}
