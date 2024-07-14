using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = false;

    public void MovePlayer(List<Vector3> path)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveAlongPath(path));
        }
    }

    private IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;

        foreach (Vector3 position in path)
        {
            while (Vector3.Distance(transform.position, position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
