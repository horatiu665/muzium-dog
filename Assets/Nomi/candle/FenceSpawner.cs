using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToyBoxHHH;
using UnityEngine;
//using Chinchillada.DefaultAsset;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class FenceSpawner : MonoBehaviour
{
    [Header("AutoUpdate")]
    public bool autoUpdate = false;
    public float autoUpdateInterval = 1f;
    private float autoUpdateTimer;

    [Header("Perimeter")]
    public List<Transform> fencePoints = new List<Transform>();
    //[DefaultAsset("[FencePointPrefab]")]
    public GameObject fencePointPrefab;

    [Header("Settings")]
    public bool autoFenceDist = true;
    public float fenceDist = 1f;
    public bool equalSpacingPerSide = true;
    public float fenceSpacing = 0f;
    public float fenceCornerSpacing = 0f;
    public int randomizeSeed = 177717771;
    private Random.State prevRandomState;

    [Space]
    public bool setOnGround = true;
    public bool setOnGroundNormal = true;
    public LayerMask groundLayer = -1;
    public Vector3 rotOffset = new Vector3(0, 0, 0);

    [Header("Fence posts")]
    public GameObject fencePrefab;
    public List<GameObject> experimentalMultipleFences = new List<GameObject>();
    public Transform fenceSpawnParent;
    public List<GameObject> fencesSpawned = new List<GameObject>();

    private List<MeshRenderer> meshesCache = new List<MeshRenderer>();

    private void Reset()
    {
        fenceSpawnParent = transform;
    }

    [DebugButton]
    public void FenceClear()
    {
        for (int i = 0; i < fencesSpawned.Count; i++)
        {
            if (fencesSpawned[i] != null)
            {

                if (Application.isPlaying)
                {
                    Destroy(fencesSpawned[i].gameObject);
                }
                else
                {
                    DestroyImmediate(fencesSpawned[i].gameObject);
                }

            }
        }
        fencesSpawned.Clear();
    }

    public GameObject SpawnOne(Vector3 pos, Quaternion rot)
    {
        if (experimentalMultipleFences.Count > 1)
        {
            if (fencesSpawned.Count == 0)
            {
                fencePrefab = experimentalMultipleFences[0];
            }
            else
            {
                var ef = experimentalMultipleFences[Random.Range(0, experimentalMultipleFences.Count)];
                var monteCarlo = 10;
                while (ef == fencePrefab && monteCarlo-- > 0)
                {
                    ef = experimentalMultipleFences[Random.Range(0, experimentalMultipleFences.Count)];
                }

                fencePrefab = ef;
            }
        }

        GameObject s = null;
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            s = PrefabUtility.InstantiatePrefab(fencePrefab, fenceSpawnParent) as GameObject;
            s.transform.position = pos;
            s.transform.rotation = rot;

            if (!autoUpdate)
                Undo.RegisterCreatedObjectUndo(fencePrefab, fencePrefab.name);
        }
        else
#endif
        {
            s = Instantiate(fencePrefab, pos, rot, fenceSpawnParent);
        }

        fencesSpawned.Add(s);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        return s;
    }

    [DebugButton]
    public void FenceDraw()
    {
        prevRandomState = Random.state;
        Random.InitState(randomizeSeed);

#if UNITY_EDITOR
        if (!autoUpdate)
        {
            Undo.SetCurrentGroupName("FenceDraw");
        }
        int group = Undo.GetCurrentGroup();
#endif

        FenceClear();

        if (fencePoints.Count == 0)
        {
            Debug.LogError("Add some fence points");
            return;
        }

        var point = fencePoints.First();
        var position = point.position;
        for (int i = 1; i < fencePoints.Count; i++)
        {
            var nextPoint = fencePoints[i];
            if (setOnGround)
            {
                if (Physics.Raycast(nextPoint.position + 0.1f * Vector3.up, Vector3.down, out var hit, Mathf.Infinity, groundLayer))
                {
                    nextPoint.position = hit.point;
                }
            }

            // draw fences until distance to next corner < fenceDist
            var towardsNext = nextPoint.position - position;

            // calculate border spacing as half of the remainder after filling the side of the fence
            if (equalSpacingPerSide)
            {
            }

            float remainderDistance = (towardsNext.magnitude - fenceCornerSpacing * 2f) % (fenceDist + fenceSpacing);
            position = point.position + (0.5f * remainderDistance + fenceCornerSpacing + fenceSpacing * 0.5f) * towardsNext.normalized;

            while (towardsNext.magnitude > fenceDist + fenceSpacing * 0.5f + fenceCornerSpacing)
            {
                var f = SpawnOne(position, Quaternion.LookRotation(towardsNext)).transform;
                if (setOnGroundNormal || setOnGround)
                {
                    if (Physics.Raycast(f.position + 0.1f * Vector3.up, Vector3.down, out var hit, Mathf.Infinity, groundLayer))
                    {
                        if (setOnGround)
                            f.position = hit.point;
                        if (setOnGroundNormal)
                            f.rotation = Quaternion.LookRotation(towardsNext, hit.normal) * Quaternion.Euler(rotOffset);

                    }
                }

                if (autoFenceDist)
                {
                    var oldRot = f.rotation;
                    f.rotation = Quaternion.identity;
                    f.GetComponentsInChildren<MeshRenderer>(meshesCache);
                    var firstMesh = meshesCache.FirstOrDefault();
                    if (firstMesh != null)
                    {
                        var bounds = firstMesh.bounds;
                        foreach (var mc in meshesCache)
                        {
                            bounds.Encapsulate(mc.bounds);
                        }
                        fenceDist = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                    }
                    f.rotation = oldRot;
                }

                position += (fenceDist + fenceSpacing) * towardsNext.normalized;
                towardsNext = nextPoint.position - position;
            }

            point = nextPoint;
            position = point.position;

        }

#if UNITY_EDITOR
        if (!autoUpdate)
            Undo.CollapseUndoOperations(group);
#endif

        Random.state = prevRandomState;
    }

    [DebugButton]
    public void FencePerimeterAddPoint()
    {
        var pos = transform.position;
        var rot = transform.rotation;
        if (fencePoints.Count > 0)
        {
            pos = fencePoints.Last().position;
            rot = fencePoints.Last().rotation;
        }

        var newPoint = Instantiate(fencePointPrefab).transform;
        newPoint.position = pos;
        newPoint.rotation = rot;
        newPoint.SetParent(transform);

#if UNITY_EDITOR
        UnityEditor.Selection.activeObject = newPoint.gameObject;

#endif

        fencePoints.Add(newPoint);
    }

    [DebugButton]
    public void FencePerimeterClear()
    {
        foreach (var fp in fencePoints)
        {
            DestroyImmediate(fp.gameObject);
        }
        fencePoints.Clear();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }


    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

    private void EditorUpdate()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            if (autoUpdate)
            {
                if (EditorApplication.timeSinceStartup > autoUpdateTimer)
                {
                    autoUpdateTimer = (float)EditorApplication.timeSinceStartup + autoUpdateInterval;

                    FenceDraw();

                }
            }
        }
#endif
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < fencePoints.Count - 1; i++)
        {
            int i2 = (i + 1) % fencePoints.Count;

            var dir = fencePoints[i2].position - fencePoints[i].position;

            // line
            Gizmos.color = Color.red;
            Gizmos.DrawLine(fencePoints[i].position, fencePoints[i2].position);

            // corner spacing
            Gizmos.color = Color.cyan;
            GizmosDrawPerpLine(fencePoints[i].position + dir.normalized * fenceCornerSpacing, dir, 0.5f);
            GizmosDrawPerpLine(fencePoints[i2].position - dir.normalized * fenceCornerSpacing, dir, 0.5f);


        }

    }
    private void GizmosDrawPerpLine(Vector3 position, Vector3 perpendicularTo, float length)
    {
        var right = Vector3.Cross(perpendicularTo, Vector3.up).normalized;
        Gizmos.DrawLine(position - right * length / 2, position + right * length / 2);

    }

}
