using UnityEngine;

public class CameraView : MonoBehaviour
{
    public void SetViewBasedOnGrid(int rows, int columns)
    {
        float xPos = (float)rows / 2 - 0.5f;
        float yPos = (float)columns / 2 - 0.5f;
        //Adjust the camera position to the middle of the grid.
        transform.position = new Vector3(xPos, yPos, Camera.main.transform.position.z);
        if (rows < columns - 6) Camera.main.fieldOfView = Mathf.Lerp(60f, 94f, (columns - 5f) / (20f - 5f));
        else Camera.main.fieldOfView = Mathf.Lerp(60f, 130f, (rows - 5f) / (20f - 5f));
    }
}
