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
		leftBeamExtruder.setUV(0, 9);
		rightBeamExtruder = new BoxExtruder(.04782f, .03054f);
		rightBeamExtruder.setUV(0, 9);

		crossBeamOuterExtruder = new BoxExtruder(.04782f, .1f);
		crossBeamOuterExtruder.closeEnds = true;
		crossBeamInnerExtruder = new BoxExtruder(.04782f, .1f);
		crossBeamInnerExtruder.closeEnds = true;

		collisionMeshExtruder = new BoxExtruder(trackWidth, .03054f);
		buildVolumeMeshExtruder = new BoxExtruder(this.getRailWidth(trackSegment), .03054f);
		buildVolumeMeshExtruder.closeEnds = true;

		mainDropdownSideSupports = new BoxExtruder(.02f,.08f);
		mainDropdownSideSupports.closeEnds = true;

		setModelExtruders(rightBeamExtruder, leftBeamExtruder);
	}


	public override void sampleAt(TrackSegment4 trackSegment, float t)
	{
		base.sampleAt(trackSegment, t);

		Vector3 normal = trackSegment.getNormal(t);
		Vector3 trackPivot = getTrackPivot(trackSegment.getPoint(t), normal);
		Vector3 tangentPoint = trackSegment.getTangentPoint(t);
		Vector3 binormal = Vector3.Cross(normal, tangentPoint).normalized;

		Vector3 leftRail = trackPivot + binormal * trackWidth / 2f;
		Vector3 rightRail = trackPivot - binormal * trackWidth / 2f;
		Vector3 midPoint = trackPivot + normal * getCenterPointOffsetY();

		leftBeamExtruder.extrude(leftRail, tangentPoint, normal);
		rightBeamExtruder.extrude(rightRail, tangentPoint, normal);
		collisionMeshExtruder.extrude(trackPivot, tangentPoint, normal);

		if (liftExtruder != null)
		{
			liftExtruder.extrude(midPoint, tangentPoint, normal);
		}
	}


	public override void afterExtrusion(TrackSegment4 trackSegment, GameObject putMeshOnGO)
	{
		base.afterExtrusion(trackSegment, putMeshOnGO);

		float sample = trackSegment.getLength() / Mathf.RoundToInt(trackSegment.getLength() / crossBeamSpacing);
		float pos = 0.0f;
		int index = 0;
		while (pos < trackSegment.getLength())
		{
			float tForDistance = trackSegment.getTForDistance(pos);

			index++;
			pos += sample;

			Vector3 normal = trackSegment.getNormal(tForDistance);
			Vector3 tangentPoint = trackSegment.getTangentPoint(tForDistance);
			Vector3 binormal = Vector3.Cross(normal, tangentPoint).normalized;
			Vector3 trackPivot = getTrackPivot(trackSegment.getPoint(tForDistance), normal);
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
		trackWidth = .34406f;
		crossBeamSpacing = .5f;
	}

	public override Extruder getBuildVolumeMeshExtruder()
	{
		return buildVolumeMeshExtruder;
	}


	public override Mesh getCollisionMesh(GameObject putMeshOnGO)
	{
		return collisionMeshExtruder.getMesh(putMeshOnGO.transform.worldToLocalMatrix);
	}

	public override Mesh getMesh(GameObject putMeshOnGO)
	{
		return MeshCombiner.start().add(leftBeamExtruder, rightBeamExtruder, crossBeamOuterExtruder, crossBeamInnerExtruder, mainDropdownSideSupports).end(putMeshOnGO.transform.worldToLocalMatrix);
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

    public override float getTunnelWidth(TrackSegment4 trackSegment, float t)
    {
        return 0.7f;
    }

    public override float getTunnelHeight()
	{
		return 0.95f;
	}


}

