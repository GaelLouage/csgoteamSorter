using counterstrikeWarTeamMaker.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace counterstrikeWarTeamMaker.Dictionaries
{
    public static class Dictionary
    {
        public static new Dictionary<int, Action> GetSortingDictionary(ListView lVPlayers, List<Player> playerList)
        {
            return new Dictionary<int, Action>()
                {
                    { 0,() =>  lVPlayers.ItemsSource = playerList.OrderBy(x => x.Name).ToList() },
                    { 1, () =>lVPlayers.ItemsSource =  playerList.OrderByDescending(x => x.TypeOfPlayer).ToList()},
                    { 2, () =>  lVPlayers.ItemsSource = playerList.OrderBy(x => x.TypeOfPlayer).ToList()},
                };
        }
    }
}
