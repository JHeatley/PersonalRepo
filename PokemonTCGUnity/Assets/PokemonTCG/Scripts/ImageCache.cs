using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PokemonTCG
{
    /// <summary>Downloads card art once and hands the same texture to every card that needs it.</summary>
    public class ImageCache
    {
        readonly MonoBehaviour runner;
        readonly Dictionary<string, Texture2D> done = new Dictionary<string, Texture2D>();
        readonly Dictionary<string, List<Action<Texture2D>>> waiting = new Dictionary<string, List<Action<Texture2D>>>();
        readonly HashSet<string> failed = new HashSet<string>();

        public ImageCache(MonoBehaviour runner)
        {
            this.runner = runner;
        }

        /// <summary>Calls back with the texture, or with null if it can't be downloaded.</summary>
        public void Get(string url, Action<Texture2D> callback)
        {
            if (string.IsNullOrEmpty(url) || failed.Contains(url))
            {
                callback(null);
                return;
            }
            Texture2D tex;
            if (done.TryGetValue(url, out tex))
            {
                callback(tex);
                return;
            }
            List<Action<Texture2D>> list;
            if (waiting.TryGetValue(url, out list))
            {
                list.Add(callback);
                return;
            }
            waiting[url] = new List<Action<Texture2D>> { callback };
            runner.StartCoroutine(Download(url));
        }

        IEnumerator Download(string url)
        {
            using (var req = UnityWebRequestTexture.GetTexture(url))
            {
                req.timeout = 20;
                yield return req.SendWebRequest();
                Texture2D tex = null;
                if (string.IsNullOrEmpty(req.error))
                    tex = DownloadHandlerTexture.GetContent(req);
                if (tex == null)
                    failed.Add(url);
                else
                    done[url] = tex;
                var list = waiting[url];
                waiting.Remove(url);
                foreach (var cb in list)
                    cb(tex);
            }
        }
    }
}
