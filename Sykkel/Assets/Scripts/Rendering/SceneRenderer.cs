using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneRenderer : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float lookRange = 15;

    void Update() {
        Cursor.visible = false;
        RenderWorld();
    }

    void RenderWorld() {
        List<PlaymateGameObject> objectsInRangeUnsorted = new List<PlaymateGameObject>();
        FindObjectsInRange(ref objectsInRangeUnsorted);
        PlaymateGameObject[] objectsInRange = objectsInRangeUnsorted.OrderBy(x => (x.position - (Vector2)playerTransform.position).sqrMagnitude).ToArray();

        LogObjectPositions(objectsInRange);

        List<Vector2> hitScreenPositions = new List<Vector2>();
        List<PlaymateGameObject> hitObjects = new List<PlaymateGameObject>();
        RaycastObjects(objectsInRange, ref hitScreenPositions, ref hitObjects);

        // For increased performance, raycasts should directly render objects instead of saving them to arrays
        // For a finished game, this would be implemented. 

        RenderRubble(hitScreenPositions.ToArray(), hitObjects.ToArray(), SceneData.rubblePool.ToArray(), SceneData.rubbleRenderers.ToArray());
        RenderClouds(playerTransform.eulerAngles.z, SceneData.cloudPool.ToArray(), SceneData.cloudRenderers.ToArray());
    }

    void FindObjectsInRange(ref List<PlaymateGameObject> objectsInRange) {
        for (int i = 0; i < SceneData.rubbleObjects.Length; i++)
        {
            if ((SceneData.rubbleObjects[i].position - (Vector2)playerTransform.position).sqrMagnitude < lookRange * lookRange) {
                objectsInRange.Add(SceneData.rubbleObjects[i]);
            }
        }
    }

    void LogObjectPositions(PlaymateGameObject[] objects) {
        for (int i = 0; i < objects.Length; i++)
        {
            Color color = Color.Lerp(Color.magenta, Color.black, (float)i / objects.Length);
            Vector2 position = objects[i].position;
            float size = objects[i].size;

            Vector2[] directions = {Vector2.up, Vector2.right, Vector2.left, Vector2.down};
            foreach (Vector2 direction in directions)
                Debug.DrawLine(position, position + direction * size / 2, color);
        }
    }

    void RaycastObjects(PlaymateGameObject[] objectsInRange, ref List<Vector2> hitScreenPositions, ref List<PlaymateGameObject> hitObjects) {
        List<int> hitIndexes = new List<int>();

        int rayAmount = 160;
        for (int i=-rayAmount/2; i<rayAmount/2; i++) {
            Vector2 direction = playerTransform.up + playerTransform.right * i * (1f / rayAmount);

            for (int j = 0; j < objectsInRange.Length; j++)
            {
                if (HitObjectWithRay(objectsInRange[j], direction, j, ref hitIndexes)) {
                    Vector2 hitPosition = new Vector2(i * (30f / rayAmount), (objectsInRange[j].position - (Vector2)playerTransform.position).magnitude);
                    hitScreenPositions.Add(hitPosition);
                    hitObjects.Add(objectsInRange[j]);
                }
            }
        }
    }

    bool HitObjectWithRay(PlaymateGameObject gameObject, Vector2 direction, int index, ref List<int> hitIndexes) {
        if (GetDistPointToLine(playerTransform.position, direction, gameObject.position) > gameObject.size)
            return false;

        if (hitIndexes.Contains(index)) {
            return false;
        }

        hitIndexes.Add(index);
        Debug.DrawLine(playerTransform.position, gameObject.position, Color.green);

        return true;
    }

    static public float GetDistPointToLine(Vector2 origin, Vector2 direction, Vector2 point){
        Vector2 point2origin = origin - point;
        if (180 - Vector2.Angle(point2origin, direction) > 3)
            return 10000;

        Vector2 point2closestPointOnLine = point2origin - Vector2.Dot(point2origin, direction) * direction;
        return point2closestPointOnLine.magnitude;
    }


    public void RenderRubble(Vector2[] screenPositions, PlaymateGameObject[] objects, Transform[] pool, SpriteRenderer[] poolRenderers) {
        for (int i = 0; i < pool.Length; i++)
        {
            if (i >= screenPositions.Length) {
                pool[i].gameObject.SetActive(false);
                continue;
            }

            pool[i].gameObject.SetActive(true);
            pool[i].position = Vector2.right * screenPositions[i].x;
            pool[i].localScale = Vector2.one * Mathf.Clamp(objects[i].size * Mathf.Clamp(lookRange - screenPositions[i].y, 0, lookRange) / screenPositions[i].y, 0, 50);
            poolRenderers[i].sortingOrder = (int)(1000 / screenPositions[i].y);
            poolRenderers[i].sprite = objects[i].sprite;
        }
    }

    public void RenderClouds(float currentRotation, Transform[] pool, SpriteRenderer[] poolRenderers) {
        int usedPoolClouds = 0;
        for (int i = 0; i < SceneData.rotationToClouds.Length; i++)
        {
            float rotation = Mathf.Abs(currentRotation - SceneData.rotationToClouds[i]);
            if (rotation > 180)
                rotation = Mathf.Abs(rotation - 360);

            if (rotation > 60) {
                pool[usedPoolClouds].gameObject.SetActive(false);
                continue;
            }

            pool[usedPoolClouds].gameObject.SetActive(true);
            pool[usedPoolClouds].transform.position = new Vector3((currentRotation - SceneData.rotationToClouds[i]) / 2, 1);
            poolRenderers[usedPoolClouds].sprite = SceneData.spritesOfClouds[i];

            usedPoolClouds ++;
            if (usedPoolClouds >= pool.Length)
                return;
        }
    }
}
