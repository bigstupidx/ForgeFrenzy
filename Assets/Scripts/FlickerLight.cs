using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour {

    public float maxReduction;
    public float maxIncrease;
    public float rateDamping;
    public float strength;
    public bool stopFlickering;
    [SerializeField]
    GameObject sparks;

    private Light _lightSource;
    private float _baseIntensity;
    private bool _flickering;

    public void Reset()
    {
        maxReduction = 0.2f;
        maxIncrease = 0.2f;
        rateDamping = 0.1f;
        strength = 300;
    }

    public void Start()
    {
        _lightSource = GetComponent<Light>();
        if (_lightSource == null)
        {
            Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
            return;
        }
        _baseIntensity = _lightSource.intensity;
        StartCoroutine(DoFlicker());
    }

    void Update()
    {
        if (!stopFlickering && !_flickering)
        {
            StartCoroutine(DoFlicker());
        }
    }

    private IEnumerator DoFlicker()
    {
        _flickering = true;
        while (!stopFlickering)
        {
            _lightSource.intensity = Mathf.Lerp(_lightSource.intensity, Random.Range(_baseIntensity - maxReduction, _baseIntensity + maxIncrease), strength * Time.deltaTime);
            if (sparks != null && _lightSource.intensity > (0.5f * _baseIntensity))
            {
                sparks.GetComponent<ParticleSystem>().Play();
            }
            yield return new WaitForSeconds(rateDamping);
        }
        _flickering = false;
    }

}
