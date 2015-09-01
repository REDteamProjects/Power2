using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public class MyBuildPostprocessor 
{
	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
	{ 		
		string objCPath = Application.dataPath + "/WebPlayerTemplates";
		Process myCustomProcess = new Process();		
		myCustomProcess.StartInfo.FileName = "python";
		myCustomProcess.StartInfo.Arguments = string.Format("Assets/Editor/post_process.py \"{0}\" \"{1}\"", pathToBuiltProject, objCPath);
		myCustomProcess.StartInfo.UseShellExecute = false;
		myCustomProcess.StartInfo.RedirectStandardOutput = false;
		myCustomProcess.Start(); 
		myCustomProcess.WaitForExit(); 		
	}
}
