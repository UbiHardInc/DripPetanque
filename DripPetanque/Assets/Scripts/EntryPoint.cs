using UnityEngine;
using UnityEngine.InputSystem;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_asset;

    // Start is called before the first frame update
    private void Start()
    {
        m_asset.Enable();


    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
