using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneCanvas : MonoBehaviour
{
    [SerializeField] private Button startGameBtn;
    // Start is called before the first frame update
    void Start()
    {
        startGameBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("GameScene");
        });
    }

}
