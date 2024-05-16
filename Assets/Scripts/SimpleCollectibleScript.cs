using UnityEngine;
using UnityEngine.Audio;

public class SimpleCollectibleScript : MonoBehaviour
{

    [SerializeField]AudioClip audioClip;
    [SerializeField]AudioMixer mixer;
    [SerializeField] GameObject collectEffect;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    float minY, maxY;

    private void Awake()
    {
        minY = transform.position.y - 0.5f;
        maxY = transform.position.y + 0.5f;
    }

    //υπαρχει απλα για να εχει ενα μικρο animation το gameobject(up-down και rotate)
    void Update()
    {
        float verticalMovement = Mathf.Sin(Time.time * moveSpeed);
        float mappedVerticalMovement = Mathf.Lerp(minY, maxY, (verticalMovement + 1) / 2);

        transform.position = new Vector3(transform.position.x, mappedVerticalMovement, transform.position.z);

        float rotationAmount = Time.deltaTime * rotateSpeed;
        transform.Rotate(Vector3.up, rotationAmount, Space.World);
    }

    //αν κανει collide με τον παιχτη τρεχει την συναρτηση collect
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Collect(other);
        }
    }

    //καλει την συναρτηση heal απο το attributes manager του παιχτη, παιζει το pickup soundeffect και το particle system και μετα κανει destroy το gameobject
    void Collect(Collider col)
    {
        col.GetComponent<AttributesManager>().Heal(50);
        mixer.GetFloat("SFXVolume", out float SFXVolume);
        SFXVolume = MapValue(SFXVolume);
        AudioSource.PlayClipAtPoint(audioClip, transform.position, SFXVolume);
        Instantiate(collectEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    //για να μετατρεψω το -80 μεχρι 0 που εχει το audioMixer σε 0 μεχρι 1 που παιρνει η συναρτηση PlayClipAtPoint
    float MapValue(float value)
    {
        return(value + 80) / 80;
    }
}
