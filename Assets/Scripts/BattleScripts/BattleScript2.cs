using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleScript2 : MonoBehaviour

{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public BattleState state;
    public Text dialogueText;

    Animator playerAnimator;
    Animator enemyAnimator;

    public BattleScriptHUD playerHud;
    public BattleScriptHUD enemyHud;

    public GameObject projectile;
    public GameObject slash;
    public GameObject blood;
    public GameObject powerup;
    public GameObject axe;
    public GameObject redSlash;


    public Transform player;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        player = GameObject.Find("Enemy1(Clone)").transform;
        player.LookAt(transform);
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGo.GetComponent<Unit>();
        playerAnimator = playerGo.GetComponent<Animator>();

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();
        enemyAnimator = enemyGo.GetComponent<Animator>();


        dialogueText.text = "A " + enemyUnit.unitName + " Stops You";
        playerHud.SetHUD(playerUnit);
        enemyHud.SetHUD(enemyUnit);

        yield return new WaitForSeconds(3f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        yield return new WaitForSeconds(1f);

        if(playerUnit.bp < 2){
            playerUnit.bp += 1;
            playerHud.SetBP(playerUnit.bp);
        }

        dialogueText.text = "BONED";
        transform.LookAt(player);

        playerAnimator.SetBool("isAttack", true);

        yield return new WaitForSeconds(1);
        audioManager.Play("PlayerAttack");
        //bone projectiles
        Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

        rb.AddRelativeTorque(Vector3.up * 20f);

        rb.AddForce(transform.forward * 7f, ForceMode.Impulse);
        rb.AddForce(transform.up * 3f, ForceMode.Impulse);


        //blood effects
        enemyAnimator.Play("hit");
        audioManager.Play("EnemyHit");

        yield return new WaitForSeconds(.3f);
        bleed();
        playerAnimator.SetBool("isAttack", false);
        enemyHud.SetHP(enemyUnit.currentHP);
        yield return new WaitForSeconds(.6f);
        audioManager.Play("EnemyFall");
 
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerSpell()
    {
        playerUnit.bp -= 2;
        playerHud.SetBP(playerUnit.bp);
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage * 2);
        yield return new WaitForSeconds(1f);

        dialogueText.text = "BONE STORMMMMMM!";
        playerAnimator.SetBool("isSpell", true);
        yield return new WaitForSeconds(1f);
        audioManager.Play("PlayerSpell");

        //move position to above the enemy battle station
        transform.position = new Vector3(1.219f, 2.41f, -10.919f);
        for (int i = 0; i < 10; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(-transform.up * 8f, ForceMode.Impulse);
        }

        enemyAnimator.Play("side_hit");
        bleed();
        yield return new WaitForSeconds(.2f);
        bleed();
        audioManager.Play("EnemyHit");
        yield return new WaitForSeconds(2f);
        playerAnimator.SetBool("isSpell", false);
        enemyHud.SetHP(enemyUnit.currentHP);

        //reset position
        transform.position = new Vector3(-1.143f, -0.412f, -10.919f);



        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }


    IEnumerator EnemyTurn()
    {
        bool isDead = false;


        int randomNumber = Random.Range(1, 4);

        if (enemyUnit.currentHP <= 20)
        {
            dialogueText.text = enemyUnit.unitName + " uses AXE EXPLOSION!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.Play("barb_axe_explosion");
            audioManager.Play("EnemyGrunt2");
            yield return new WaitForSeconds(1f);
            audioManager.Play("EnemyAttack2");
            axeExplosion();
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            isDead = playerUnit.TakeDamage(playerUnit.currentHP - 1);
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            playerAnimator.SetBool("getHit", false);
        }
        else if (randomNumber == 1)
        {
            dialogueText.text = enemyUnit.unitName + " unleashes a flurry of attacks";
            yield return new WaitForSeconds(1f);
            enemyAnimator.SetBool("attack", true);
            audioManager.Play("EnemyGrunt");
            yield return new WaitForSeconds(2.5f);
            redSlashAttack();
            audioManager.Play("EnemyAttack");
            yield return new WaitForSeconds(.3f);
            Rigidbody rb = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);
            enemyAnimator.SetBool("attack", false);
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            playerAnimator.SetBool("getHit", false);
        }
        else if (randomNumber == 2)
        {
            dialogueText.text = enemyUnit.unitName + " stabs himself for more power";
            yield return new WaitForSeconds(1f);
            enemyAnimator.Play("barb_powerup");
            audioManager.Play("EnemyBleed");
            bleed();
            yield return new WaitForSeconds(1.7f);
            enemyUnit.TakeDamage(30);
            enemyUnit.damage += 10;
            enemyHud.SetHP(enemyUnit.currentHP);
            Rigidbody bd = Instantiate(powerup, player.position, Quaternion.identity).GetComponent<Rigidbody>();
            bd.AddForce(transform.up * 5f, ForceMode.Impulse);
            enemyAnimator.Play("barb_roar");
            audioManager.Play("EnemyPowerUp");
            audioManager.Play("EnemyRoar");
            yield return new WaitForSeconds(1f);
        }
        else if (randomNumber == 3)
        {
            dialogueText.text = enemyUnit.unitName + " loafs around";
            enemyAnimator.Play("laugh");
            audioManager.Play("EnemyLoaf");
            yield return new WaitForSeconds(3f);
        }

        if (enemyUnit.currentHP <= 0)
        {
            state = BattleState.WON;
            EndBattle();
        }

        else if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            playerUnit.xp += enemyUnit.xp;
            playerHud.SetXP(playerUnit.xp);
            audioManager.Play("PlayerWin");
            audioManager.Play("EnemyDeath");
            enemyAnimator.Play("barb_die");
            dialogueText.text = "YOU WON!";

            if (playerUnit.xp >= playerUnit.xpToLevel)
            {
                StartCoroutine(levelUp());
            }
            switchScene();
        }
        else if (state == BattleState.LOST)
        {
            
            audioManager.Play("PlayerLose");
            dialogueText.text = "YOU SUCK!";
            playerAnimator.SetBool("isDead", true);
        }
    }
    void PlayerTurn()
    {
        dialogueText.text = "Choose an Action!";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        state = BattleState.ENEMYTURN;
        StartCoroutine(PlayerAttack());
    }
    public void OnSpellButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        else if (playerUnit.bp < 2)
        {
            dialogueText.text = "Not enough bone power";
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(PlayerSpell());
        }
        
    }

    public void bleed()
    {
        //blood effects
        Rigidbody bd = Instantiate(blood, player.position, Quaternion.identity).GetComponent<Rigidbody>();
        bd.AddForce(transform.up * 5f, ForceMode.Impulse);
    }


    IEnumerator levelUp()
    {
        yield return new WaitForSeconds(2f);
        dialogueText.text = "You Leveled Up!";
        playerHud.SetXP(playerUnit.xp - playerUnit.xpToLevel);
        playerUnit.xpToLevel += 50;
        playerUnit.unitLevel += 1;
        playerHud.SetLvl(playerUnit.unitLevel);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator switchScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void axeExplosion()
    {

        Rigidbody rb = Instantiate(axe, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

        rb.AddForce(transform.up * 5f, ForceMode.Impulse);

    }

    public void redSlashAttack()
    {
        //transform above enemy
        player.position = new Vector3(1.219f, -.1f, -10.919f);

        Rigidbody slashattack = Instantiate(redSlash, player.position, Quaternion.identity).GetComponent<Rigidbody>();
        var rotationVector = transform.rotation.eulerAngles;
        rotationVector.y = 180;  //this number is the degree of rotation around Z Axis
        slashattack.rotation = Quaternion.Euler(rotationVector);

        slashattack.AddForce(-transform.forward * 9f, ForceMode.Impulse);
        slashattack.AddForce(transform.up * 2f, ForceMode.Impulse);

        //reset position
        player.position = new Vector3(1.219f, -0.41f, -10.919f);
    }
}
