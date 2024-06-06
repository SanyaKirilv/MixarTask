using System;
using System.Collections.Generic;
using System.Linq;

public class MetroGraph
{
    public Dictionary<string, Station> Stations { get; }

    public MetroGraph()
    {
        Stations = new Dictionary<string, Station>();
    }

    public void AddStation(string name)
    {
        if (!Stations.ContainsKey(name))
        {
            Stations[name] = new Station(name);
        }
    }

    public void AddConnection(string station1, string station2, string line)
    {
        if (Stations.ContainsKey(station1) && Stations.ContainsKey(station2))
        {
            Stations[station1].AddNeighbor(Stations[station2]);
            Stations[station2].AddNeighbor(Stations[station1]);
            Stations[station1].AddLine(line);
            Stations[station2].AddLine(line);
        }
    }

    public Station GetStation(string name)
    {
        if (Stations.ContainsKey(name))
        {
            return Stations[name];
        }
        return null;
    }
}
