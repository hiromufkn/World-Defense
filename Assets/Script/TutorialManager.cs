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


    [SerializeField] private PlayerMove playerMove;
    //[SerializeField] private Player player;

    //触れる対象オブジェクト
    [SerializeField] private GameObject MoveGoal;
    [SerializeField] private GameObject WallRunGoal;
    [SerializeField] private GameObject AttackGoal;

    private bool isGoalTouched = false;
    private bool isEnemyDefeated = false;

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
        tutorialText.text="やってみよう!";

        //3秒やってみよう!を表示
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        //プレイヤー操作可能
        playerMove.canMove = true;

        //一定数移動したら次のステップへ
        yield return new WaitUntil(()=>isGoalTouched);

        //次のステップへ
        currentStep = tutorialStep.WallRun;

        StartCoroutine(WallRunTutorial());
    }

    IEnumerator WallRunTutorial()
    {
        //壁走りの説明表示
        tutorialUI.SetActive(true);
        tutorialText.text = "壁走り\n\n" + "壁に近づき走る\n\n";
        //player.speed = 0;

        //操作方法の説明を削除
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        //やってみよう！を表示
        tutorialUI.SetActive(true);
        tutorialText.text = "やってみよう!";

        //3秒後やってみよう!を削除
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        playerMove.canMove = true;

        //WallRunGoalに触れるまで待つ
        isGoalTouched = false;
        yield return new WaitUntil(() => isGoalTouched);

        playerMove.canMove = false;

        currentStep = tutorialStep.Attack;

        StartCoroutine(AttackTutorial());
    }

    IEnumerator AttackTutorial()
    {
        //敵への攻撃説明
        tutorialUI.SetActive(true);
        tutorialText.text = "攻撃方法\n\n" + "CTRLまたはマウスクリック\n\n";
        //player.speed = 0;

        //説明削除
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        //やってみよう!表示
        tutorialUI.SetActive(true);
        tutorialText.text = "やってみよう!";

        //3秒後やってみよう!削除
        yield return new WaitForSeconds(3);
        tutorialUI.SetActive(false);

        playerMove.canMove = true;

        //敵を倒すまで待つ
        yield return new WaitUntil(() => isEnemyDefeated);

        playerMove.canMove = false;

        //チュートリアル終了メッセージ
        tutorialUI.SetActive(true);
        tutorialText.text = "チュートリアル終了!";

        //3秒待ち削除
        yield return new WaitForSeconds(3);

        tutorialUI.SetActive(false);

        //ゲームシーンへ移動
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageScene");
    }

    public void GoalTouched(GameObject goal)
    {
        if (goal == MoveGoal && currentStep == tutorialStep.Move)
        {
            //プレイヤー操作無効
            playerMove.canMove = false;

            isGoalTouched = true;
        }

        if(goal==WallRunGoal && currentStep == tutorialStep.WallRun)
        {
            playerMove.canMove = false;

            isGoalTouched = true;
        }
    }

    public void EnemyDefeated()
    {
        if(currentStep==tutorialStep.Attack)
        {
            isEnemyDefeated = true;
        }
    }
}
