#region

using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace ViewTo.Connector.Unity
{
	public class ConsoleToGUI : MonoBehaviour
	{
		bool doShow = true;
		string filename = "";
		int kChars = 700;
		string myLog = "*begin log";

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space)) doShow = !doShow;
		}

		void OnEnable()
		{
			Application.logMessageReceived += Log;
		}

		void OnDisable()
		{
			Application.logMessageReceived -= Log;
		}

		void OnGUI()
		{
			if (!doShow) return;

			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
			                           new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
			GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
		}

		public void Log(string logString, string stackTrace, LogType type)
		{
			// for onscreen...
			myLog = myLog + "\n" + logString;
			if (myLog.Length > kChars) myLog = myLog.Substring(myLog.Length - kChars);

			// for the file ...
			if (filename == "")
			{
				var d = Environment.GetFolderPath(
					        Environment.SpecialFolder.Desktop)
				        + "/YOUR_LOGS";
				Directory.CreateDirectory(d);
				var r = Random.Range(1000, 9999).ToString();
				filename = d + "/log-" + r + ".txt";
			}

			try
			{
				File.AppendAllText(filename, logString + "\n");
			}
			catch
			{ }
		}
	}
}