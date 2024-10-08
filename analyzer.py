import json
from pathlib import Path

def selection_sort(data):
    for i in range(len(data)):
        min_index = i

        for j in range(i + 1, len(data)):
            if data[j]["count"] < data[min_index]["count"]:
                min_index = j

        data[i], data[min_index] = data[min_index], data[i]

    return data

def analyse_data(data, results_directory):
    individual_song_data = []
    total_played_songs = 0

    for item in data:
        song_in_array = False
        index = 0
        for i in range(len(individual_song_data)):
            if individual_song_data[i]["song"] == item["master_metadata_track_name"] and individual_song_data[i]["artist"] == item["master_metadata_album_artist_name"]:
                song_in_array = True
                index = i
                break

        if song_in_array:
            individual_song_data[index]["count"] += 1
            individual_song_data[index]["time played"] += item["ms_played"]

        else:
            individual_song_data.append({"song": item["master_metadata_track_name"], "artist": item["master_metadata_album_artist_name"], "album": item["master_metadata_album_album_name"], "count": 1, "time played": item["ms_played"]})

    with open(results_directory / "individual_song_data.json", "w", encoding="utf-8") as file:
        individual_song_data = selection_sort(individual_song_data)

        json.dump(individual_song_data, file, indent=4)
        file.close()

def open_all_files(directory):
    merged_data = []
    for path in directory.iterdir():
        with open(path, "r", encoding="utf-8") as file:
            data = json.load(file)

            merged_data.extend(data)
            file.close()
    
    return merged_data



Data_Directory = Path("data")
Results_directory = Path("results")

merged_data = open_all_files(Data_Directory)

analyse_data(merged_data, Results_directory)
