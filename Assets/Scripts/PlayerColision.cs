using UnityEngine;

public class PlayerColision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
                Audio.instance.PlaySound(Audio.instance.die);
            playerMovement.Die();
        }
    }
}