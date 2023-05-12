using UnityEngine;
using System.Collections;
using UnityEngine.XR;
using System;
using System.Collections.Generic;

public class MirrorCameraMover : MonoBehaviour
{

    // sceneCamera (in VR it's the HMD)
    private Camera sceneCamera;
    // Create a camera body and render texture for the left eye ( If not in VR then this will be the main camera)
    private Camera mirrorCameraLeftEye;
    private RenderTexture mirrorTextureLeftEye;
    // Create a camera body and render texture for the right eye
    private Camera mirrorCameraRightEye; 
    private RenderTexture mirrorTextureRightEye;

    // unclear what does this do
    public int renderTextureSize = 256;

    
    private Matrix4x4 flip;
    public Color mirrorBackgroundColor = new Color(0.5f, 0, 0, 1);
    public float RimPower = 0.25f;
    public Plane mirrorPlane;
    private bool shaderVariablesSet = false;

    private bool runningInVR = false;



    private void Start()
    {

        // use XRUTil to check if AR/VR is present 
        runningInVR = MyXRUtil.isPresent();
        sceneCamera = Camera.main;

        GameObject mirrorCameraLeftEyeGO = new GameObject("mirrorCameraLeftEye");
        mirrorCameraLeftEyeGO.AddComponent<Camera>();
        mirrorCameraLeftEye = mirrorCameraLeftEyeGO.GetComponent<Camera>();
        // create left eye texture
        mirrorTextureLeftEye = new RenderTexture(renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
        mirrorTextureLeftEye.Create();
        mirrorCameraLeftEye.targetTexture = mirrorTextureLeftEye;
        mirrorCameraLeftEye.orthographic = sceneCamera.orthographic;
        mirrorCameraLeftEye.fieldOfView = sceneCamera.fieldOfView;
        mirrorCameraLeftEye.depth = sceneCamera.depth - 1;
        mirrorCameraLeftEye.nearClipPlane = 0.01f;


        int layer = LayerMask.NameToLayer("Mirror");
        if (layer == -1)
        {
            for (int i = 256; i < 2147483647; i *= 2)
            {
                if (LayerMask.LayerToName(i) == "")
                {
                    layer = i;
                }
            }
        }


        mirrorCameraLeftEye.GetComponent<Camera>().cullingMask = 1 << layer;
        mirrorCameraLeftEye.cullingMask = (1 << sceneCamera.cullingMask) | ~(1 << layer);

        mirrorCameraLeftEye.backgroundColor = mirrorBackgroundColor;
        mirrorCameraLeftEye.clearFlags = CameraClearFlags.SolidColor;
        mirrorCameraLeftEye.aspect = sceneCamera.aspect;

        if (runningInVR)
        {
            //VR mode with two cameras
            GameObject MirrorCameraGO2 = new GameObject("mirrorCameraRightEye");
            MirrorCameraGO2.AddComponent<Camera>();
            mirrorCameraRightEye = MirrorCameraGO2.GetComponent<Camera>();

            mirrorTextureRightEye = new RenderTexture(renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
            mirrorTextureRightEye.Create();
            mirrorCameraRightEye.targetTexture = mirrorTextureRightEye;
            mirrorCameraRightEye.orthographic = sceneCamera.orthographic;
            mirrorCameraRightEye.fieldOfView = sceneCamera.fieldOfView;
            mirrorCameraRightEye.depth = sceneCamera.depth - 1;
            mirrorCameraRightEye.nearClipPlane = 0.01f;

            mirrorCameraRightEye.GetComponent<Camera>().cullingMask = 1 << layer;
            mirrorCameraRightEye.cullingMask = (1 << sceneCamera.cullingMask) | ~(1 << layer);

            mirrorCameraRightEye.backgroundColor = mirrorBackgroundColor;
            mirrorCameraRightEye.clearFlags = CameraClearFlags.SolidColor;
            mirrorCameraRightEye.aspect = sceneCamera.aspect;
        }

        this.gameObject.layer = layer;

        if (!shaderVariablesSet)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;
                if (child.GetComponent<MeshRenderer>().material.shader.name.Equals("Custom/MirrorSurface"))
                {
                    child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mirrorTextureLeftEye);
                    if (runningInVR)
                    {
                        child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex2", mirrorTextureRightEye);
                    }
                    child.GetComponent<MeshRenderer>().material.SetFloat("_RimPower", RimPower);
                    shaderVariablesSet = true;
                }
            }
        }

        flip = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));


    }




    void LateUpdate()
    {
        Matrix4x4 reflectionSurfaceLocalLocalToWorldMatrix = this.transform.localToWorldMatrix;// Matrix4x4.TRS(reflectionSurface.position, reflectionSurface.rotation, reflectionSurface.lossyScale);
        Matrix4x4 reflectionSurfaceLocalWorldToLocalMatrix = this.transform.worldToLocalMatrix;
        //Matrix4x4 mirroredCameraTransform = (flip*reflectionSurface.transform.worldToLocalMatrix*Camera.main.transform.localToWorldMatrix).inverse;
        //probe.transform.SetPositionAndRotation(PositionFromMatrix(mirroredCameraTransform), RotationFromMatrix(mirroredCameraTransform));

        Vector3 mirroredForward;
        Vector3 mirroredUp;

        if (runningInVR)
        //if (MyXRUtil.isPresent())
        {//if(UnityEngine.XR.XRDevice.isPresent) {
            //VR mode with two cameras
            Vector3 leftEyePos = Vector3.zero;// deprectated: UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftEye);
            Vector3 rightEyePos = Vector3.zero; ;// deprectated: UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightEye);
            UnityEngine.XR.InputDevice deviceLeftEye = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftEye);
            UnityEngine.XR.InputDevice deviceRightEye = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightEye);
            UnityEngine.XR.InputDevice deviceHead = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head);
            if (deviceLeftEye.isValid)
            {
                deviceLeftEye.TryGetFeatureValue(UnityEngine.XR.CommonUsages.leftEyePosition, out leftEyePos);
            }
            if (deviceRightEye.isValid)
            {
                deviceRightEye.TryGetFeatureValue(UnityEngine.XR.CommonUsages.rightEyePosition, out rightEyePos);
            }

            mirrorCameraLeftEye.transform.position = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyPoint3x4(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyPoint3x4(leftEyePos));
            mirrorCameraRightEye.transform.position = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyPoint3x4(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyPoint3x4(rightEyePos));


            Quaternion leftEyeRotation = Quaternion.identity;
            Quaternion rightEyeRotation = Quaternion.identity;
            Quaternion headRotation = sceneCamera.transform.rotation;
            if (deviceLeftEye.isValid)
            {
                deviceLeftEye.TryGetFeatureValue(UnityEngine.XR.CommonUsages.leftEyeRotation, out leftEyeRotation);
            }
            if (deviceRightEye.isValid)
            {
                deviceRightEye.TryGetFeatureValue(UnityEngine.XR.CommonUsages.rightEyeRotation, out rightEyeRotation);
            }

            //VR mode with two cameras
            mirroredForward = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.TransformDirection((leftEyeRotation * Quaternion.Inverse(headRotation)) * Vector3.forward)));
            mirroredUp = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.TransformDirection((leftEyeRotation * Quaternion.Inverse(headRotation)) * Vector3.up)));
            mirrorCameraLeftEye.transform.rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);


            mirroredForward = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.TransformDirection((rightEyeRotation * Quaternion.Inverse(headRotation)) * Vector3.forward)));
            mirroredUp = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.TransformDirection((rightEyeRotation * Quaternion.Inverse(headRotation)) * Vector3.up)));
            mirrorCameraRightEye.transform.rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);
        }
        else
        {
            mirrorCameraLeftEye.transform.position = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyPoint3x4(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyPoint3x4(sceneCamera.transform.position));

            mirroredForward = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.forward));
            mirroredUp = (reflectionSurfaceLocalLocalToWorldMatrix * flip).MultiplyVector(reflectionSurfaceLocalWorldToLocalMatrix.MultiplyVector(sceneCamera.transform.up));
            mirrorCameraLeftEye.transform.rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);
        }


        mirrorCameraLeftEye.backgroundColor = mirrorBackgroundColor;
        if (runningInVR)
        {
            mirrorCameraRightEye.backgroundColor = mirrorBackgroundColor;
        }


        Matrix4x4 camProj;
        Matrix4x4 vrproj;
        Vector4 clipPlaneMirrorCameraSpace;

        mirrorPlane = new Plane(this.transform.up, this.transform.position);
        
        if (runningInVR)
        //if (MyXRUtil.isPresent())
        {//if(UnityEngine.XR.XRDevice.isPresent) {
            //VR mode with two cameras
            if (mirrorPlane.GetSide(mirrorCameraLeftEye.transform.position))
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraLeftEye, this.transform.position, -this.transform.up, 1.0f);
            }
            else
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraLeftEye, this.transform.position, this.transform.up, 1.0f);
            }

            camProj = mirrorCameraLeftEye.CalculateObliqueMatrix(clipPlaneMirrorCameraSpace);

            vrproj = sceneCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right); //Swap eyes because were rendering to a mirror

            camProj[0, 0] = 1.0f / sceneCamera.aspect * vrproj[1, 1];
            camProj[1, 1] = vrproj[1, 1];

            camProj[0, 2] = vrproj[0, 2]; //off axis projection principal point x
            camProj[1, 2] = vrproj[1, 2]; //off axis projection principal point y

            mirrorCameraLeftEye.projectionMatrix = camProj;

            if (mirrorPlane.GetSide(mirrorCameraRightEye.transform.position))
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraRightEye, this.transform.position, -this.transform.up, 1.0f);
            }
            else
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraRightEye, this.transform.position, this.transform.up, 1.0f);
            }

            camProj = mirrorCameraRightEye.CalculateObliqueMatrix(clipPlaneMirrorCameraSpace);

            vrproj = sceneCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left); //Swap eyes because were rendering to a mirror

            camProj[0, 0] = 1.0f / sceneCamera.aspect * vrproj[1, 1];
            camProj[1, 1] = vrproj[1, 1];

            camProj[0, 2] = vrproj[0, 2]; //off axis projection principal point x
            camProj[1, 2] = vrproj[1, 2]; //off axis projection principal point y

            mirrorCameraRightEye.projectionMatrix = camProj;

        }
        else
        {
            if (mirrorPlane.GetSide(mirrorCameraLeftEye.transform.position))
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraLeftEye, this.transform.position, -this.transform.up, 1.0f);
            }
            else
            {
                clipPlaneMirrorCameraSpace = CameraSpacePlane(mirrorCameraLeftEye, this.transform.position, this.transform.up, 1.0f);
            }

            camProj = sceneCamera.CalculateObliqueMatrix(clipPlaneMirrorCameraSpace);
            camProj[0, 2] = -camProj[0, 2];
            camProj[1, 2] = -camProj[1, 2];
            //camProj[0, 3] = -camProj[0, 3];


            //camProj[0, 0] = 1.0f / sceneCamera.aspect * sceneCamera.projectionMatrix[1, 1];
            //camProj[1, 1] = sceneCamera.projectionMatrix[1, 1];

            mirrorCameraLeftEye.projectionMatrix = camProj;
        }


        if (!shaderVariablesSet)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<MeshRenderer>().material.shader.name.Equals("Custom/MirrorSurface"))
                {
                    child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mirrorTextureLeftEye);
                    if (runningInVR)
                    {//if(UnityEngine.XR.XRDevice.isPresent) {
                        child.GetComponent<MeshRenderer>().material.SetTexture("_MainTex2", mirrorTextureRightEye);
                    }
                    child.GetComponent<MeshRenderer>().material.SetFloat("_RimPower", RimPower);
                    shaderVariablesSet = true;
                }
            }
        }
    }


    static Vector3 PositionFromMatrix(Matrix4x4 m)
    {
        return m.GetColumn(3);
    }

    static Vector3 ScaleFromMatrix(Matrix4x4 matrix)
    {
        Vector3 scale = new Vector3(
            matrix.GetColumn(0).magnitude,
            matrix.GetColumn(1).magnitude,
            matrix.GetColumn(2).magnitude
            );
        if (Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)).normalized != (Vector3)matrix.GetColumn(2).normalized)
        {
            scale.x *= -1;
        }
        return scale;
    }

    static Quaternion RotationFromMatrix(Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        normal = normal.normalized;
        Vector3 offsetPos = pos + normal * 0.0f;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }


    internal static class MyXRUtil
    {
        public static bool isPresent()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
