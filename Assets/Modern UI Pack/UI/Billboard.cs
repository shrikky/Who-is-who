using UnityEngine;
public class Billboard : MonoBehaviour
{
    public bool LimitTo2DBillbord;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (ObjectMoved())
        {
            if (LimitTo2DBillbord)
                Do2DBillbord();
            else
                DoBillbord();
        }
    }

	private bool ObjectMoved() => (transform.position - Camera.main.transform.position).magnitude > 0;

	private void Do2DBillbord()
    {
        transform.forward = transform.position - mainCamera.transform.position;
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = Vector3.up.x;
        transform.eulerAngles = rot;
    }

    private void DoBillbord()
    {
        var forward = transform.position - mainCamera.transform.position;
        if (forward != Vector3.zero)
            transform.forward = forward;
    }
}
