using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is responsible for processing textures and superimposing them into
 * 	   game assets.
 */
public class TextureAssembler : MonoBehaviour {

	public Texture2D texture;
	public GameObject cube; //temp

	/*******************************************************************************************************************************/

	//The raw images downloaded from the internet
	private ArrayList rawTextures;

	private Color replacement = new Color (1, 0, 1, 1);

	/*******************************************************************************************************************************/


	/*
	 * This function is to be called outside of this class.
	 */
	public void StartProcessing(ArrayList rawTextures){
		this.rawTextures = rawTextures;

		GameObject spawned = (GameObject)Instantiate (cube, Vector3.zero, new Quaternion(0,0,180,0));
		spawned.GetComponent<Renderer> ().material.mainTexture = SuperImposeTexture (texture, (Texture2D)rawTextures [0]);
		spawned.GetComponent<Renderer> ().material.shader = Shader.Find ("Unlit/Texture");
	}


	/*
	 * Superimpose a texture onto another texture with magic pink values (255, 0, 255)
	 */
	public Texture2D SuperImposeTexture(Texture2D baseTex, Texture2D overlayTex){
		Texture2D newTex = new Texture2D (baseTex.width, baseTex.height);

		newTex.SetPixels32 (baseTex.GetPixels32 ());

		for (int x = 0; x < newTex.width; x++) {
			for (int y = 0; y < newTex.height; y++) {				
				if (newTex.GetPixel (x, y) == replacement) {
					newTex.SetPixel (x, y, GetSafePixel ((Texture2D)overlayTex, x, y));
				}
			}
		}

		newTex.Apply ();

		return newTex;

	}

	/*
	 * If this function is trying to call an invalid pixel it will wrap around to a valid one
	 */
	public Color GetSafePixel(Texture2D tex, int x, int y){
		if (x % (tex.width * 2) > tex.width) {
			x = tex.width - (x % tex.width);
		} else {
			x = x % tex.width;
		}

		if (y % (tex.height * 2) > tex.height) {
			y = tex.height - (y % tex.height);
		} else {
			y = y % tex.height;
		}

		return tex.GetPixel (x, y);
	}


}
