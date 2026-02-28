using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Reads a managed event queue from ECS (or polls DamageBuffer) to spawn floating damage text.
/// For MVP, this is a simplified approach.
/// </summary>
public class DamageNumberSpawner : MonoBehaviour
{
    public GameObject DamageNumberPrefab; // World-space Canvas text
    public float FloatSpeed = 2f;
    public float LifeTime = 0.8f;

    // Pool
    private Queue<GameObject> _pool = new Queue<GameObject>();

    public void SpawnDamageNumber(Vector3 worldPos, float damage)
    {
        GameObject go;
        if (_pool.Count > 0)
        {
            go = _pool.Dequeue();
            go.SetActive(true);
        }
        else
        {
            if (DamageNumberPrefab == null) return;
            go = Instantiate(DamageNumberPrefab);
        }

        go.transform.position = worldPos;
        var tmp = go.GetComponentInChildren<TMPro.TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = ((int)damage).ToString();
            tmp.color = damage > 20f ? Color.yellow : Color.white;
        }

        StartCoroutine(FloatAndReturn(go));
    }

    private System.Collections.IEnumerator FloatAndReturn(GameObject go)
    {
        float elapsed = 0f;
        Vector3 startPos = go.transform.position;

        while (elapsed < LifeTime)
        {
            elapsed += Time.deltaTime;
            go.transform.position = startPos + Vector3.up * FloatSpeed * elapsed;

            // Fade
            var tmp = go.GetComponentInChildren<TMPro.TextMeshPro>();
            if (tmp != null)
            {
                Color c = tmp.color;
                c.a = 1f - (elapsed / LifeTime);
                tmp.color = c;
            }

            yield return null;
        }

        go.SetActive(false);
        _pool.Enqueue(go);
    }
}