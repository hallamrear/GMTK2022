using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushDice : MonoBehaviour
{
    public GameObject Dice;
    public float MaxForce;
    public float MinForce;

    // Start is called before the first frame update
    void Start()
    {

    }

    [ContextMenu("push")]
    public void push()
    {
        Dice.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(MinForce, MaxForce), Random.Range(MinForce, MaxForce), (Random.Range(MinForce, MaxForce))), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
