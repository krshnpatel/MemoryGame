using UnityEngine;
using System.Collections;

public class SetScreenResolution : MonoBehaviour
{
	void Start ()
	{
		Screen.SetResolution (1024, 576, true);
	}
}
