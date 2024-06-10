using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WellTeleport : MonoBehaviour
{
    public Collider2D Collider;

    public string Scene;
    public GameObject DungeonGenerator;
    public GameObject Camera;
    public GameObject VirtualCamera;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SceneManager.LoadScene(Scene);
            Invoke(nameof(Scan), 0.1f);
            // var dungeon = SceneManager.CreateScene("Dungeon");
            // var cameraInstance = Instantiate(Camera, Vector3.zero, Quaternion.identity);
            // var virtualCameraInstance = Instantiate(VirtualCamera, Vector3.zero, Quaternion.identity);
            // var DGInstance = Instantiate(DungeonGenerator, Vector3.zero, Quaternion.identity);
            // SceneManager.MoveGameObjectToScene(DGInstance, dungeon);
            // SceneManager.MoveGameObjectToScene(cameraInstance, dungeon);
            // SceneManager.MoveGameObjectToScene(virtualCameraInstance, dungeon);
            // StartCoroutine(SwitchToNewScene(dungeon));
        }
    }
    
    private void Scan()
    {
        AstarPath.active.Scan();
    }
    
    private static IEnumerator SwitchToNewScene(Scene newScene)
    {
        yield return new WaitForEndOfFrame();

        var oldScene = SceneManager.GetActiveScene();

        SceneManager.SetActiveScene(newScene);

        SceneManager.UnloadSceneAsync(oldScene);
    }
}
