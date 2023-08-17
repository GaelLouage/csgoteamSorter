using counterstrikeWarTeamMaker.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace counterstrikeWarTeamMaker.Helpers
{
    public static class ClipBoardHelper
    {
        public static void CopyToClipBoard(List<SelectedPlayer> selResetAfterDelete)
        {
			try
			{
                var clip = string.Join("\n", selResetAfterDelete.Select(x => new StringBuilder($"{x.Id}. {x.Name}")));
                Clipboard.SetText(clip);
            }
			catch {}
        }
    }
}
