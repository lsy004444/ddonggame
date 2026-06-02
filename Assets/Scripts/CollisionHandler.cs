
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Poop"))
        {
            if (GameManager.instance != null)
                GameManager.instance.AddPoop(1);

            PoopController poopCtrl = other.GetComponent<PoopController>();
            Debug.Log("poopCtrl: " + poopCtrl);
            Debug.Log("poopType: " + (poopCtrl != null ? poopCtrl.poopType?.ToString() : "null"));
            Debug.Log("ResouceManager: " + ResourceManager.Instance);
        
            if(ResourceManager.Instance != null && poopCtrl != null)
            {
                ResourceManager.Instance.AddPoop(poopCtrl.poopType, 1);
            }
            Destroy(other.gameObject);
        }
    }
}