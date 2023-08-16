using counterstrikeWarTeamMaker.Entitites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace counterstrikeWarTeamMaker.Services.Interfaces
{
    public interface IJsonService
    {
        List<Player> Players { get; set; }

        Task<string> CreatePlayerAsync(Player player);
        Task<List<Player>> GetAllPlayersAsync();
        Task<bool> UpdateFileAsync();
        Task<string> RemovePlayerAsync(Player player);
        Task<string> UpdatePlayerAsync(Player player);
    }
}