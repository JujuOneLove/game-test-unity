using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Player"))
        {
            if (name == "Player1" && GameManager.checkPointP1 != null)
            {
                transform.position = (Vector3) GameManager.checkPointP1;
            }
            else if (name == "Player2" && GameManager.checkPointP2 != null)
            {
                transform.position = (Vector3) GameManager.checkPointP2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}