using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ListController : MonoBehaviour {

    const string boardsDirectory = "Testing";

    public List<string> names = new List<string>();
    public List<BoardSerializer.SerializedBoard> boards = new List<BoardSerializer.SerializedBoard>();

    [SerializeField]
    private GameObject ListTilePrefab = null;
    [SerializeField]
    private GameObject ListHolder = null;

    private void Start() {
        Refresh();
    }

    public void Refresh() {
        GatherBoards();
        GenerateTiles();
    }

    private void GatherBoards() {
        names.Clear();
        boards.Clear();
        foreach (string s in Directory.GetFiles(boardsDirectory, "*", SearchOption.TopDirectoryOnly)) {
            names.Add(Path.GetFileNameWithoutExtension(s.Substring(boardsDirectory.Length+1)));
            boards.Add(JsonUtility.FromJson<BoardSerializer.SerializedBoard>(File.OpenText(s).ReadToEnd()));
        }
    }

    private void GenerateTiles() {
        foreach (var g in GameObject.FindGameObjectsWithTag("ListTiles")) Destroy(g);
        // Generate new tiles.
        for (int i=0; i<boards.Count; i++) {
            var tile = Instantiate(ListTilePrefab, ListHolder.transform);
            tile.name = names[i];
            var rt = tile.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -rt.sizeDelta.y * (0.5f + i));
            ListItem li = tile.GetComponent<ListItem>();
            li.SetTitle(names[i]);
            li.SetBoard(boards[i]);
        }
    }

}
