using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public enum tutorialStep 
    {
        Move,

        WallRun,

        Attack,
    }
    private tutorialStep currentStep;

    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [SerializeField] private Player player;
    [SerializeField] private PlayerMove playerMove;

    //触れる対象オブジェクト
    [SerializeField] private GameObject MoveGoal;
    [SerializeField] private GameObject AttackGoal;
    [SerializeField] private GameObject DethGoal;
    [SerializeField] private GameObject ClearGoal;

    private bool isGoalTouched = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStep = tutorialStep.Move;

        StartCoroutine(MoveTutorial());

        //ShowMoveTutorial();
    }

    // Update is called once per frame

    IEnumerator MoveTutorial()
    {
        //プレイヤーは最初操作禁止
        player.enabled = false;
        playerMove.canMove = false;

        //3秒待つ
        yield return new WaitForSeconds(3);

        //操作方法の説明表示
        tutorialUI.SetActive(true);
        tutorialText.text = "操作方法\n\n" + "WASD\n\n" + "ジャンプ\n\n" + "Space\n\n";

        //操作方法の説明を削除
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        //やってみよう！を表示
        tutorialUI.SetActive(true);
        tutorialText.text="やってみよう！";

        //3秒やってみよう!を表示
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        //プレイヤー操作可能
        player.enabled = true;
        playerMove.canMove = true;

        //一定数移動したら次のステップへ
        yield return new WaitUntil(()=>isGoalTouched);

        //次のステップへ
        currentStep = tutorialStep.Attack;

        Debug.Log("チュートリアル終了");

    }
    public void GoalTouched(GameObject goal)
    {
        if (goal == MoveGoal && currentStep == tutorialStep.Move)
        {
            isGoalTouched = true;
        }
    }

}
