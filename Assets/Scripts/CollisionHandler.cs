using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{
    private void OnTiggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Poop"))
        {
            GameManager.instance.AddPoop(1);
            Destroy(other.gameObject);
        }
    }
}
