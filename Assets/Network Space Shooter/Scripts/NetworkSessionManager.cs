using Mirror;
using UnityEngine;

namespace NetworkSpaceShooter
{
    public class NetworkSessionManager : NetworkManager
    {
        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

        public bool IsServer => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly;
        public bool IsClient => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly;

        public override Transform GetStartPosition()
        {
            // first remove any dead transforms
            startPositions.RemoveAll(t => t == null);

            if (startPositions.Count == 0)
                return null;

            foreach (var startPos in startPositions)
            {
                Collider2D collider = Physics2D.OverlapCircle(startPos.position, 5.0f);

                if (collider == null)
                {
                    Debug.Log("Found place with no colliders. Go with it");
                    return startPos;
                }
                else
                {
                    Debug.Log("No place. Moving to next pos");
                }
            }

            // if for some reason colliders everywhere

            if (playerSpawnMethod == PlayerSpawnMethod.Random)
            {
                return startPositions[UnityEngine.Random.Range(0, startPositions.Count)];
            }
            else
            {
                Transform startPosition = startPositions[startPositionIndex];
                startPositionIndex = (startPositionIndex + 1) % startPositions.Count;
                return startPosition;
            }
        }
    }
}
