﻿using counterstrikeWarTeamMaker.Entitites;
using counterstrikeWarTeamMaker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace counterstrikeWarTeamMaker.Helpers
{
    public static class ValidatoinHelper
    {
        public static (bool isValid, Func<string> message) PlayerSaveValidation(TextBox txtPlayerName, Player player)
        {
            //serparate class and method for error handling
            if (string.IsNullOrEmpty(txtPlayerName.Text))
            {
                return (false, () => "Name is required!");
            }
            var typesOfPlayerLevel = Enum.GetValues(typeof(TypeOfPlayer)).Cast<TypeOfPlayer>().ToList();
            if (!typesOfPlayerLevel.Any(t => t == player.TypeOfPlayer))
            {
                return (false, () => "Selection Typ of Player is required!");
            }
            player.Name = txtPlayerName.Text;
            return (true, () => "Player is Valid!");
        }
    }
}
