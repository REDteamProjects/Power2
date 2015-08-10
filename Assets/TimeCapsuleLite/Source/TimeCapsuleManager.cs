using UnityEngine;
using System.Collections;
using Whisper;

/// <summary>
/// Time capsule manager.
/// </summary>
public class TimeCapsuleManager : MonoBehaviour {

	//*******************************************//
	// Public Member Variables
	//*******************************************//

	/// <summary>
	/// The directory to save to.
	/// </summary>
	public string directory;

	//*******************************************//
	// Private Member Variables
	//*******************************************//

	/// <summary>
	/// The static instance of the <see cref="TimeCapsuleManager"/> class.
	/// </summary>
	private static TimeCapsuleManager g_instance;

	//*******************************************//
	// Public Member Methods
	//*******************************************//

	/// <summary>
	/// Save the specified data, with the specified filename at the specified directory.
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="fileName">File name.</param>
	/// <param name="directory">Directory.</param>
	public void Save (object data, string fileName, string directory, string save) { SaveHandler.Save (data, fileName, directory, save); }

	/// <summary>
	/// Load the specified filename at the specified directory.
	/// </summary>
	/// <param name="fileName">File name.</param>
	/// <param name="directory">Directory.</param>
	/// <typeparam name="T">Type of object to load.</typeparam>
	public T Load<T> (string fileName, string directory, string save) { return LoadHandler.Load<T>(fileName, directory, save); }

	//*******************************************//
	// Singleton
	//*******************************************//

	/// <summary>
	/// Gets or sets the singleton instance of the <see cref="TimeCapsuleManager"/> class.
	/// </summary>
	/// <value>The single instance of the <see cref="TimeCapsuleManager"/> class.</value>
	public static TimeCapsuleManager Instance {

		get {
			if (g_instance == null) {
				g_instance = (TimeCapsuleManager)GameObject.FindObjectOfType(typeof(TimeCapsuleManager));
			}

			return g_instance;
		}

		set { g_instance = null; }
	}
}
