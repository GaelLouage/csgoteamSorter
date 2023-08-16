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
            await UiHelper.UpdateUiAsync(_jsonService,Dispatcher, cmbTypeOfPlayers, lVPlayers, lVPlayersRemover,
                lVPlayersUpdate, cmbTypeOfPlayersUpdate);
        }
        private async void btnSavePlayer_Click(object sender, RoutedEventArgs e)
        {
            var playerSaveValidation = ValidatoinHelper.PlayerSaveValidation(txtPlayerName, player);
            if(!playerSaveValidation.isValid)
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
            if(lVPlayersRemover.SelectedIndex >= 0)
            {
                var player = lVPlayersRemover.SelectedItem as Player;
                var remove = await _jsonService.RemovePlayerAsync(player);
                MessageBox.Show(remove);
                await UiHelper.ListViewUpdaterAsync(lVPlayers, lVPlayersRemover, lVPlayersUpdate);
            }
        }

        private async void btnUpdatePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (lVPlayersUpdate.SelectedIndex <= -1)
            {
                MessageBox.Show("No player selected!");
                return;
            }
            if(cmbTypeOfPlayersUpdate.SelectedIndex <= -1)
            {
                MessageBox.Show("No level selected");
                return;
            }
            if (string.IsNullOrEmpty(txtPlayerNameUpdate.Text))
            {
                MessageBox.Show("Name is required!");
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
            MessageBox.Show(updatePlayer);
        }

        private void lVPlayersUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lVPlayersUpdate.SelectedIndex >= 0)
            {
                var player = lVPlayersUpdate.SelectedItem as Player;
                txtPlayerNameUpdate.Text = player.Name;
                cmbTypeOfPlayersUpdate.SelectedItem = player.TypeOfPlayer;
            }
        }
    }
}
