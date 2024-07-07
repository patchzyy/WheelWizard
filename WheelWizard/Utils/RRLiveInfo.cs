using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Utils;

public class RRLiveInfo
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<RRInformation> getCurrentGameData()
    {
      string data = @"[
  {
    ""id"": ""PIFYVV"",
    ""game"": ""mariokartwii"",
    ""created"": ""2024-07-05T13:01:48.8546078-07:00"",
    ""type"": ""anybody"",
    ""suspend"": false,
    ""host"": ""10"",
    ""rk"": ""vs_751"",
    ""players"": {
      ""10"": {
        ""count"": ""1"",
        ""pid"": ""602132615"",
        ""name"": ""Nicky"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""1"",
        ""suspend"": ""0"",
        ""fc"": ""4086-2402-5735"",
          ""ev"": ""8541"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""gAwATgBpAGMAawB5AAAAAAAAAAAAAGULgAAAAAAAAAAgE02AOdQqo5xsiFgUTZiNAIoAiiaSAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""Nicky""
          }
        ]
      },
      ""17"": {
        ""count"": ""1"",
        ""pid"": ""600357570"",
        ""name"": ""Rave"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""0349-6009-5938"",
        ""ev"": ""9249"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""wAAAUgBhAHYAZQAAAAAAAAAAAAAAAAAAgAAAAAAAAAAAVzrAAZRgNSBProACdZgQZMEAiiUEAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""Rave""
          }
        ]
      },
      ""23"": {
        ""count"": ""1"",
        ""pid"": ""600279379"",
        ""name"": ""Pure Play"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""0521-3988-6931"",
        ""ev"": ""8517"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""gAYAUAB1AHIAZQAgAFAAbABhAHkAAEBAgAAAAAAAAAAoVogAmZMIozyMBnikTXCNgIkAiiUFAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""Pure Play""
          }
        ]
      },
      ""26"": {
        ""count"": ""1"",
        ""pid"": ""602330808"",
        ""name"": ""mario"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""5417-6821-0104"",
        ""ev"": ""7305"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""gAAAbQBhAHIAaQBvAAAAAAAAAAAAAEhFgAAAAAAAAAAkFnxgihMooYRsjDhYTakMAIpBCSUFAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""mario""
          }
        ]
      },
      ""27"": {
        ""count"": ""1"",
        ""pid"": ""601029743"",
        ""name"": ""coop"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""2368-2423-1023"",
        ""ev"": ""8017"",
        ""eb"": ""4791"",
        ""mii"": [
          {
            ""data"": ""ABAAYwBvAG8AcAAAAAAAAAAAAAAAAH9/gAAAAAAAAAAgAUQBEVAQgJBqTkoXRUDNEOoBCsGQAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""coop""
          }
        ]
      },
      ""29"": {
        ""count"": ""1"",
        ""pid"": ""602056598"",
        ""name"": ""f¢ εmpex"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""0091-9199-1190"",
        ""ev"": ""10006"",
        ""eb"": ""9999"",
        ""mii"": [
          {
            ""data"": ""gBQAZgCiACADtQBtAHAAZQB4AAAAAEBAgAAAAAAAAAAG53ngkaQLEqSSDkAAfbgSYJOAR42EAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""f¢ εmpex""
          }
        ]
      },
      ""31"": {
        ""count"": ""1"",
        ""pid"": ""40"",
        ""name"": ""Serena"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""3607-7725-2904"",
        ""ev"": ""6649"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""wBAAUwBlAHIAZQBuAGEAAAAAAAAAAEBAgAAAAAAAAAAgJyJASWQoooSNKFgUTUCNEIoAiiUEAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""Serena""
          }
        ]
      },
      ""32"": {
        ""count"": ""1"",
        ""pid"": ""601996810"",
        ""name"": ""f¢ 3 dots"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""4816-3833-3962"",
        ""ev"": ""9131"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""wBQAZgCiACAAMwAgAGQAbwB0AHMAAEBAgAAAAAAAAAA0FhAAudOINqhxDFgAhbgRKrEIiiUFAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""f¢ 3 dots""
          }
        ]
      },
      ""33"": {
        ""count"": ""1"",
        ""pid"": ""600172949"",
        ""name"": ""gio"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""4644-5664-0917"",
        ""ev"": ""9658"",
        ""eb"": ""9651"",
        ""mii"": [
          {
            ""data"": ""ABQAZwBpAG8AAAAAAAAAAAAAAAAAAGAsgAAAAAAAAACGRVRBqVAIonSNRkpkRZhuAGqgqY3GAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""gio""
          }
        ]
      },
      ""34"": {
        ""count"": ""1"",
        ""pid"": ""90"",
        ""name"": ""ViniX2"",
        ""conn_map"": ""222222222"",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""0343-5973-8458"",
        ""ev"": ""5007"",
        ""eb"": ""5000"",
        ""mii"": [
          {
            ""data"": ""gBYAVgBpAG4AaQBYADIAAAAAAAAAAABAgAAAAAAAAAAkLnhQuVkookhgAUAAYbgRYO8AiiUEAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""ViniX2""
          }
        ]
      }
    }
  },
  {
    ""id"": ""UEJEAV"",
    ""game"": ""mariokartwii"",
    ""created"": ""2024-07-05T15:15:57.9174716-07:00"",
    ""type"": ""anybody"",
    ""suspend"": false,
    ""host"": ""0"",
    ""rk"": ""vs_751"",
    ""players"": {
      ""0"": {
        ""count"": ""1"",
        ""pid"": ""601462443"",
        ""name"": ""BossBoy28"",
        ""conn_map"": """",
        ""conn_fail"": ""0"",
        ""suspend"": ""0"",
        ""fc"": ""4945-2270-1483"",
        ""ev"": ""8975"",
        ""eb"": ""5500"",
        ""mii"": [
          {
            ""data"": ""gAgAQgBvAHMAcwBCAG8AeQAyADgAAGBAgAAAAAAAAAAAE2dAMZSmogiMSFgUTXiNAIsKiiUEAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
            ""name"": ""BossBoy28""
          }
        ]
      }
    }
  }
]";
      List<RoomInfo> rooms = JsonSerializer.Deserialize<List<RoomInfo>>(data, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
      return new RRInformation { Rooms = rooms };
      // string apiUrl = "http://zplwii.xyz/api/groups";
      // try
      // {
      //     HttpResponseMessage response = await client.GetAsync(apiUrl);
      //     response.EnsureSuccessStatusCode();
      //     string responseBody = await response.Content.ReadAsStringAsync();
      //
      //     List<RoomInfo> rooms = JsonSerializer.Deserialize<List<RoomInfo>>(responseBody, new JsonSerializerOptions
      //     {
      //         PropertyNameCaseInsensitive = true
      //     });
      //     return new RRInformation { Rooms = rooms };
      // }
      // catch (HttpRequestException e)
      // {
      //     return new RRInformation { Rooms = new List<RoomInfo>() };
      // }
      // catch (JsonException e)
      // {
      //     MessageBox.Show(($"I messed up, here is why: {e.Message}"));
      //     
      //     return new RRInformation { Rooms = new List<RoomInfo>() };
      // }
    }

    public static int GetCurrentOnlinePlayers(RRInformation info)
    {
        int PlayerCount = 0;
        foreach (RoomInfo room in info.Rooms) PlayerCount += room.Players.Count;
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

      public class Player : INotifyPropertyChanged
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

        private BitmapImage _miiImage;
        public BitmapImage MiiImage
        {
          get => _miiImage;
          set
          {
            if (_miiImage != value)
            {
              _miiImage = value;
              OnPropertyChanged(nameof(MiiImage));
            }
          }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      }

      public class Mii
      {
        public string Data { get; set; }
        public string Name { get; set; }
      }
    }
}