using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject spawn1;
    [SerializeField] private GameObject spawn2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.name == "Player1")
            {
                if (spawn1 != null)
                {
                    GameManager.checkPointP1 = spawn1.transform.position;
                }
                else  GameManager.checkPointP1 = transform.position;
            }
            else if (other.name == "Player2")
            {
                if (spawn2 != null)
                {
                    GameManager.checkPointP2 = spawn2.transform.position;
                }
                else  GameManager.checkPointP2 = transform.position;
            }
        }
    }
}