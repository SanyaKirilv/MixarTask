using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Dummiesman;

public class DynamicImage : MonoBehaviour
{
    public Texture2D Target;
    public string TargetName;
    public string TargetURL;
    public GameObject Model;
    public string ModelName;
    public string ModelURL;
    public Text log;
    public List<MoveController> MoveControllers;

    [SerializeField] private XRReferenceImageLibrary runtimeImageLibrary;
    
    private ARTrackedImageManager trackImageManager;

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        StartCoroutine(LoadFromWeb(TargetURL, "Target"));
        StartCoroutine(LoadFromWeb(ModelURL, "Model"));
    }

    private void CreateLibrary()
    {
        trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();
        trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);

        trackImageManager.enabled = true;

        trackImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        StartCoroutine(AddImageJob(Target));
    }

    private IEnumerator LoadFromWeb(string url, string type)
    {
        UnityWebRequest _request = type == "Target" ?
            UnityWebRequestTexture.GetTexture(url) :
            UnityWebRequest.Get(url);

        yield return _request.SendWebRequest();

        if (_request.result == UnityWebRequest.Result.ConnectionError)
            log.text += "- Connection error! Please retry later.\n";
        else
        {
            log.text += $"- {type} downloaded!\n";
            switch (type)
            {
                case "Target":
                    LoadTarget(_request);
                    break;
                case "Model":;
                    LoadModel(_request);
                    break;
            }
        }
    }

    private void LoadTarget(UnityWebRequest request)
    {
        Target = DownloadHandlerTexture.GetContent(request);
        log.text += $"- File {TargetName} loaded!\n";
    }

    private void LoadModel(UnityWebRequest request)
    {
        MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(request.downloadHandler.text));
        Model = new OBJLoader().Load(textStream);
        Model.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
        Model.SetActive(false);
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        foreach (var mesh in Model.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.material = mat;
        }
        log.text += $"- File {ModelName} loaded!\n";

        CreateLibrary();
    }

    private IEnumerator AddImageJob(Texture2D texture2D)
    {
        yield return null;

        while(texture2D == null)
        {
            yield return new WaitForSeconds(1f);
        }

        trackImageManager.requestedMaxNumberOfMovingImages = 1;
        trackImageManager.trackedImagePrefab = Model;

        Guid firstGuid = Guid.NewGuid();
        Guid secondGuid = Guid.NewGuid();

        ulong firstGuidHigh = BitConverter.ToUInt64(firstGuid.ToByteArray(), 0);
        ulong firstGuidLow = BitConverter.ToUInt64(firstGuid.ToByteArray(), 8);
        ulong secondGuidHigh = BitConverter.ToUInt64(secondGuid.ToByteArray(), 0);
        ulong secondGuidLow = BitConverter.ToUInt64(secondGuid.ToByteArray(), 8);


        XRReferenceImage newImage = new XRReferenceImage(
            new SerializableGuid(firstGuidHigh, firstGuidLow),
            new SerializableGuid(secondGuidHigh, secondGuidLow),
            new Vector2(0.1f, 0.1f),
            Guid.NewGuid().ToString(),
            texture2D
        );

        try
        {
            log.text += "-" + newImage.ToString() + "\n";

            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            if (mutableRuntimeReferenceImageLibrary == null)
            {
                log.text += "- Reference library is not mutable.\n";
                yield break;
            }

            var jobState = mutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(texture2D, Guid.NewGuid().ToString(), 0.1f);
            while (!jobState.jobHandle.IsCompleted)
            {
                yield break;
            }

            jobState.jobHandle.Complete();

            if (jobState.status != AddReferenceImageJobStatus.Success)
            {
                log.text += "- Image addition job did not complete successfully: " + jobState.status + "\n";
            }
            else
            {
                log.text += "- Image added successfully.\n";
            }
        }
        catch(Exception e)
        {
            log.text += e.ToString();
        }
    }

    private void OnDisable() => trackImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Model?.SetActive(true);
            ChangeSpeed();
            UpdateARImage(trackedImage);
            log.text += $"- Find {trackedImage}\n";
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            Model?.SetActive(true);
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Model?.SetActive(false);
            UpdateARImage(trackedImage);
            log.text += $"- Lost {trackedImage}\n";
        }
        
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        AssignGameObject(trackedImage.transform.position);
    }

    private void AssignGameObject(Vector3 newPosition)
    {
        if (Model != null)
            Model.transform.localPosition = newPosition;
    }

    private void ChangeSpeed()
    {
        foreach (MoveController moveController in MoveControllers)
        {
            moveController.speed = 5f;
        }
    }
}
