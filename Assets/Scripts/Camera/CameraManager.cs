using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField ,Header("ShakeTime")]
    private float ShakeTime;
    [SerializeField, Header("ShakeMagnitude")]
    private float ShakeMagnitude;

    private PlayerMover player;
    private Vector3 initPos;
    private float shakeCount;
    private int currentPlayerHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerMover>();
        currentPlayerHP = player.GetHP();
    }

    // Update is called once per frame
    void Update()
    {
        ShakeCheck();
        FollowPlayer();
    }

    private void ShakeCheck()
    {
        if(currentPlayerHP != player.GetHP())
        {
            currentPlayerHP = player.GetHP();
            shakeCount = 0.0f;
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        Vector3 initPos = transform.position;

        while(shakeCount < ShakeTime)
        {
            float x = initPos.x + Random.Range(- ShakeMagnitude, ShakeMagnitude);
            float y = initPos.y + Random.Range(- ShakeMagnitude, ShakeMagnitude);
            transform.position = new Vector3(x,y,initPos.z);

            shakeCount += Time.deltaTime;

            yield return null;
        }

        transform.position = initPos;
    }

    private void FollowPlayer()
    {
        float x = player.transform.position.x;
        x = Mathf.Clamp(x, initPos.x, Mathf.Infinity);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
