using UnityEngine;

public interface AI
{
    void MoveTowardsPlayer(Vector3 playerPosition);
    bool IsMoving();
}
