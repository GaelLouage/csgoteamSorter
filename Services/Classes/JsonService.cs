using counterstrikeWarTeamMaker.Entitites;
using counterstrikeWarTeamMaker.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace counterstrikeWarTeamMaker.Services.Classes
{
    public class JsonService : IJsonService
    {
        private string pathOfFile;
        private List<Player> players = new List<Player>();
        public List<Player> Players { get => players; set => players = value; }
        public JsonService(string filename)
        {
            pathOfFile = Path.Combine(Environment.CurrentDirectory, filename);
            if (!File.Exists(pathOfFile))
            {
                var fs = File.Create(pathOfFile);
                fs.Close();
            }
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            var readFile = await File.ReadAllTextAsync(pathOfFile);
            var players = JsonConvert.DeserializeObject<List<Player>>(readFile);
            return players.ToList() ?? new List<Player>();
        }

        public async Task<string> CreatePlayerAsync(Player player)
        {
            Players = await GetAllPlayersAsync();
            if (Players.Any(x => x.Name == player.Name))
            {
                return $"Player {player.Name} already in database!";
            }
            // inc id
            player.id = Players.Max(x => x.id) + 1;
            Players.Add(player);
            await UpdateFileAsync();
            return "Successfully created player!";
        }
        public async Task<string> RemovePlayerAsync(Player player)
        {
            Players = await GetAllPlayersAsync();
            var oldCount = Players.Count;
            var playerToRemove = Players.FirstOrDefault(x => x.id == player.id);
            Players.Remove(playerToRemove);
            await UpdateFileAsync();
            var updateCount = (await GetAllPlayersAsync()).Count();
            return oldCount > updateCount ? "Successfully deleted player!" : "Failed to remove player";
        }
        public async Task<bool> UpdateFileAsync()
        {
            var jsonConv = JsonConvert.SerializeObject(Players);
            await File.WriteAllTextAsync(pathOfFile, jsonConv);
            return true;
        }

        public async Task<string> UpdatePlayerAsync(Player player)
        {
            Players = await GetAllPlayersAsync();
            var playerToUpdate = Players.FirstOrDefault(x => x.id == player.id);
            if (playerToUpdate is null)
            {
                return $"No player with name {player.Name} found!";
            }
            //map this 
            playerToUpdate.Name = player.Name;
            playerToUpdate.TypeOfPlayer = player.TypeOfPlayer;
            await UpdateFileAsync();
            return "Update successful!";
        }
    }
}
