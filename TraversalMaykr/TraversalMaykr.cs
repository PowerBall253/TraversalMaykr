using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TraversalMaykr
{
	class TraversalMaykr
	{
		private static string[] AvailableAnims = {
			"jump_forward_100", "jump_forward_1000", "jump_forward_1000_down_1000", "jump_forward_1000_up_1000", "jump_forward_500", "jump_forward_500_down_500", "jump_forward_500_up_500", "ledge_down_100",
			"ledge_down_1000", "ledge_down_500", "ledge_up_100", "ledge_up_1000", "ledge_up_500", "rail_down_100", "rail_down_1000", "rail_down_500", "rail_down_700", "rail_up_100", "rail_up_1000", "rail_up_500"
		};

		private static string[] SmallDemons = {
			"gargoyle", "imp", "soldier_blaster", "zombie_maykr", "zombie_tier_1", "zombie_tier_3", "prowler",
			"carcass", "whiplash", "hell_knight", "dread_knight", "revenant", "marauder", "marauder_wolf"
		};

		private static void Main(string[] args)
		{
			Console.WriteLine("TraversalMaykr v1.0 by PowerBall253\n\n");
			bool firstTime = true;

			while (true)
			{
				double[][] coordsArray = GetCoords();

				if (coordsArray == null)
				{
					continue;
				}

				double[] coords = coordsArray[0];
				double[] reverseCoords = coordsArray[1];

				bool? onlySmallDemons = null;
				Console.WriteLine("Available monsters:\n");
				Console.WriteLine("[1] Small demons (" + String.Join(", ", SmallDemons) + ")");
				Console.WriteLine("[2] All demons");
				Console.WriteLine("\n");

				while (!onlySmallDemons.HasValue)
				{
					Console.Write("Input the number of the monsters you want to generate anims for: ");
					int demonsIndex = -1;

					if (!Int32.TryParse(Console.ReadLine().Trim(), out demonsIndex) || demonsIndex <= 0 || demonsIndex > SmallDemons.Length)
					{
						Console.Error.WriteLine("Invalid value!");
					}
					else
					{
						onlySmallDemons = (demonsIndex == 1) ? true : false;
					}
				}

				string traversalAnim = String.Empty;
				Console.WriteLine("Available anims:\n");

				for (int i = 1; i < AvailableAnims.Length + 1; i++)
				{
					Console.WriteLine($"[{i}] {AvailableAnims[i - 1]}");
				}

				Console.WriteLine();

				while (String.IsNullOrEmpty(traversalAnim))
				{
					Console.Write("Input the number of the anim you want to generate: ");
					int traversalAnimIndex = -1;

					if (!Int32.TryParse(Console.ReadLine().Trim(), out traversalAnimIndex) || traversalAnimIndex <= 0 || traversalAnimIndex > AvailableAnims.Length)
					{
						Console.Error.WriteLine("Invalid value!");
						continue;
					}

					traversalAnimIndex--;
					traversalAnim = AvailableAnims[traversalAnimIndex];
				}

				List<string> traversalInfo = GetTraversalInfo(File.ReadAllText($"traversal{Path.DirectorySeparatorChar}entitydef.txt"), traversalAnim, onlySmallDemons.GetValueOrDefault(), coords, reverseCoords);
				bool append = !firstTime && File.Exists("Generated Traversals.txt") ? true : false;
				firstTime = false;

				using (StreamWriter streamWriter = new StreamWriter("Generated Traversals.txt", append))
				{
					for (int i = 0; i < traversalInfo.Count; i++)
					{
						streamWriter.Write(traversalInfo[i]);
					}
				}

				Console.WriteLine("Successfully generated traversal info!\n");
				Console.WriteLine("Press any key to continue...\n");
				Console.ReadKey();
			}
		}

		private static List<string> GetTraversalInfo(string traversalInfo, string anim, bool onlySmallDemons, double[] coords, double[] reverseCoords)
		{
			List<string> traversals = new List<string>();
			Random random = new Random();

			foreach (string traversalFilePath in Directory.GetFiles("traversal", "*.tin"))
			{
				if (onlySmallDemons && !SmallDemons.Contains<string>(Path.GetFileNameWithoutExtension(traversalFilePath)))
				{
					continue;
				}

				string demonTraversalInfo = traversalInfo;
				string reverseDemonTraversalInfo = traversalInfo;
				string[] traversalInfoFile = File.ReadAllLines(traversalFilePath);
				string traversalAnim = GetTraversalAnim(anim, traversalInfoFile.Skip(3).ToArray());

				if (String.IsNullOrEmpty(traversalAnim))
				{
					continue;
				}

				string reverseTraversalAnim = GetReverseTraversalAnim(traversalAnim);

				demonTraversalInfo = demonTraversalInfo.Replace("MONSTER_NAME", traversalInfoFile[0]);
				demonTraversalInfo = demonTraversalInfo.Replace("NUM", $"{random.Next()}");
				demonTraversalInfo = demonTraversalInfo.Replace("TRAVERSAL_ANIM", traversalInfoFile[2] + traversalAnim);
				demonTraversalInfo = demonTraversalInfo.Replace("MONSTER_TYPE", traversalInfoFile[1]);
				demonTraversalInfo = demonTraversalInfo.Replace("X1", $"{coords[0]}");
				demonTraversalInfo = demonTraversalInfo.Replace("Y1", $"{coords[1]}");
				demonTraversalInfo = demonTraversalInfo.Replace("Z1", $"{coords[2]}");
				demonTraversalInfo = demonTraversalInfo.Replace("X2", $"{coords[3]}");
				demonTraversalInfo = demonTraversalInfo.Replace("Y2", $"{coords[4]}");
				demonTraversalInfo = demonTraversalInfo.Replace("Z2", $"{coords[5]}");
				traversals.Add(demonTraversalInfo);

				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("MONSTER_NAME", traversalInfoFile[0]);
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("NUM", $"{random.Next()}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("TRAVERSAL_ANIM", traversalInfoFile[2] + reverseTraversalAnim);
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("MONSTER_TYPE", traversalInfoFile[1]);
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("X1", $"{reverseCoords[0]}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("Y1", $"{reverseCoords[1]}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("Z1", $"{reverseCoords[2]}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("X2", $"{reverseCoords[3]}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("Y2", $"{reverseCoords[4]}");
				reverseDemonTraversalInfo = reverseDemonTraversalInfo.Replace("Z2", $"{reverseCoords[5]}");
				traversals.Add(reverseDemonTraversalInfo);
			}

			return traversals;
		}

		private static string GetTraversalAnim(string anim, string[] traversalAnims)
		{
			if (traversalAnims.Contains(anim))
			{
				return anim;
			}
			else if (traversalAnims.Contains("jump_" + anim))
			{
				return "jump_" + anim;
			}
			else if (traversalAnims.Contains("jump_" + anim.Substring(anim.IndexOf("_") + 1)))
			{
				return "jump_" + anim.Substring(anim.IndexOf("_") + 1);
			}
			else if (traversalAnims.Contains("ledge_" + anim.Substring(anim.IndexOf("_") + 1)))
			{
				return "ledge_" + anim.Substring(anim.IndexOf("_") + 1);
			}

			return String.Empty;
		}

		private static string GetReverseTraversalAnim(string traversalAnim)
		{
			if (traversalAnim.Contains("up"))
			{
				return traversalAnim.Replace("up", "down");
			}
			else if (traversalAnim.Contains("down"))
			{
				return traversalAnim.Replace("down", "up");
			}
			else
			{
				return traversalAnim;
			}
		}

		private static double[] GetDoubleArray(string coordsString)
		{
			string[] coordsStringArray = coordsString.Split(' ');
			double[] coords = new double[3];

			for (int i = 0; i < coords.Length; i++)
			{
				if (!Double.TryParse(coordsStringArray[i], out coords[i]))
				{
					Console.Error.WriteLine("Failed to parse coord: " + coordsStringArray[i]);
					return null;
				}
			}

			return coords;
		}

		private static double[][] GetCoords()
		{
			Console.Write("Input the start coords: ");
			StringBuilder coordsStringBuilder = new StringBuilder();

			while (Regex.Replace(coordsStringBuilder.ToString().Trim(), @"\s+", " ").Split(' ').Length < 3)
			{
				coordsStringBuilder.Append(Console.ReadLine() + " ");
			}

			double[] startCoords = GetDoubleArray(Regex.Replace(coordsStringBuilder.ToString().Trim(), @"\s+", " "));
			coordsStringBuilder.Clear();

			if (startCoords == null)
			{
				return null;
			}

			Console.Write("Input the end coords: ");

			while (Regex.Replace(coordsStringBuilder.ToString().Trim(), @"\s+", " ").Split(' ').Length < 3)
			{
				coordsStringBuilder.Append(Console.ReadLine() + " ");
			}

			double[] endCoords = GetDoubleArray(Regex.Replace(coordsStringBuilder.ToString().Trim(), @"\s+", " "));
			coordsStringBuilder.Clear();

			if (endCoords == null)
			{
				return null;
			}

			startCoords[2] -= 1.6;
			endCoords[2] -= 1.6;

			double[] coords = startCoords.Concat(endCoords).ToArray();

			for (int i = 0; i < endCoords.Length; i++)
			{
				coords[startCoords.Length + i] -= coords[i];
			}

			double[] reverseCoords = endCoords.Concat(coords.Skip(3)).ToArray();

			for (int i = 0; i < endCoords.Length; i++)
			{
				reverseCoords[startCoords.Length + i] = -reverseCoords[startCoords.Length + i];
			}

			double[][] coordsArray = {coords, reverseCoords};
			return coordsArray;
		}
	}
}