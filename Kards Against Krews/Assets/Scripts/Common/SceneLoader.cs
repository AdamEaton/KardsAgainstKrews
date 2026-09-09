using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneLoader : CustomBehaviour
{
	public string scene;

	public void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}
	public void LoadSceneDefault()
	{
		SceneManager.LoadScene(scene);
	}
}
