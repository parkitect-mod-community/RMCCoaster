using System;
using UnityEngine;
using System.Collections.Generic;

public class AssetBundleManager
{
    private readonly Main _main;
    public GameObject FrontCarGo;
    public GameObject BackCarGo;

    public GameObject[] Support_Go;
    public GameObject[] AngledSupportGo;
    public GameObject SupportHalf;

    private readonly AssetBundle assetBundle;



	public AssetBundleManager (Main main)
    {
        this._main = main;
        var dsc = System.IO.Path.DirectorySeparatorChar;
        assetBundle = AssetBundle.LoadFromFile(_main.Path + dsc + "assetbundle" + dsc + "reel");

        FrontCarGo = assetBundle.LoadAsset<GameObject> ("Front_Car");
        BackCarGo = assetBundle.LoadAsset<GameObject> ("Cart");

        Support_Go = new GameObject[] {
            assetBundle.LoadAsset<GameObject> ("Support_1"),
            assetBundle.LoadAsset<GameObject> ("Support_2"),
            assetBundle.LoadAsset<GameObject> ("Support_3 1")
            };

        AngledSupportGo = new GameObject[] {
            assetBundle.LoadAsset<GameObject>("Angle_support_1"),
            assetBundle.LoadAsset<GameObject>("Angle_support_2")
        };

        SupportHalf = assetBundle.LoadAsset<GameObject> ("Half_Support");

        assetBundle.Unload(false);

    }
}


