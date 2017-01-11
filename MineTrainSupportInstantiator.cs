using System;
using UnityEngine;

public class MineTrainSupportInstantiator : SupportInstantiator
{
    public Material baseMaterial;

    public override void instantiateSupports (MeshGenerator meshGenerator, TrackSegment4 trackSegment, UnityEngine.GameObject putMeshOnGO)
    {
        CrossedTiles crossedTiles = trackSegment.getCrossedTiles ();
        foreach (CrossedTileInfo current in crossedTiles.crossedTilesInfo) {

            var component = new GameObject ().AddComponent<MineTrainSupports>();//.GetComponent<MineTrainSupports> ();

			component.crossedTiles = crossedTiles.getCrossedSides (current.getWorldX(), current.getWorldZ());
			component.x = current.getWorldX();
			component.y = current.getMinYOnRails();
			component.z = current.getWorldZ();
            component.baseMaterial = meshGenerator.material;
            component.transform.parent = putMeshOnGO.transform;
            component.Initialize ();

        }
    }
}


