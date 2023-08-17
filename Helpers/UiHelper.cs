using counterstrikeWarTeamMaker.Entitites;
using counterstrikeWarTeamMaker.Enums;
using counterstrikeWarTeamMaker.Services.Classes;
using counterstrikeWarTeamMaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace counterstrikeWarTeamMaker.Helpers
{
    public static class UiHelper
    {

        private static IJsonService _jsonService;
        public static async Task UpdateUiAsync(IJsonService jsonService, Dispatcher dispatcher, ComboBox cmbTypeOfPlayers, ListView lVPlayers, ListView lVPlayerRemover,
            ListView lVPlayerUpdate, ComboBox cmbUpdatePlayers , ComboBox cmdMainTypeOfPlayers)
        {
            _jsonService = jsonService;
            UpdateComboboxPlayers(cmbTypeOfPlayers);
            UpdateComboboxPlayers(cmbUpdatePlayers);
            //UpdateComboboxPlayers(cmdMainTypeOfPlayers);
            await ListViewUpdaterAsync(lVPlayers, lVPlayerRemover, lVPlayerUpdate);
        }

        private static void UpdateComboboxPlayers(ComboBox cmbTypeOfPlayers)
        {
            cmbTypeOfPlayers.ItemsSource = null;
            cmbTypeOfPlayers.ItemsSource = Enum.GetValues(typeof(TypeOfPlayer)).Cast<TypeOfPlayer>();
        }
        public async static Task UpdatePlayerListViewAsync(ListView lVPlayers)
        {
            lVPlayers.ItemsSource = null;
            lVPlayers.ItemsSource = await _jsonService.GetAllPlayersAsync();
        }
        public async  static Task ListViewUpdaterAsync(ListView lVPlayers, ListView lVPlayersRemover, ListView lVPlayersUpdate)
        {
            await UpdatePlayerListViewAsync(lVPlayers);
            await UpdatePlayerListViewAsync(lVPlayersRemover);
            await UpdatePlayerListViewAsync(lVPlayersUpdate);
        }
        public async static Task UpdateListViewOnSearchAsync(ListView lv, TextBox txtSortUpdatePlayer)
        {
            lv.ItemsSource = null;
            lv.ItemsSource = (await _jsonService.GetAllPlayersAsync()).Where(x => x.Name.ToLower().Contains(txtSortUpdatePlayer.Text.ToLower()));
        }

        public static void SelectedPlayerButtonAndTeamsVisibility(List<SelectedPlayer> selectedPlayers, Button btnCreateTeams,
           StackPanel stPTeams )
        {
            if (selectedPlayers.Count == 10 || selectedPlayers.Count == 12)
            {
                btnCreateTeams.Visibility = Visibility.Visible;
            }
            if (selectedPlayers.Count < 10 || (selectedPlayers.Count > 10 && selectedPlayers.Count < 12))
            {
                btnCreateTeams.Visibility = Visibility.Hidden;
                stPTeams.Visibility = Visibility.Hidden;
            }
        }
    }
}
