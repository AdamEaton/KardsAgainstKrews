using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadSceneButtonDecorator : UIButtonDecorator
{
	public string sceneToLoad;

	public override void OnButtonClicked()
	{
		SceneManager.LoadScene(sceneToLoad);
	}
}
