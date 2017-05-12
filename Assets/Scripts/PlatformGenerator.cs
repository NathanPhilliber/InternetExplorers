using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {

	//The how wide this chunk should be
	public int width;

	//How tall this chunk should be
	public int height;

	//How many platforms should be spawned?
	public float density;

	//The platform to be spawned (contains many variations within)
	public GameObject platform;

	//Where this section of platforms should be enterable
	public int enterPointY;


}
