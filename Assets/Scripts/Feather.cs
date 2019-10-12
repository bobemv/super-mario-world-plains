using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour
{
    public bool isPickable = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakePickableTimeout());
    }

    public void StartPosition(Vector3 position) {
        transform.Translate(position);
    }

    IEnumerator MakePickableTimeout() {
        yield return new WaitForSeconds(1.5f);
        isPickable = true;
    }
    // Update is called once per frame
    
}
