using UnityEngine;
using UnityEngine.UI;

public class LeftAreaPanel : MonoBehaviour
{
    public Button saveBtn;
    public Button loadBtn;
    public Button clearBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        saveBtn.onClick.AddListener(() =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_LeftAreaButtonClick, 0 );
        });
        
        loadBtn.onClick.AddListener(() =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_LeftAreaButtonClick, 1 );
        });
        
        clearBtn.onClick.AddListener(() =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_LeftAreaButtonClick, 2 );
        });
    }
}
