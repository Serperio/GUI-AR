using UnityEngine;
using System.Collections;

public class SimpleSingleton : MonoBehaviour
{
	private static SimpleSingleton instance = null;

	// Game Instance Singleton
	public static SimpleSingleton Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		// if the singleton hasn't been initialized yet
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}

		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	void Update()
	{
		//Debug.Log("SINGLETON UPDATE");
	}
}