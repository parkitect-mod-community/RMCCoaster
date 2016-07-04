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

        binder = new TrackRiderBinder ("d41d8cd98f00b204e9800998ecf8427e");

        TrackedRide trackedRide = binder.RegisterTrackedRide<TrackedRide> ("Wooden Coaster","MineTrainCoaster", "Mine Train Coaster");
        trackedRide.price = 3600;
        trackedRide.dropsImportanceExcitement = .7f;
        trackedRide.inversionsImportanceExcitement = .67f;
        trackedRide.averageLatGImportanceIntensity = .7f;

        trackedRide.carTypes = new CoasterCarInstantiator[]{ };


        MinetrainTrackGenerator meshGenerator =  binder.RegisterMeshGenerator<MinetrainTrackGenerator> (trackedRide);
        TrackRideHelper.PassMeshGeneratorProperties (TrackRideHelper.GetTrackedRide ("Wooden Coaster").meshGenerator,trackedRide.meshGenerator);
        trackedRide.meshGenerator.customColors = new Color[] {
            new Color (63f / 255f, 46f / 255f, 37f / 255f, 1), 
            new Color (43f / 255f, 35f / 255f, 35f / 255f, 1), 
            new Color (90f / 255f, 90f / 255f, 90f / 255f, 1)
        };

        MineTrainSupportInstantiator supportGenerator = binder.RegisterSupportGenerator<MineTrainSupportInstantiator> (trackedRide);

        CoasterCarInstantiator coasterCarInstantiator = binder.RegisterCoasterCarInstaniator<CoasterCarInstantiator> (trackedRide, "MineTrainInstantiator", "Mine Train", 5, 7, 2);
        Color[] CarColors = new Color[] { 
            new Color(68f / 255, 58f / 255, 50f / 255), 
            new Color(176f / 255, 7f / 255, 7f / 255), 
            new Color(55f / 255, 32f / 255, 12f / 255),
            new Color(61f / 255, 40f / 255, 19f / 255)
        };

        BaseCar frontCar = binder.RegisterCar<BaseCar> (Main.AssetBundleManager.FrontCarGo, "MineTrainCartFront", .02f, .4f, true,CarColors);
        BaseCar backCar = binder.RegisterCar<BaseCar> (Main.AssetBundleManager.BackCarGo, "MineTrainCarBack", .02f, .02f, false,CarColors);

        frontCar.gameObject.AddComponent<RestraintRotationController>().closedAngles = new Vector3(0,0,120);
        backCar.gameObject.AddComponent<RestraintRotationController>().closedAngles = new Vector3(0,0,120);

        coasterCarInstantiator.frontCarGO = frontCar.gameObject;
        coasterCarInstantiator.carGO = backCar.gameObject;

        binder.Apply ();

        //deprecatedMappings
        string oldHash = "ASDFawjebfa8pwh9n3a3h8ank";
        GameObjectHelper.RegisterDeprecatedMapping ("mine_train_coaster_GO", trackedRide.name);
        GameObjectHelper.RegisterDeprecatedMapping ("mine_train_coaster_GO"+oldHash, trackedRide.name);

        GameObjectHelper.RegisterDeprecatedMapping ("Mine Train@CoasterCarInstantiator"+oldHash, coasterCarInstantiator.name);
        GameObjectHelper.RegisterDeprecatedMapping ("Mine Train@CoasterCarInstantiator", coasterCarInstantiator.name);

        GameObjectHelper.RegisterDeprecatedMapping ("MineTrainCar_Car"+oldHash, backCar.name);
        GameObjectHelper.RegisterDeprecatedMapping ("MineTrainCar_Car", backCar.name);

        GameObjectHelper.RegisterDeprecatedMapping ("MineTrainCar_Front"+oldHash, frontCar.name);
        GameObjectHelper.RegisterDeprecatedMapping ("MineTrainCar_Front", frontCar.name);


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
        get { return "Mine Train Coaster"; }
    }


	public string Path { get; set; }

}

