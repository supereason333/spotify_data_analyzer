using Godot;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

public partial class DataProcess : Node
{

	string DEFAULT_SAVE_PATH = "res://results/";		// "user://results "
	string DEFAULT_DATA_PATH = "res://data/";			// user://data "

	public void ProcessData(string dir)
	{
		Dictionary<string, object>[] data = openAllFiles(dir);
		List<Dictionary<string, object>> individualSongData = new List<Dictionary<string, object>>();
		// Dictionary<string, int>[] individualSongData = new Dictionary<string, int>[0];

		GD.Print(data[0]["master_metadata_track_name"]);
		GD.Print(data[1]["master_metadata_track_name"]);
		GD.Print(data[data.Length - 1]["master_metadata_track_name"]);
		GD.Print(data[data.Length - 2]["master_metadata_track_name"]);

		Node Analyzer = (Node)GetNode("../root/GameData");//GetTree().Root.GetNode("Analyzer");	//(Node)GetNode("/root/GameData");
		Analyzer.Set("current_process_name", "PROCESSING DATA");
		int a = 0;
		int size = data.Length;
		foreach (Dictionary<string, object> item in data)
		{
			Analyzer.Set("current_process_status", (float)a / size);
			
			bool songExists = false;
			int index = 0;

			for (int i = 0; i < individualSongData.Count; i++)
			{
				if (item["song"] == individualSongData[i]["master_metadata_track_name"] && item["artist"] == individualSongData[i]["master_metadata_album_artist_name"])
				{
					songExists = true;
					index = i;
					break;
				}
			}

			if (songExists)
			{
				individualSongData[index]["count"] = (int)individualSongData[index]["count"] + 1;
				individualSongData[index]["total_playtime"] = (int)individualSongData[index]["total_playtime"] + (int)item["ms_played"];
			}
			else
			{
				individualSongData.Add(new Dictionary<string, object>
				{
					{ "song", item["master_metadata_track_name"] },
					{ "artist", item["master_metadata_album_artist_name"] },
					{ "count", 1 },
					{ "total_playtime", item["ms_played"] }
				});
			}
			a++;
		}
		individualSongData = SelectionSort(individualSongData.ToArray(), "count").ToList();
	}
	public Dictionary<string, object>[] SelectionSort(Dictionary<string, object>[] data, string key = "count")
	{
		for (int i = 0; i < data.Length - 1; i++)
		{
			int minIndex = i;
			for (int j = i + 1; j < data.Length; j++)
			{
				if ((int)data[j][key] < (int)data[minIndex][key])
				{
					minIndex = j;
				}
			}
			Dictionary<string, object> temp = data[minIndex];
			data[minIndex] = data[i];
			data[i] = temp;
		}
		return data;
	}
	public Dictionary<string, object>[] openAllFiles(string dir)
	{
		List<string> files = DirAccess.GetFilesAt(dir).ToList();
		for (var i = 0; i < files.Count; i++)
		{
			if (!files[i].EndsWith(".json") || !files[i].StartsWith("Streaming_History_Audio_"))
			{
				files.RemoveAt(i);
			}
		}
		List<Dictionary<string, object>> merged_data = new List<Dictionary<string, object>>();
		for (var i = 0; i < files.Count; i++)
		{
			GD.Print("LOAD FILE " + dir + files[i]);

			string jsonString = File.ReadAllText(ProjectSettings.GlobalizePath(dir + files[i]));
			Dictionary<string, object>[] data = JsonSerializer.Deserialize<Dictionary<string, object>[]>(jsonString);
			merged_data.AddRange(data.ToList());
		}

		return merged_data.ToArray();
	}
}
