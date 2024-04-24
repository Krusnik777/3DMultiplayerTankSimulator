using UnityEngine;

namespace MultiplayerTanks
{
    public class ConditionDeathMatch : MonoBehaviour, IMatchCondition
    {
        private int red;
        private int blue;

        private int winTeamId = -1;
        public int WinTeamId => winTeamId;

        private bool triggered;

        public bool IsTriggered => triggered;

        public void OnServerMatchStart(MatchController controller)
        {
            Reset();

            foreach (var member in FindObjectsOfType<MatchMember>())
            {
                if (member.ActiveVehicle != null)
                {
                    member.ActiveVehicle.Destroyed += OnVehicleDestroyed;

                    if (member.TeamId == TeamSide.TeamRed)
                        red++;
                    else
                    if (member.TeamId == TeamSide.TeamBlue)
                        blue++;
                }
            }
        }

        public void OnServerMatchEnd(MatchController controller)
        {

        }

        private void Reset()
        {
            red = 0;
            blue = 0;
            triggered = false;
        }

        private void OnVehicleDestroyed(Destructible destructible)
        {
            Vehicle vehicle = destructible as Vehicle;

            if (vehicle == null) return;

            var ownerMember = vehicle.Owner?.GetComponent<MatchMember>();

            if (ownerMember == null) return;

            switch (ownerMember.TeamId)
            {
                case TeamSide.TeamRed : red--; break;
                case TeamSide.TeamBlue : blue--; break;
            }

            if (red == 0)
            {
                winTeamId = TeamSide.TeamBlue;
                triggered = true;
            }
            else
            if (blue == 0)
            {
                winTeamId = TeamSide.TeamRed;
                triggered = true;
            }
        }
    }
}
