using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ProtectWindow : MonoBehaviour
{
    private string apiUrl = "https://worldtimeapi.org/api/timezone/Asia/Almaty";

    private void Awake()
    {
        Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        Debug.Log("Start");
        StartCoroutine(GetDataFromApi());
    }

    IEnumerator GetDataFromApi()
    {
        Debug.Log("Start coroutine");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка: " + webRequest.error);
                Application.Quit();
            }
            else
            {
                // Получаем ответ в виде JSON
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log(jsonResponse);

                // Обрабатываем полученный JSON
                ProcessJsonResponse(jsonResponse);
            }
        }
    }

    void ProcessJsonResponse(string json)
    {
        TimeApiResponse data = JsonUtility.FromJson<TimeApiResponse>(json);

        DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(data.datetime);

        Debug.Log($"Текущее время Алматы: {dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss")}");

        DateTimeOffset targetDate = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(6));

        if (dateTimeOffset > targetDate)
        {
            Debug.Log("Время вышло!");
            Application.Quit();
        }
        else
        {
            Debug.Log($"Текущее время: {dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss")}");
            Debug.Log($"Время еще не вышло. Целевая дата: {targetDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            SceneManager.LoadScene(1);

        }
    }
}

[System.Serializable]
public class TimeApiResponse
{
    public string datetime;
}
