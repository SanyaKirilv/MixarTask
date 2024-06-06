using System.Collections.Generic;

public class Station
{
    public string Name { get; }
    public List<Station> Neighbors { get; }
    public List<string> Lines { get; }

    public Station(string name)
    {
        Name = name;
        Neighbors = new List<Station>();
        Lines = new List<string>();
    }

    public void AddNeighbor(Station neighbor)
    {
        if (!Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }

    public void AddLine(string line)
    {
        if (!Lines.Contains(line))
        {
            Lines.Add(line);
        }
    }
}
