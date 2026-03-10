using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{ 
    

    IEnumerator OnCollisionEnter()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
