
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Poop"))
        {
            GameManager.instance.AddPoop(1);

            PoopController poopCtrl = other.GetComponent<PoopController>();
            if(ResourceManager.Instance != null && poopCtrl != null)
            {
                ResourceManager.Instance.AddPoop(poopCtrl.poopType, 1);
            }
            Destroy(other.gameObject);
        }
    }
}