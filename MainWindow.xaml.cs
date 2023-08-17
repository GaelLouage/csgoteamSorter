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
using System.Xml.Linq;

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
        private List<SelectedPlayer> selectedPlayers = new List<SelectedPlayer>();
        private bool playerDeleted;
        private List<SelectedPlayer> teamCT;
        private List<SelectedPlayer> teamT;
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
            btnCreateTeams.Visibility = Visibility.Hidden;
            stPTeams.Visibility = Visibility.Hidden;
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
            }
            else
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

        private void lVPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (selectedPlayers.Count >= 12)
                {
                    MessageBox.Show("Maximum of players in list");
                    return;
                }

                var selectedPlayer = lVPlayers.SelectedItem as Player;
                if (selectedPlayers.Any(x => x.Name == selectedPlayer.Name))
                {
                    MessageBox.Show("Player already in list.");
                    return;
                }
                var idPlus = selectedPlayers.Count() + 1;
                selectedPlayers.Add(new SelectedPlayer() { Id = idPlus, Name = selectedPlayer.Name });
                lBselectedPlayers.ItemsSource = null;
                lBselectedPlayers.ItemsSource = selectedPlayers;
                ClipBoardHelper.CopyToClipBoard(selectedPlayers);
                if (playerDeleted)
                {
                    lBselectedPlayers.ItemsSource = null;
                    lBselectedPlayers.ItemsSource = SelectedPlayerResetter();
                    ClipBoardHelper.CopyToClipBoard(SelectedPlayerResetter());
                }
                UiHelper.SelectedPlayerButtonAndTeamsVisibility(selectedPlayers,btnCreateTeams,stPTeams);
            }
            catch { }
        }

     

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            playerDeleted = true;
            Button clickedButton = (Button)sender;
            var name = (string)clickedButton.Tag;
            var toRemove = selectedPlayers.FirstOrDefault(x => x.Name == name);
            if (toRemove is null)
            {
                MessageBox.Show("Not found!");
                return;
            }
            selectedPlayers.Remove(toRemove);
            UiHelper.SelectedPlayerButtonAndTeamsVisibility(selectedPlayers, btnCreateTeams, stPTeams);
            lBselectedPlayers.ItemsSource = null;
            lBselectedPlayers.ItemsSource = SelectedPlayerResetter();
        }

        private List<SelectedPlayer> SelectedPlayerResetter()
        {
            var selResetAfterDelete = new List<SelectedPlayer>();
            int idSetter = 1;
            foreach (var player in selectedPlayers)
            {
                selResetAfterDelete.Add(new SelectedPlayer() { Id = idSetter, Name = player.Name });
                idSetter++;
            }
            ClipBoardHelper.CopyToClipBoard(selResetAfterDelete);
            return selResetAfterDelete;
        }

        private void btnCreateTeams_Click(object sender, RoutedEventArgs e)
        {
            stPTeams.Visibility = Visibility.Visible;
            teamCT = new List<SelectedPlayer>();
            teamT = new List<SelectedPlayer>();
            if(selectedPlayers.Count == 12)
            {
                CreateTeams(6);
            } else if(selectedPlayers.Count == 10) 
            {
                CreateTeams(5);
            }
            txtTeamCT.Text = string.Join("\n\n", teamCT.Select((x,i) => new StringBuilder($"{i + 1}. {x.Name}")));
            txtTeamT.Text = string.Join("\n\n", teamT.Select((x,i) => new StringBuilder($"{i+1}. {x.Name}")));
        }

        public void CreateTeams(int index)
        {
            while (teamT.Count < index || teamCT.Count < index)
            {
                var random = new Random();
                int next = random.Next(0, selectedPlayers.Count);
                if (teamCT.Count < index)
                {
                    AddPlayer(teamCT, teamT,next);
                }
                next = random.Next(0, selectedPlayers.Count);
                if (teamT.Count < index)
                {
                    AddPlayer(teamT, teamCT, next);
                }
            }
        }

        private void AddPlayer(List<SelectedPlayer> team, List<SelectedPlayer> teamTwo, int next)
        {
            if (!team.Any(x => x.Name == selectedPlayers[next].Name) && !teamTwo.Any(x => x.Name == selectedPlayers[next].Name))
            {
                team.Add(selectedPlayers[next]);
            }
        }
    }
}
