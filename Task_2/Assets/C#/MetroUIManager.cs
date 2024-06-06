using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MetroUIManager : MonoBehaviour
{
    public InputField startStationInput;
    public InputField endStationInput;
    public Button searchButton;
    public Text resultText;

    private MetroGraph metroGraph;
    private PathFinder pathFinder;

    void Start()
    {
        metroGraph = new MetroGraph();
        pathFinder = new PathFinder();
        LoadMetroData();

        searchButton.onClick.AddListener(Search);
    }

    void LoadMetroData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "metro_data.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            MetroData metroData = JsonUtility.FromJson<MetroData>(dataAsJson);

            foreach (var stationName in metroData.stations)
            {
                metroGraph.AddStation(stationName);
            }

            foreach (var connectionData in metroData.connections)
            {
                metroGraph.AddConnection(connectionData.station1, connectionData.station2, connectionData.line);
            }
        }
        else
        {
            Debug.LogError("Нет файла!");
        }
    }

    void Search()
    {
        string startStation = startStationInput.text;
        string endStation = endStationInput.text;

        var results = pathFinder.FindAllPaths(metroGraph, startStation, endStation);

        if (results != null && results.Count > 0)
        {
            resultText.text = "";
            int count = 1;
            foreach (var result in results)
            {
                resultText.text += $"Путь {count}:\n";
                foreach (var station in result.path)
                {
                    resultText.text += station.Name + " ";
                }
                resultText.text += $"\nСтанции: {result.path.Count}\nПреходы: {result.transfers}\n";
                resultText.text += "Станции каждой линии:\n";
                foreach (var line in result.stationsOnLines)
                {
                    resultText.text += $"{line.Key}: {line.Value} станций\n";
                }
                resultText.text += "\n";
                count++;
            }
        }
        else
        {
            resultText.text = "Такого пути нет";
        }
    }
}
