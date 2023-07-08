using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mainCamera;
    public Camera mainCamerav2;
    public PlayerController player;
    public float offsetX;
    public float offsetY;
    public float offsetSmoothing;
    public bool isShaking;
    private Vector3 playerPosition;


    // Update is called once per frame
    void FixedUpdate()
    {
        // following and creating explosion at the same time is hard so that when explosion occurs,
        // stop following the player (camera will still follow but not with FollowPlayer() function)
        if (!isShaking)
        {
            FollowPlayer();
        }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        StartCoroutine(StartShaking(duration));
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            GetPlayerPosition();
            playerPosition = new Vector3(playerPosition.x + x, playerPosition.y + y, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, playerPosition, offsetSmoothing*Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

    }
    private void FollowPlayer()
    {
        GetPlayerPosition();
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, playerPosition, offsetSmoothing * Time.deltaTime);
    }
    
    private IEnumerator StartShaking(float duration)
    {
        isShaking = true;
        yield return new WaitForSeconds(duration);
        isShaking = false;
    }
    public void StartExplosion(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    private Vector3 GetPlayerPosition()
    {
        playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, mainCamera.transform.position.z);
        float aimDirectionX = player.GetBodyDirection().normalized.x;
        float aimDirectionY = player.GetBodyDirection().normalized.y;
        playerPosition = new Vector3(player.transform.position.x + (offsetX * aimDirectionX),
                                     player.transform.position.y + (offsetY * aimDirectionY),
                                     mainCamera.transform.position.z);
        return playerPosition;
    }
}
