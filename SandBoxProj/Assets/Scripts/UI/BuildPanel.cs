using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    [SerializeField]
    private Button floorButton;
    
    [SerializeField]
    private Button chairButton;
    
    [SerializeField]
    private Button bedButton;
    // Start is called before the first frame update
    void Start()
    {
        floorButton.onClick.AddListener(() =>
        {
            Debug.Log("floorButton.onclick");
            EventCenter.Instance.EventTrigger(E_EventType.E_BuildButtonClick, 0 );
        });
        
        chairButton.onClick.AddListener(() =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_BuildButtonClick, 1 );

        });
        
        bedButton.onClick.AddListener(() =>
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_BuildButtonClick, 2 );

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
