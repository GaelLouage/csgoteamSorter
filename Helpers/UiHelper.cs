﻿using counterstrikeWarTeamMaker.Entitites;
using counterstrikeWarTeamMaker.Enums;
using counterstrikeWarTeamMaker.Services.Classes;
using counterstrikeWarTeamMaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace counterstrikeWarTeamMaker.Helpers
{
    public static class UiHelper
    {

        private static IJsonService _jsonService;
        public static async Task UpdateUiAsync(IJsonService jsonService, Dispatcher dispatcher, ComboBox cmbTypeOfPlayers, ListView lVPlayers, ListView lVPlayerRemover,
            ListView lVPlayerUpdate, ComboBox cmbUpdatePlayers)
        {
            _jsonService = jsonService;
            UpdateComboboxPlayers(cmbTypeOfPlayers);
            UpdateComboboxPlayers(cmbUpdatePlayers);
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
    }
}
