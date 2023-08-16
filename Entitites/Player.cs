using counterstrikeWarTeamMaker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace counterstrikeWarTeamMaker.Entitites
{
    public class Player
    {
        public int id { get; set; }
        public string Name { get; set; }
        public TypeOfPlayer TypeOfPlayer { get; set; } 
    }
}
