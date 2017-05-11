using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Responsible for Scraping a URL and downloading assets.
 * 
 */
public class AssetGrabber : MonoBehaviour {

	public string url;


	IEnumerator Start(){
		WWW www = new WWW (url);
		yield return www;

		ProcessWWW (www);
	}


	private void ProcessWWW(WWW page){
		string pageTextRaw = page.text;

		ArrayList links = new ArrayList ();
		string[] linkStarters = new string[] {"href=", "src="};

		string[] result = pageTextRaw.Split (linkStarters, System.StringSplitOptions.RemoveEmptyEntries);

		foreach (string line in result) {
			int startQuote = line.IndexOf ("\"");

			if (startQuote >= 0 && startQuote + 1 < line.Length) {				
				int endQuote = line.IndexOf ("\"", startQuote + 1);

				if (endQuote >= 1 && endQuote - startQuote > 6) {
					links.Add (line.Substring (startQuote + 1, endQuote - startQuote - 1));
				}
			}
		}

		for (int i = 0; i < links.Count ; i++) {
			Debug.Log (links [i]);
		}

	}

}
	