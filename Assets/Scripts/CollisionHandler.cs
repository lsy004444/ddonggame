using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Poop"))
        {
            GameManager.instance.AddPoop(1);
            Destroy(other.gameObject);
        }
    }
}
