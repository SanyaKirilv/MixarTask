using System;
using System.Collections.Generic;
using System.Linq;

public class PathFinder
{
    public List<(List<Station> path, int transfers, Dictionary<string, int> stationsOnLines)> FindAllPaths(MetroGraph metroGraph, string start, string end)
    {
        var startStation = metroGraph.GetStation(start);
        var endStation = metroGraph.GetStation(end);
        if (startStation == null || endStation == null) return null;

        var paths = new List<(List<Station> path, int transfers, Dictionary<string, int> stationsOnLines)>();
        var visited = new HashSet<Station>();
        Find(startStation, endStation, new List<Station>(), 0, null, visited, paths, new Dictionary<string, int>());
        return paths.OrderBy(p => p.transfers).ThenBy(p => p.path.Count).ToList();
    }

    private void Find(Station current, Station end, List<Station> path, int transfers, string lastLine, HashSet<Station> visited, List<(List<Station> path, int transfers, Dictionary<string, int> stationsOnLines)> paths, Dictionary<string, int> stationsOnLines)
    {
        path.Add(current);
        visited.Add(current);

        if (current == end)
        {
            paths.Add((new List<Station>(path), transfers, new Dictionary<string, int>(stationsOnLines)));
        }
        else
        {
            foreach (var neighbor in current.Neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    var commonLines = current.Lines.Intersect(neighbor.Lines).ToList();
                    string nextLine = commonLines.FirstOrDefault();
                    int newTransfers = transfers + (lastLine != null && lastLine != nextLine ? 1 : 0);

                    if (nextLine != null)
                    {
                        if (!stationsOnLines.ContainsKey(nextLine))
                        {
                            stationsOnLines[nextLine] = 0;
                        }
                        stationsOnLines[nextLine]++;
                    }

                    Find(neighbor, end, path, newTransfers, nextLine, visited, paths, stationsOnLines);

                    if (nextLine != null)
                    {
                        stationsOnLines[nextLine]--;
                        if (stationsOnLines[nextLine] == 0)
                        {
                            stationsOnLines.Remove(nextLine);
                        }
                    }
                }
            }
        }

        path.RemoveAt(path.Count - 1);
        visited.Remove(current);
    }
}
