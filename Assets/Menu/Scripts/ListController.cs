using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ListController : MonoBehaviour {

    public static ListController instance;
    public bool clicked = false;

    const string boardsDirectory = "Testing";

    public List<string> names = new List<string>();
    public List<BoardSerializer.SerializedBoard> boards = new List<BoardSerializer.SerializedBoard>();
    private List<ListItem> items = new List<ListItem>();

    [SerializeField]
    private GameObject ListTilePrefab = null;
    [SerializeField]
    private GameObject ListHolder = null;

    public Color Background;
    public Color MainText;
    public Color SecondaryText;
    public Color BackgroundSelected;
    public Color MainTextSelected;
    public Color SecondaryTextSelected;

    private void Start() {
        instance = this;
        Refresh();
    }

    public void Refresh() {
        GatherBoards();
        GenerateTiles();
        clicked = false;
    }

    private void GatherBoards() {
        names.Clear();
        boards.Clear();
        items.Clear();
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
            li.SetColors(Background, MainText, SecondaryText);
            items.Add(li);
        }
    }

    public void SetSelected(ListItem li) {
        foreach (var x in items) {
            x.SetColors(Background, MainText, SecondaryText);
        }
        li.SetColors(BackgroundSelected, MainTextSelected, SecondaryTextSelected);
    }

}
