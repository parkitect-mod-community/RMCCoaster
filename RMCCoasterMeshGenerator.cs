using System;
using UnityEngine;

public class RMCCoasterMeshGenerator : MeshGenerator
{
	private BoxExtruder collisionMeshExtruder;

	private BoxExtruder leftBeamExtruder;
	private BoxExtruder rightBeamExtruder;

	private const float mainCrossBeamSupportDistance = .3f;

	private BoxExtruder crossBeamOuterExtruder;
	private BoxExtruder crossBeamInnerExtruder;

	private BoxExtruder mainDropdownSideSupports;
	private TubeExtruder CementPillerExtruder;

	public override void prepare(TrackSegment4 trackSegment, GameObject putMeshOnGO)
	{
		base.prepare(trackSegment, putMeshOnGO);

		leftBeamExtruder = new BoxExtruder(.04782f, .03054f);
		this.leftBeamExtruder.setUV(0, 9);
		rightBeamExtruder = new BoxExtruder(.04782f, .03054f);
		this.rightBeamExtruder.setUV(0, 9);

		this.crossBeamOuterExtruder = new BoxExtruder(.04782f, .1f);
		this.crossBeamOuterExtruder.closeEnds = true;
		this.crossBeamInnerExtruder = new BoxExtruder(.04782f, .1f);
		this.crossBeamInnerExtruder.closeEnds = true;

		this.collisionMeshExtruder = new BoxExtruder(base.trackWidth, .03054f);
		this.buildVolumeMeshExtruder = new BoxExtruder(this.getRailWidth(), .03054f);
		this.buildVolumeMeshExtruder.closeEnds = true;

		this.mainDropdownSideSupports = new BoxExtruder(.02f,.08f);
		this.mainDropdownSideSupports.closeEnds = true;

		base.setModelExtruders(new Extruder[]{
			rightBeamExtruder,
			leftBeamExtruder
		});
	}


	public override void sampleAt(TrackSegment4 trackSegment, float t)
	{
		base.sampleAt(trackSegment, t);

		Vector3 normal = trackSegment.getNormal(t);
		Vector3 trackPivot = base.getTrackPivot(trackSegment.getPoint(t, 0), normal);
		Vector3 tangentPoint = trackSegment.getTangentPoint(t);
		Vector3 binormal = Vector3.Cross(normal, tangentPoint).normalized;

		Vector3 leftRail = trackPivot + binormal * base.trackWidth / 2f;
		Vector3 rightRail = trackPivot - binormal * base.trackWidth / 2f;
		Vector3 midPoint = trackPivot + normal * this.getCenterPointOffsetY();

		this.leftBeamExtruder.extrude(leftRail, tangentPoint, normal);
		this.rightBeamExtruder.extrude(rightRail, tangentPoint, normal);
		this.collisionMeshExtruder.extrude(trackPivot, tangentPoint, normal);

		if (this.liftExtruder != null)
		{
			this.liftExtruder.extrude(midPoint, tangentPoint, normal);
		}
	}


	public override void afterExtrusion(TrackSegment4 trackSegment, GameObject putMeshOnGO)
	{
		base.afterExtrusion(trackSegment, putMeshOnGO);

		float sample = trackSegment.getLength(0) / (float)Mathf.RoundToInt(trackSegment.getLength(0) / this.crossBeamSpacing);
		float pos = 0.0f;
		int index = 0;
		while (pos < trackSegment.getLength(0))
		{
			float tForDistance = trackSegment.getTForDistance(pos, 0);

			index++;
			pos += sample;

			Vector3 normal = trackSegment.getNormal(tForDistance);
			Vector3 tangentPoint = trackSegment.getTangentPoint(tForDistance);
			Vector3 binormal = Vector3.Cross(normal, tangentPoint).normalized;
			Vector3 trackPivot = base.getTrackPivot(trackSegment.getPoint(tForDistance, 0), normal);
			Vector3 binormalFlat = Vector3.Cross(Vector3.up, tangentPoint).normalized;


			if (!(trackSegment is Station))
			{
				crossBeamOuterExtruder.extrude(trackPivot + tangentPoint * (mainDropdownSideSupports.width)  + normal * crossBeamOuterExtruder.width + binormal * mainCrossBeamSupportDistance, -1f * binormal, Vector3.up);
				crossBeamOuterExtruder.extrude(trackPivot + tangentPoint * (mainDropdownSideSupports.width) + normal * crossBeamOuterExtruder.width - binormal * mainCrossBeamSupportDistance, -1f * binormal, Vector3.up);
				crossBeamOuterExtruder.end();

				crossBeamInnerExtruder.extrude(trackPivot - tangentPoint * (mainDropdownSideSupports.width ) + normal * crossBeamOuterExtruder.width  + binormal * mainCrossBeamSupportDistance, -1f * binormal, Vector3.up);
				crossBeamInnerExtruder.extrude(trackPivot - tangentPoint * (mainDropdownSideSupports.width) + normal * crossBeamOuterExtruder.width  - binormal * mainCrossBeamSupportDistance, -1f * binormal, Vector3.up);
				crossBeamInnerExtruder.end();

				{
					Vector3 dropDownPoint = trackPivot + normal * (crossBeamOuterExtruder.width / 2.0f) + binormal * mainCrossBeamSupportDistance;
					float height= GameController.Instance.park.getHeightAt(dropDownPoint);
					mainDropdownSideSupports.extrude(dropDownPoint, Vector3.down, -1f * binormal);
					mainDropdownSideSupports.extrude(new Vector3(dropDownPoint.x, height,dropDownPoint.z), Vector3.down, -1f * binormal);
					mainDropdownSideSupports.end();
				}

				{
					Vector3 dropDownPoint = trackPivot + normal * (crossBeamOuterExtruder.width / 2.0f) - binormal * mainCrossBeamSupportDistance;
					float height = GameController.Instance.park.getHeightAt(dropDownPoint);
					mainDropdownSideSupports.extrude(dropDownPoint, Vector3.down, -1f * binormal);
					mainDropdownSideSupports.extrude(new Vector3(dropDownPoint.x,height,dropDownPoint.z), Vector3.down, -1f * binormal);
					mainDropdownSideSupports.end();
				}

				//TODO: check banking and deterim if track needs secondary angled out supports

			}
		}
	}


	protected override void Initialize()
	{
		base.Initialize();
		base.trackWidth = .34406f;
		this.crossBeamSpacing = .5f;
	}

	public override Extruder getBuildVolumeMeshExtruder()
	{
		return this.buildVolumeMeshExtruder;
	}


	public override Mesh getCollisionMesh(GameObject putMeshOnGO)
	{
		return collisionMeshExtruder.getMesh(putMeshOnGO.transform.worldToLocalMatrix);
	}

	public override Mesh getMesh(GameObject putMeshOnGO)
	{
		return MeshCombiner.start().add(new Extruder[]{
			this.leftBeamExtruder,
			this.rightBeamExtruder,
			this.crossBeamOuterExtruder,
			this.crossBeamInnerExtruder,
			this.mainDropdownSideSupports
		}).end(putMeshOnGO.transform.worldToLocalMatrix);
	}

	public override float trackOffsetY()
	{
		return .065f;
	}

	protected override float railHalfHeight()
	{
		return .065f;
	}


	public override float getSupportOffsetY()
	{
		return 0.05f;
	}

	public override float getTunnelOffsetY()
	{
		return 0.15f;
	}

	public override float getTunnelWidth()
	{
		return 0.7f;
	}

	public override float getTunnelHeight()
	{
		return 0.95f;
	}


}

