using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;

public class RestAPI : MonoBehaviour
{
    [SerializeField] private ImageProcessor _processor;

    string roomUrl = "https://gamelevelgenerator.onrender.com/predict";
    string dungeonUrl = "https://dungeongenerator.onrender.com/predict";

    string roomServiceId = "srv-coc74q0l6cac73er73sg";
    string dungeonServiceId = "srv-coe58eol6cac73bu4tn0";

    string token = "rnd_aCszpQWV9nfEUWBYvpQ7EzHbiAcY";

    [SerializeField] private GameObject _loadingText;
    [SerializeField] private GameObject _controlsText;

    private void Start()
    {
        _loadingText.SetActive(true);
        StartCoroutine(SendPostForDungeon());
        //StartCoroutine(SendPostForRoom());
    }

    IEnumerator SendPostForDungeon()
    {
        int roomNum = 0;
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(dungeonUrl, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                JSONArray jsonArray = JSON.Parse(jsonResponse).AsArray;
                int[][] resultArray = new int[jsonArray.Count][];

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    JSONArray row = jsonArray[i].AsArray;
                    resultArray[i] = new int[row.Count];

                    for (int j = 0; j < row.Count; j++)
                    {
                        resultArray[i][j] = row[j];
                    }
                }
                Debug.Log(jsonResponse);
                roomNum = _processor.ProcessDungeon(resultArray);
            }
        }
        for (int i = 0; i < roomNum; i++)
        {
            StartCoroutine(SendPostForRoom());
        }

        //restart service
        StartCoroutine(SendRestartRequest(roomServiceId));
        StartCoroutine(SendRestartRequest(dungeonServiceId));
    }

    IEnumerator SendPostForRoom()
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(roomUrl, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;

                JSONArray jsonArray = JSON.Parse(jsonResponse).AsArray;
                int[][] resultArray = new int[jsonArray.Count][];

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    JSONArray row = jsonArray[i].AsArray;
                    resultArray[i] = new int[row.Count];

                    for (int j = 0; j < row.Count; j++)
                    {
                        resultArray[i][j] = (int)(row[j].AsFloat * 10f);
                    }
                }

                _processor.ProcessRoom(resultArray);
            }
        }
        _loadingText.SetActive(false);
        _controlsText.SetActive(true);
    }

    IEnumerator SendRestartRequest(string ID)
    {
        string url = $"https://api.render.com/v1/services/{ID}/restart";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Service restarted successfully.");
        }
        else
        {
            Debug.LogError("Failed to restart service: " + request.error);
        }
    }
}
