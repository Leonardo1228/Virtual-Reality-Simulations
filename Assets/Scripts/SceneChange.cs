using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void SceneCharge(int Nescene){
        SceneManager.LoadScene(Nescene);
     }
}
