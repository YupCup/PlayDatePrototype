using System.Collections.Generic;
using UnityEngine;

public static class SceneData {
    public static List<Transform> rubblePool = new List<Transform>();
    public static List<Transform> cloudPool = new List<Transform>();

    public static List<SpriteRenderer> rubbleRenderers = new List<SpriteRenderer>();
    public static List<SpriteRenderer> cloudRenderers = new List<SpriteRenderer>();


    public static PlaymateGameObject[] rubbleObjects;

    public static float[] rotationToClouds;
    public static Sprite[] spritesOfClouds;
}