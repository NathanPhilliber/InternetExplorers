using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Responsible for Scraping a URL and downloading assets.
 * 
 */
public class AssetGrabber : MonoBehaviour {

	public string url;

	/*
	 * Download and create a WWW object from a url.
	 * Start processing after download is complete.
	 */
	IEnumerator Start(){
		WWW www = new WWW (url);
		yield return www;

		ProcessWWW (www);
	}

	/*
	 * Process a WWW object
	 */
	private void ProcessWWW(WWW page){
		
		//Convert the page to a string
		string pageTextRaw = page.text;

		//Place to store all links
		ArrayList links = new ArrayList ();

		//Look for links following "href" and "src"
		string[] linkStarters = new string[] {"href=", "src="};

		//Split the page string based on linkStarters
		string[] result = pageTextRaw.Split (linkStarters, System.StringSplitOptions.RemoveEmptyEntries);

		//Now trim the strings that contain links to just links
		foreach (string line in result) {

			//Find the first quote
			int startQuote = line.IndexOf ("\"");

			//If the first quote exists and it's not at the end of the string then continue.
			if (startQuote >= 0 && startQuote + 1 < line.Length) {				

				//Find the next quote after the first quote
				int endQuote = line.IndexOf ("\"", startQuote + 1);

				//If there's another end quote and the link is at least 6 characters long then continue.
				if (endQuote >= 1 && endQuote - startQuote > 6) {

					//Add the link to the arraylist, trimming out the quotations and other junk characters.
					links.Add (line.Substring (startQuote + 1, endQuote - startQuote - 1));
				}
			}
		}

		for (int i = 0; i < links.Count ; i++) {
			Debug.Log (links [i]);
		}

	}

}
	