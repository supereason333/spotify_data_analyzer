extends Node

const DEFAULT_SAVE_PATH := "res://results/"		# "user://results "
const DEFAULT_DATA_PATH := "res://data/"			# user://data "

var process_thread:Thread
var current_process_name:String
var current_process_status:float

func _ready() -> void:
	process_thread = Thread.new()
	process_thread.start(process_data.bind(DEFAULT_DATA_PATH))

func _process(delta: float) -> void:
	if current_process_name:
		print(current_process_name + ": " + str(current_process_status))

func selection_sort(data:Array) -> Array:
	var size := len(data)
	for i in size:
		current_process_name = "SORTING"
		current_process_status = float(i) / size
		
		var min_index = i
		for j in range(i + 1, size):
			if data[j]["count"] < data[min_index]["count"]:
				min_index = j
				
		if min_index != i:
			var a = data[i]
			data[i] = data[min_index]
			data[min_index] = a
	
	return data

func process_data(data_path:String = DEFAULT_DATA_PATH):
	DataProcess.ProcessData(data_path)
	
	#var data:Array = DataProcess.ProcessData(data_path)
	
	"""
	var data = open_all_files(data_path)		# This is the entire JSON data
	
	#var individual_song_data:Array = DataProcess.ProcessData(data)
	var individual_song_data:Array
	var total_unique_songs := len(individual_song_data)
	
	var size = len(data)
	for i in size:
		current_process_name = "COMPILING"
		current_process_status = float(i) / size
		
		var song_in_array := true
		var index := 0
		index = individual_song_data.find(data[i])
		if index == -1: song_in_array = false
		
		if song_in_array:
			individual_song_data[index]["count"] += 1
			individual_song_data[index]["time_played"] += data[i]["ms_played"]
		else:
			individual_song_data.append({"song": data[i]["master_metadata_track_name"], "artist": data[i]["master_metadata_album_artist_name"], "album": data[i]["master_metadata_album_album_name"], "count": 1, "time played": data[i]["ms_played"]})
		
	individual_song_data = selection_sort(individual_song_data)
	
	print(individual_song_data[0])
	print(individual_song_data[1])
	print(individual_song_data[-1])
	print(individual_song_data[2])
	"""

func open_all_files(dir:String) -> Array[Dictionary]:
	var merged_data: Array[Dictionary]
	var folder_files := DirAccess.get_files_at(dir)
	for file_name in folder_files:
		print_debug("LOAD FILE " + dir + file_name)
		if file_name.ends_with(".json") and file_name.begins_with("Streaming_History_Audio_"):
			var file = FileAccess.open(dir + file_name, FileAccess.READ)	# READ
			var data = JSON.parse_string(file.get_as_text())
			if data:
				merged_data.append_array(data)
			else:
				print_debug("LOAD FAILED " + dir + file_name)
	
	return merged_data


"""
Top album
Top artist
Top song
"""
