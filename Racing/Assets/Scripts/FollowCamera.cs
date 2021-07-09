using UnityEngine;
using UnityEngine.UI;

public class FollowCamera : MonoBehaviour
{
    public RawImage rearCamView;
    public static Transform PlayerCar;
    public float distance=8f;
    public float height=1.5f;
    public float heightOffset = 1.0f;

    float heightDamping = 3f;
    float rotationDamping = 3f;
    int index;

    Transform[] target;
    int FP = -1;

    private void Start()
    {
        if(PlayerPrefs.HasKey("FP"))
        {
            FP = PlayerPrefs.GetInt("FP");
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            target = new Transform[cars.Length];
            for(int i=0;i<cars.Length;i++)
            {
                target[i] = cars[i].transform;
                if (target[i] == PlayerCar) index = i;
            }
            target[index].Find("Rear Camera").gameObject.GetComponent<Camera>().targetTexture = (rearCamView.texture as RenderTexture);
            return;
        }

        if(FP==1)
        {
            transform.position = target[index].position-target[index].forward*0.2f+target[index].up*0.8f;
            transform.LookAt(target[index].position + target[index].forward * 3);
        }

        else
        {
            float wantedRotatiobAngle = target[index].eulerAngles.y;
            float wantedHeight = target[index].position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotatiobAngle, rotationDamping * Time.deltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = target[index].position;
            transform.position -= currentRotation * Vector3.forward * distance;

            transform.position = new Vector3(transform.position.x, currentHeight + heightOffset, transform.position.z);
            transform.LookAt(target[index]);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            FP = FP * -1;
            PlayerPrefs.SetInt("FP", FP);
        }

        /*   if(Input.GetKeyDown(KeyCode.T))
           {
               target[index].Find("Rear Camera").gameObject.GetComponent<Camera>().targetTexture = null;
               index++;
               if (index > target.Length - 1) index = 0;
               target[index].Find("Rear Camera").gameObject.GetComponent<Camera>().targetTexture = (rearCamView.texture as RenderTexture);
           }
        */
    }
}
