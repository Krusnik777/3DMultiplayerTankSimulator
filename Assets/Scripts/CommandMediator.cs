using UnityEngine;
using Mirror;

namespace MultiplayerTanks
{
    public class CommandMediator
    {
        public CommandMediator() { }

        [Command]
        public void CmdSetNetAim(Vehicle vehicle, Vector3 aim)
        {
            vehicle.CmdSetNetAimPoint(aim);
        }
    }
}
