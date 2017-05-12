using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Responsible for Scraping a URL and downloading assets.
 * 
 */
public class AssetGrabber : MonoBehaviour {

	//Url to get html from
	public string url;

	//Look for links following "href" and "src"
	private string[] linkStarters = new string[] {"href=", "src="};

	//If the link contains any of these then it must be an image
	private string[] imageExtensions = new string[] {".png", ".jpg", ".jpeg", ".gif", ".bmp", ".img", ".tiff", ".ico"};

	//If the link contains any of these then we don't want to try and load this texture.
	private string[] restrictedImageExtensions = new string[] {".gifv", "domain/"};

	//If the link contains any of these then we will not recursively call it
	private string[] restrictedLinkExtensions = new string[] {".js", ".css"};

	//How many subpages do we want to process?
	private int maxRecursiveCalls = 10;

	/*******************************************************************************************************************************/

	//All the textures found from the webpage and child pages
	private ArrayList textures = new ArrayList ();

	//All the links to the textures found from the webpage and child pages
	private ArrayList textureLinks = new ArrayList ();

	//The number of items currently being downloaded
	private int currentlyDownloading = 0;

	//Has OnDownloadComplete been called yet?
	private bool downloadProcessed = false;

	//The number of subpages that have been processed
	private int numberOfRecursiveCalls = 0;

	/*******************************************************************************************************************************/

	/*
	 * Unity Update function
	 */
	public void Update(){		
		CheckForDownloadComplete ();

	} //End of Update


	/*
	 * Unity Start function
	 */
	public void Start(){
		StartCoroutine (ExtractFromUrl (url, true));

	} //End of Start


	/*
	 * Called when the downloads are all complete
	 */
	public void OnDownloadComplete(){
		GetComponent<TextureAssembler> ().StartProcessing (textures);
	} //End of OnDownloadComplete


	/*
	 * Check to see if we are done downloading everything.
	 * 
	 */
	private void CheckForDownloadComplete(){
		if(downloadProcessed == false && textures.Count > 0 && currentlyDownloading == 0){
			downloadProcessed = true;
			OnDownloadComplete ();
		}
	} //End of CheckForDownloadComplete


	/*
	 * Download and create a WWW object from a url.
	 * Start processing after download is complete.
	 * 
	 * string url: the url to extract from
	 * bool recursive: should we also process links within this page?
	 */
	private IEnumerator ExtractFromUrl(string url, bool recursive){
		WWW www = new WWW (url);
		yield return www;

		if (www.error == null) {
			ProcessWWW (www, recursive);
		}

	} //End of ExtractFromURL


	/*
	 * Process a WWW object
	 * 
	 * WWW page: the downloaded www page to be processed
	 * bool recursive: should we also process links within this page?
	 */
	private void ProcessWWW(WWW page, bool recursive){
		
		//Convert the page to a string
		string pageTextRaw = page.text;

		//Place to store all kinds of links
		ArrayList allLinks = new ArrayList ();

		//Place to store all image links
		ArrayList images = new ArrayList ();

		//Place to store all links
		ArrayList links = new ArrayList ();

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
					string link = CleanupLink(line.Substring (startQuote + 1, endQuote - startQuote - 1));

					if (IsValidURL (link)) {
						allLinks.Add (link);
					}
				}
			}
		}

		//Sort out the images from the page links
		foreach (string link in allLinks) {

			if(!ContainsExtension(link, restrictedImageExtensions) && ContainsExtension(link, imageExtensions)){

				//Make sure we aren't storing duplicates
				if (images.Contains (link) == false) {					
					if (textureLinks.Contains (link) == false) {
						images.Add (link);

						//Start the coroutine to save the texture
						StartCoroutine (GrabTexture (link));
					}
				}
			}

			//This must be a page link
			else {

				//Don't store duplicate links
				if (links.Contains (link) == false) {
					links.Add (link);

					//If this is the first call, then call on subpages 
					if (numberOfRecursiveCalls < maxRecursiveCalls && recursive && !ContainsExtension(link, restrictedLinkExtensions)) {
						numberOfRecursiveCalls++;

						StartCoroutine (ExtractFromUrl (link, false));
					}
				}
			}
		}

	} //End of ProcessWWW


	/*
	 * Return true if any key is a substring of haystack.
	 */
	private bool ContainsExtension(string haystack, string[] key){
		//Try each image type
		foreach (string ext in key) {

			//If the string has the extension then this is an image
			if (haystack.ToUpper().Contains (ext.ToUpper())) {
				return true;
			} 					
		}

		return false;
	} //End of ContainsExtension
		

	/*
	 * Gets the texture from a link and stores it in global arraylist.
	 * 
	 * string link: the link to the image.
	 */
	private IEnumerator GrabTexture(string link){
		WWW www = new WWW (link);
		currentlyDownloading++;
		yield return www;
		currentlyDownloading--;

		if (www.error == null) {
			textures.Add (www.texture);
			textureLinks.Add (link);
		}

	} //End of GrabTexture


	/*
	 * Misc cleanup for a link.
	 */
	private string CleanupLink(string link){
		while (link [0] == '/') {
			link = link.Substring (1);
		}

		return link;
	} //End of CleanupLink


	/*
	 * Misc checks for a valid url.
	 */
	private bool IsValidURL(string link){
		if (link [0] == '#') {
			return false;
		}

		if (link.Contains (".") == false) {
			return false;
		}

		return true;
	} //End of IsValidURL
}
	