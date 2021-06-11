using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
				Console.Write("Input the start coords: ");
				StringBuilder stringBuilder = new StringBuilder();

				while (stringBuilder.ToString().Trim().Split(' ').Length < 3)
				{
					stringBuilder.Append(Console.ReadLine() + " ");
				}

				double[] startCoords = GetCoords(stringBuilder.ToString());

				Console.Write("Input the end coords: ");
				StringBuilder stringBuilder2 = new StringBuilder();

				while (stringBuilder2.ToString().Trim().Split(' ').Length < 3)
				{
					stringBuilder2.Append(Console.ReadLine() + " ");
				}

				double[] endCoords = GetCoords(stringBuilder2.ToString());

				startCoords[2] -= 1.6;
				endCoords[2] -= 1.6;

				for (int i = 0; i < endCoords.Length; i++)
				{
					endCoords[i] -= startCoords[i];
				}

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

				List<string> traversalInfo = GetTraversalInfo(File.ReadAllText($"traversal{Path.DirectorySeparatorChar}entitydef.txt"), traversalAnim, onlySmallDemons.GetValueOrDefault(), startCoords, endCoords);
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

		private static List<string> GetTraversalInfo(string traversalInfo, string traversalAnim, bool onlySmallDemons, double[] startCoords, double[] endCoords)
		{
			List<string> list = new List<string>();
			Random random = new Random();

			foreach (string traversalFilePath in Directory.GetFiles("traversal", "*.tin"))
			{
				if (!onlySmallDemons || SmallDemons.Contains<string>(Path.GetFileNameWithoutExtension(traversalFilePath)))
				{
					string demonTraversalInfo = traversalInfo;
					string[] traversalInfoFile = File.ReadAllLines(traversalFilePath);
					string anim = GetTraversalAnim(traversalAnim, traversalInfoFile.Skip(3).ToArray());

					if (!String.IsNullOrEmpty(anim))
					{
						demonTraversalInfo = demonTraversalInfo.Replace("MONSTER_NAME", traversalInfoFile[0]);
						demonTraversalInfo = demonTraversalInfo.Replace("NUM", $"{random.Next()}");
						demonTraversalInfo = demonTraversalInfo.Replace("TRAVERSAL_ANIM", traversalInfoFile[2] + anim);
						demonTraversalInfo = demonTraversalInfo.Replace("MONSTER_TYPE", traversalInfoFile[1]);
						demonTraversalInfo = demonTraversalInfo.Replace("X1", $"{startCoords[0]}");
						demonTraversalInfo = demonTraversalInfo.Replace("Y1", $"{startCoords[1]}");
						demonTraversalInfo = demonTraversalInfo.Replace("Z1", $"{startCoords[2]}");
						demonTraversalInfo = demonTraversalInfo.Replace("X2", $"{endCoords[0]}");
						demonTraversalInfo = demonTraversalInfo.Replace("Y2", $"{endCoords[1]}");
						demonTraversalInfo = demonTraversalInfo.Replace("Z2", $"{endCoords[2]}");
						list.Add(demonTraversalInfo);
					}
				}
			}
			return list;
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

		private static double[] GetCoords(string coordsString)
		{
			string[] coordsStringArray = coordsString.Trim().Split(' ');
			double[] coords = new double[3];

			for (int i = 0; i < coords.Length; i++)
			{
				if (!Double.TryParse(coordsStringArray[i], out coords[i]))
				{
					Console.Error.WriteLine("Failed to parse coord: " + coordsStringArray[i]);
					Environment.Exit(1);
				}
			}

			return coords;
		}
	}
}