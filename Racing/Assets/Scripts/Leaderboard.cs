using System.Collections;
using System.Collections.Generic;
using System.Linq;

struct PlayerStats
{
    public string name;
    public int position;
    public float time;

    public PlayerStats(string n, int p,float t)
    {
        name = n;
        position = p;
        time = t;
    }
}

public class Leaderboard 
{
    static Dictionary<int, PlayerStats> lb = new Dictionary<int, PlayerStats>();
    static int carRegistered = -1;

    public static void Reset()
    {
        lb.Clear();
        carRegistered = -1;
    }

    public static int RegisterCar(string name)
    {
        carRegistered++;
        lb.Add(carRegistered, new PlayerStats(name, 0,0));
        return carRegistered;
    }

    public static void SetPosition(int regr, int lap,int checkpoint,float time)
    {
        int position = lap * 1000 + checkpoint;
        lb[regr] = new PlayerStats(lb[regr].name, position,time);
    }

    public static string GetPosition(int regr)
    {
        int index = 0;
        foreach(KeyValuePair<int,PlayerStats> pos in lb.OrderByDescending(key=>key.Value.position).ThenBy(key=>key.Value.time))
        {
            index++;
            if(pos.Key==regr)
            {
                switch(index)
                {
                    case 1:return "First";
                    case 2: return "Second";
                    case 3: return "Third";
                    case 4: return "Fourth";
                }
            }

        }

        return "unknown";
    }

    public static List<string> GetPlaces()
    {
        List<string> places = new List<string>();

        foreach (KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key => key.Value.position).ThenBy(key => key.Value.time))
        {
            places.Add(pos.Value.name);
        }
        return places;
    }
}
