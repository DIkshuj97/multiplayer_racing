using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Text lapDisplay;
    public Transform target;
    public Renderer carRend;

    CanvasGroup canvasGroup;
    CheckpointManager cpManager;

    int carRegr;
    bool regrSet = false;
   
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(),false);
        playerName = GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void LateUpdate()
    {
        if (!RaceMonitor.racing) { canvasGroup.alpha = 0; return; }

        if(!regrSet)
        {
            carRegr = Leaderboard.RegisterCar(playerName.text);
            regrSet = true;
            return;
        }

        if (carRend == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView?1:0;
        transform.position = Camera.main.WorldToScreenPoint(target.position+Vector3.up*1.2f);

        if (cpManager == null)
            cpManager = target.GetComponent<CheckpointManager>();

        Leaderboard.SetPosition(carRegr, cpManager.lap, cpManager.checkPoint,cpManager.timeEntered);
        string position = Leaderboard.GetPosition(carRegr);
        lapDisplay.text = position;
    }
}
