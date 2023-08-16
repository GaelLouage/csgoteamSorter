using counterstrikeWarTeamMaker.Dictionaries;
using counterstrikeWarTeamMaker.Entitites;
using counterstrikeWarTeamMaker.Enums;
using counterstrikeWarTeamMaker.Helpers;
using counterstrikeWarTeamMaker.Services.Classes;
using counterstrikeWarTeamMaker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace counterstrikeWarTeamMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IJsonService _jsonService;
        private Player player = new Player();
        private List<Player> playerList = new List<Player>();
        public MainWindow(IJsonService jsonService)
        {
            _jsonService = jsonService;
        }


        public MainWindow() : this(new JsonService("Players.json"))
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await UiHelper.UpdateUiAsync(_jsonService, Dispatcher, cmbTypeOfPlayers, lVPlayers, lVPlayersRemover,
                lVPlayersUpdate, cmbTypeOfPlayersUpdate, cmdMainTypeOfPlayers);
        }
        private async void btnSavePlayer_Click(object sender, RoutedEventArgs e)
        {
            var playerSaveValidation = ValidationHelper.PlayerSaveValidation(txtPlayerName, player);
            if (!playerSaveValidation.isValid)
            {
                MessageBox.Show(playerSaveValidation.message?.Invoke());
                return;
            }
            MessageBox.Show(await _jsonService.CreatePlayerAsync(player));
            await UiHelper.ListViewUpdaterAsync(lVPlayers, lVPlayersRemover, lVPlayersUpdate);
            txtPlayerName.Text = "";
        }

        private void cmbTypeOfPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            player.TypeOfPlayer = (TypeOfPlayer)cmbTypeOfPlayers.SelectedItem;
        }

        private async void btnDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (lVPlayersRemover.SelectedIndex >= 0)
            {
                var player = lVPlayersRemover.SelectedItem as Player;
                var remove = await _jsonService.RemovePlayerAsync(player);
                MessageBox.Show(remove);
                await UiHelper.ListViewUpdaterAsync(lVPlayers, lVPlayersRemover, lVPlayersUpdate);
                txtSortRemovePlayer.Text = string.Empty;
            }
        }

        private async void btnUpdatePlayer_Click(object sender, RoutedEventArgs e)
        {
            var validation = ValidationHelper.UpdateValidation(lVPlayersUpdate, cmbTypeOfPlayersUpdate, txtPlayerNameUpdate);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message);
                return;
            }
            var player = lVPlayersUpdate.SelectedItem as Player;
            player.TypeOfPlayer = (TypeOfPlayer)cmbTypeOfPlayersUpdate.SelectedItem;
            player.Name = txtPlayerNameUpdate.Text;
            var updatePlayer = await _jsonService.UpdatePlayerAsync(player);
            await UiHelper.ListViewUpdaterAsync(lVPlayers, lVPlayersRemover, lVPlayersUpdate);
            lVPlayersUpdate.SelectedItem = -1;
            cmbTypeOfPlayersUpdate.SelectedIndex = -1;
            txtPlayerNameUpdate.Text = string.Empty;
            txtSortUpdatePlayer.Text = string.Empty;
            MessageBox.Show(updatePlayer);
        }

        private void lVPlayersUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lVPlayersUpdate.SelectedIndex >= 0)
            {
                var player = lVPlayersUpdate.SelectedItem as Player;
                txtPlayerNameUpdate.Text = player.Name;
                cmbTypeOfPlayersUpdate.SelectedItem = player.TypeOfPlayer;
            }
        }

        // place in the ui updater
        private async void txtSortUpdatePlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            await UiHelper.UpdateListViewOnSearchAsync(lVPlayersUpdate, txtSortUpdatePlayer);
        }

        private async void txtSortRemovePlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            await UiHelper.UpdateListViewOnSearchAsync(lVPlayersRemover, txtSortRemovePlayer);
        }
        //main input
        private async void txtSearchPlayerMain_TextChanged(object sender, TextChangedEventArgs e)
        {
            //refactor this code 
            lVPlayers.ItemsSource = null;
            playerList = (await _jsonService.GetAllPlayersAsync()).ToList();
            if (!string.IsNullOrEmpty(txtSearchPlayerMain.Text))
            {
                playerList = (await _jsonService.GetAllPlayersAsync()).Where(x => x.Name.ToLower().Contains(txtSearchPlayerMain.Text.ToLower())).ToList();
            }
            var sorting = cmdMainTypeOfPlayers.SelectedIndex;
            if (sorting > -1)
            {
                var dictionarySorting = Dictionary.GetSortingDictionary(lVPlayers, playerList);
                if (dictionarySorting.ContainsKey(sorting))
                {
                    dictionarySorting[sorting]?.Invoke();
                }
            } else
            {
                lVPlayers.ItemsSource = playerList.Where(x => x.Name.ToLower().Contains(txtSearchPlayerMain.Text.ToLower()));
            }
          
           
        }

        private async void cmdMainTypeOfPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // refactor this code
            playerList = (await _jsonService.GetAllPlayersAsync()).ToList();
            if (!string.IsNullOrEmpty(txtSearchPlayerMain.Text))
            {
                playerList = (await _jsonService.GetAllPlayersAsync()).Where(x => x.Name.ToLower().Contains(txtSearchPlayerMain.Text.ToLower())).ToList();
            }
            lVPlayers.ItemsSource = null;
            var sorting = cmdMainTypeOfPlayers.SelectedIndex;
            if (sorting > -1)
            {
                var dictionarySorting = Dictionary.GetSortingDictionary(lVPlayers, playerList);
                if (dictionarySorting.ContainsKey(sorting))
                {
                    dictionarySorting[sorting]?.Invoke();
                }
            }
        }
    }
}
