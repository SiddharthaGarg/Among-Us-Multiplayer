using UnityEngine;
using UnityEngine.SceneManagement;

public class AU_CharacterCustomiser : MonoBehaviour
{
    [SerializeField] Color[] colors;
    public void SetAvatarColor(int colorIndex)
    {
        AU_PlayerMovement.localPlayer.SetColor(colors[colorIndex]);
    }
    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
