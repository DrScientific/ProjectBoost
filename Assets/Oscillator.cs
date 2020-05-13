using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 1;

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period >= Mathf.Epsilon)
        {
            float cycles = Time.time / period;

            float currentSineWaveValue = Mathf.Sin(cycles * Mathf.PI * 2);

            float movementFactor = currentSineWaveValue / 2f + .5f; //Half the waves' amplitude and shift it upwards by 1.

            Vector3 offset = movementVector * movementFactor;
            transform.position = startingPosition + offset;
        }
    }
}
