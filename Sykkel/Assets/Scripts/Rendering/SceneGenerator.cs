using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    [SerializeField] GameObject rubbleRenderObject;
    [SerializeField] GameObject cloudRenderObject;

    [SerializeField] Sprite[] rubbleSprites;
    [SerializeField] Sprite[] cloudSprites;
    
    [SerializeField] int rubbleObjectsAmountInPool;
    [SerializeField] int rubbleObjectsAmountTotal;
    [SerializeField] int cloudAmountInPool;
    [SerializeField] int cloudAmountTotal;

    [SerializeField] Bounds rubbleArea;


    void Start() {
        GenerateWorld();
    }

    void GenerateWorld() {
        FillPool(ref SceneData.rubblePool, ref SceneData.rubbleRenderers, rubbleRenderObject, rubbleObjectsAmountInPool, enabled: false);
        FillPool(ref SceneData.cloudPool, ref SceneData.cloudRenderers, cloudRenderObject, cloudAmountInPool, enabled: true);

        GenerateRubble(ref SceneData.rubbleObjects);
        GenerateClouds(ref SceneData.rotationToClouds, ref SceneData.spritesOfClouds);
    }

    void FillPool(ref List<Transform> pool, ref List<SpriteRenderer> renderers, GameObject fillObject, int amount, bool enabled) {
        for (int i = 0; i < amount; i++)
        {
            Transform newTransform = Instantiate(fillObject, transform.position, Quaternion.identity, transform).transform;
            newTransform.gameObject.SetActive(enabled);
            pool.Add(newTransform);
            renderers.Add(newTransform.GetComponent<SpriteRenderer>());
        }
    }

    void GenerateRubble(ref PlaymateGameObject[] rubbleObjects) {
        rubbleObjects = new PlaymateGameObject[rubbleObjectsAmountTotal];
        for (int i = 0; i < rubbleObjects.Length; i++)
        {
            rubbleObjects[i] = new PlaymateGameObject
            {
                position = new Vector3(Random.Range(-rubbleArea.min.x, -rubbleArea.max.x), Random.Range(rubbleArea.min.y, rubbleArea.max.y)/2) + rubbleArea.center,
                size = Random.Range(.1f, 1f),
                sprite = rubbleSprites[Random.Range(0, rubbleSprites.Length)]
            };
            Debug.Log(rubbleObjects[i].position);

            Debug.DrawLine(rubbleObjects[i].position, rubbleObjects[i].position + Vector2.up, Color.blue, 1);
        }
    }
    

    void GenerateClouds(ref float[] rotationToClouds, ref Sprite[] spritesOfClouds) {
        rotationToClouds = new float[cloudAmountTotal];
        spritesOfClouds = new Sprite[cloudAmountTotal];
        for (int i = 0; i < cloudAmountTotal; i++)
        {
            rotationToClouds[i] = 360 / cloudAmountTotal * i + Random.Range(1, 10);
            spritesOfClouds[i] = cloudSprites[Random.Range(0, cloudSprites.Length)];
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(rubbleArea.center, rubbleArea.size);
    }
}
