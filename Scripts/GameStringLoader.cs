using UnityEngine;

public class GameStringLoader : MonoBehaviour
{
    public static GameStrings LoadStrings(GameLanguage lang)
    {
        var file = "strings-en-us";
        GameStrings gameStrings = null;

        switch(lang)
        {
            case GameLanguage.English:
                Debug.Log("Language: English");
                file = "strings-en-us";
                break;
            case GameLanguage.Spanish:
                Debug.Log("Lengua: Español");
                file = "strings-es-es";
                break;
        }

        TextAsset textAsset = Resources.Load<TextAsset>(file);

        Debug.Assert(textAsset != null, $"Unable to load file: {file}");
        if (textAsset != null)
            gameStrings = JsonUtility.FromJson<GameStrings>(textAsset.text);

        return gameStrings;
    }
}

public enum GameLanguage
{
    English,
    Spanish
}