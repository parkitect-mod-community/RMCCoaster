using System.Collections.Generic;
using TrackedRiderUtility;
using UnityEngine;

public class MineTrainSupports : Support
{

    protected GameObject instance;


    public float x;

    public float y;

    public float z;

    public int crossedTiles;



    public Material baseMaterial;

    protected override void createInstanceObject()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        instance = new GameObject("instance");
        instance.transform.parent = transform;
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        instance.gameObject.AddComponent<MeshFilter>();
        instance.gameObject.AddComponent<MeshRenderer>();
        instance.isStatic = true;

    }

    protected override bool build()
    {

        int index = 0;
        base.build();

        Vector3 position = new Vector3((int) x + 0.5f, y - .2f, (int) z + 0.5f);
        LandPatch terrain = GameController.Instance.park.getTerrain(position);
        if (terrain == null)
            return false;

        float lowest = terrain.getLowestHeight();

        int start = Mathf.FloorToInt(y) - 1;
        if (start < lowest)
            return false;

        for (int i = start; i >= lowest; i--)
        {
            index++;
            GameObject support;

            if (crossedTiles == 9 || crossedTiles == 3 || crossedTiles == 6 || crossedTiles == 12)
            {
                support = Instantiate(
                    Main.AssetBundleManager.AngledSupportGo[
                        index % Main.AssetBundleManager.AngledSupportGo.Length]);
            }
            else
            {
                support = Instantiate(
                    Main.AssetBundleManager.Support_Go[index % Main.AssetBundleManager.Support_Go.Length]);
            }

            position.y = i;
            support.transform.position = position;
            support.transform.parent = transform;
            support.isStatic = true;

            if (crossedTiles == 9)
                support.transform.Rotate(Vector3.forward, 180f);
            else if (crossedTiles == 3)
                support.transform.Rotate(Vector3.forward, 90f);
            else if (crossedTiles == 6)
                support.transform.Rotate(Vector3.forward, 0f);
            else if (crossedTiles == 12)
                support.transform.Rotate(Vector3.forward, -90f);
            else
                support.transform.Rotate(Vector3.forward, ((index + 4) % 4) * 90f);

            //support.GetComponent<Renderer> ().sharedMaterial = baseMaterial;
            GameObjectHelper.SetUV(support, 14, 15);

            BoundingBox boundingBox2 = instance.AddComponent<BoundingBox>();
            boundingBox2.layers = BoundingVolume.Layers.Support;
            boundingBox2.setBounds(new Bounds(Vector3.up / 2f, Vector3.one));
            boundingBox2.setManuallyPositioned();
            boundingBox2.setPosition(position, support.transform.rotation);
            boundingVolumes.Add(boundingBox2);

            support.SetActive(false);
        }

        RebuildMesh();
        return true;
    }

    private void RebuildMesh()
    {
        List<CombineInstance> combine = new List<CombineInstance>();

        Destroy(instance.GetComponent<MeshFilter>().sharedMesh);
        instance.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        instance.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine.ToArray());
        instance.gameObject.SetActive(true);

        instance.GetComponent<Renderer>().sharedMaterial = baseMaterial;

    }
}


