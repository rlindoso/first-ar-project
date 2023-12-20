using UnityEngine;

public class DBInteractionScript : MonoBehaviour
{
    private CreateDBScript createDBScript;
    // Start is called before the first frame update
    void Start()
    {
        createDBScript = GetComponent<CreateDBScript>();
    }

    // Update is called once per frame
    public void OnButtonClick()
    {
        Debug.Log("botão clicado");
    }
}

