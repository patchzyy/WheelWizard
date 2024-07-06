using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Utils;

public class RRLiveInfo
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<RRInformation> getCurrentGameData()
    {
        string apiUrl = "http://zplwii.xyz/api/groups";
        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            List<RoomInfo> rooms = JsonSerializer.Deserialize<List<RoomInfo>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return new RRInformation { Rooms = rooms };
        }
        catch (HttpRequestException e)
        {
            return new RRInformation { Rooms = new List<RoomInfo>() };
        }
        catch (JsonException e)
        {
            MessageBox.Show(($"I messed up, here is why: {e.Message}"));
            
            return new RRInformation { Rooms = new List<RoomInfo>() };
        }
    }

    public static int GetCurrentOnlinePlayers(RRInformation info)
    {
        int PlayerCount = 0;
        foreach (RoomInfo room in info.Rooms) PlayerCount =+ room.Players.Count;
        return PlayerCount;
    }

    public class RRInformation
    {
        public List<RoomInfo> Rooms;
    }
    public class RoomInfo
    {
        public string Id { get; set; }
        public string Game { get; set; }
        public DateTime Created { get; set; }
        public string Type { get; set; }
        public bool Suspend { get; set; }
        public string Host { get; set; }
        public string Rk { get; set; }
        public Dictionary<string, Player> Players { get; set; }
        public class Player
        {
            public string Count { get; set; }
            public string Pid { get; set; }
            public string Name { get; set; }
            public string ConnMap { get; set; }
            public string ConnFail { get; set; }
            public string Suspend { get; set; }
            public string Fc { get; set; }
            public string Ev { get; set; }
            public string Eb { get; set; }
            public List<Mii> Mii { get; set; }
        }

        public class Mii
        {
            public string Data { get; set; }
            public string Name { get; set; }
        }
    }
}
