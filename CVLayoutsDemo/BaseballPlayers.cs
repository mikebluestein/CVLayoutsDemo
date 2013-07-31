using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MonoTouch.Foundation;

namespace CVLayoutsDemo
{
	public enum Team { One, Two }

	// Loads a collection of speakers containing file paths of speaker images
	public class BaseballPlayers : List<BaseballPlayer>
	{
		public BaseballPlayers (Team team)
		{
			string folder;

			if (team == Team.One)
				folder = "Players1";
			else
				folder = "Players2";

			Regex pattern = new Regex (@"^.*\.(jpg|png)$", RegexOptions.IgnoreCase);
			string path = Path.Combine (NSBundle.MainBundle.BundlePath, folder);
			
			Directory.GetFiles (path).Where (f => pattern.IsMatch (f)).ToList ().ForEach (p => {
				BaseballPlayer s = new BaseballPlayer{ImageFile = folder + "/" + Path.GetFileName(p)};
				this.Add (s);
			});
		}
	}

	public class BaseballPlayer
	{
		public string ImageFile { get; set; }
	}
}