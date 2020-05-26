using UnityEngine;
using System.Collections.Generic;
using TrackedRiderUtility;

public class Main : IMod
{
    public string Identifier { get; set; }
	public static AssetBundleManager AssetBundleManager = null;

    private TrackRiderBinder binder;

   public void onEnabled()
    {

		if (Main.AssetBundleManager == null) {

			AssetBundleManager = new AssetBundleManager (this);
		}

        binder = new TrackRiderBinder ("b67f7c6f2916f2e0edd368112de5a5da");

        TrackedRide trackedRide = binder.RegisterTrackedRide<TrackedRide> ("Mine Train Coaster","MineTrainCoaster", "RMC Coaster");
        trackedRide.price = 3600;
        trackedRide.dropsImportanceExcitement = .7f;
        trackedRide.inversionsImportanceExcitement = .67f;
        trackedRide.averageLatGImportanceExcitement = .7f;

        trackedRide.carTypes = new CoasterCarInstantiator[]{ };


		RMCCoasterMeshGenerator meshGenerator =  binder.RegisterMeshGenerator<RMCCoasterMeshGenerator> (trackedRide);
        TrackRideHelper.PassMeshGeneratorProperties (TrackRideHelper.GetTrackedRide ("Wooden Coaster").meshGenerator,trackedRide.meshGenerator);
        trackedRide.meshGenerator.customColors = new Color[] {
            new Color (63f / 255f, 46f / 255f, 37f / 255f, 1),
            new Color (43f / 255f, 35f / 255f, 35f / 255f, 1),
            new Color (90f / 255f, 90f / 255f, 90f / 255f, 1)
        };

        SupportStub supportGenerator = binder.RegisterSupportGenerator<SupportStub> (trackedRide);

        CoasterCarInstantiator coasterCarInstantiator = binder.RegisterCoasterCarInstaniator<CoasterCarInstantiator> (trackedRide, "MineTrainInstantiator", "Mine Train Car", 5, 7, 2);
        Color[] CarColors = new Color[] {
            new Color(68f / 255, 58f / 255, 50f / 255),
            new Color(176f / 255, 7f / 255, 7f / 255),
            new Color(55f / 255, 32f / 255, 12f / 255),
            new Color(61f / 255, 40f / 255, 19f / 255)
        };

        BaseCar frontCar = binder.RegisterCar<BaseCar> (Main.AssetBundleManager.FrontCarGo, "MineTrainCarFront", .02f, .4f, true,CarColors);
        BaseCar backCar = binder.RegisterCar<BaseCar> (Main.AssetBundleManager.BackCarGo, "MineTrainCarBack", .02f, .02f, false,CarColors);

        frontCar.gameObject.AddComponent<RestraintRotationController>().closedAngles = new Vector3(0,0,120);
        backCar.gameObject.AddComponent<RestraintRotationController>().closedAngles = new Vector3(0,0,120);

        // coasterCarInstantiator.frontCarGO = frontCar.gameObject;
        // coasterCarInstantiator.carGO = backCar.gameObject;


        binder.Apply ();


	}




    public void onDisabled()
    {
        binder.Unload ();
	}

    public string Name
    {
        get { return "Mine Train Coaster"; }
    }

    public string Description
    {
        get { return "A steel mine train coaster."; }
    }


	public string Path { get; set; }

}

