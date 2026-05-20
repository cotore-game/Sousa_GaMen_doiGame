using UnityEngine;

public class CameraChecker : MonoBehaviour
{
    private enum Mode
    {
        None,
        Render,
        RenderOut,
    }

    private Mode mode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mode = Mode.None;
    }

    // Update is called once per frame
    void Update()
    {
        Dead();
    }

    private void OWillRenderObject()
    {
         if(Camera.current.name == "Main Camera")
        {
            mode = Mode.Render;
        }
    }

    private void Dead()
    {
        if(mode == Mode.RenderOut)
        {
            Destroy(gameObject);
        }

        if(mode == Mode.Render)
        {
            mode = Mode.RenderOut;
        }
    }
}
