using UnityEngine;

public class MapBoundaries : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = FindObjectOfType<Player>();
        player.Die();
    }
}
