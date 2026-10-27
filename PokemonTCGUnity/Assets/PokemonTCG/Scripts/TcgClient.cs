using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PokemonTCG
{
    /// <summary>Talks to the local Python game server (server.py).</summary>
    public class TcgClient
    {
        public string BaseUrl;

        public TcgClient(string baseUrl)
        {
            BaseUrl = baseUrl.TrimEnd('/');
        }

        public IEnumerator GetInfo(Action<InfoData> ok, Action<string> fail)
        {
            using (var req = UnityWebRequest.Get(BaseUrl + "/info"))
            {
                req.timeout = 10;
                yield return req.SendWebRequest();
                if (!string.IsNullOrEmpty(req.error))
                {
                    fail(Describe(req));
                    yield break;
                }
                ok(JsonUtility.FromJson<InfoData>(req.downloadHandler.text));
            }
        }

        public IEnumerator Post(string path, object body, Action<StateData> ok, Action<string> fail)
        {
            using (var req = new UnityWebRequest(BaseUrl + path, "POST"))
            {
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(body)));
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.timeout = 60;
                yield return req.SendWebRequest();
                string text = req.downloadHandler != null ? req.downloadHandler.text : null;
                StateData state = null;
                if (!string.IsNullOrEmpty(text))
                {
                    try { state = JsonUtility.FromJson<StateData>(text); }
                    catch (Exception e) { fail("Bad reply from server: " + e.Message); yield break; }
                }
                if (state != null && !string.IsNullOrEmpty(state.error))
                    fail(state.error);
                else if (!string.IsNullOrEmpty(req.error) || state == null)
                    fail(Describe(req));
                else
                    ok(state);
            }
        }

        string Describe(UnityWebRequest req)
        {
            if (req.responseCode == 0)
                return "Can't reach the game server at " + BaseUrl +
                       ".\nStart it first:  cd PokemonTCGBot  then  python server.py";
            return req.error + " (" + req.responseCode + ")";
        }
    }
}
