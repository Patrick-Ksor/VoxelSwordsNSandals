using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleScript4 : MonoBehaviour

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
    public GameObject tornado;
    public GameObject fireball;
    public GameObject fireballhit;
    public GameObject boneshield;

    public Transform player;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        player = GameObject.Find("Enemy(Clone)").transform;
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
        playerUnit.bp += 1;
        playerHud.SetBP(playerUnit.bp);

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
        enemyAnimator.Play("fighter_hit");
        audioManager.Play("EnemyHit");
        yield return new WaitForSeconds(.3f);
        bleed();

        playerAnimator.SetBool("isAttack", false);
        enemyHud.SetHP(enemyUnit.currentHP);

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
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage * 2);
        yield return new WaitForSeconds(1f);
        playerUnit.bp -= 2;
        playerHud.SetBP(playerUnit.bp);

        dialogueText.text = "BONE STORMMMMMM!";
        playerAnimator.SetBool("isSpell", true);
        yield return new WaitForSeconds(1f);
        audioManager.Play("PlayerSpell");
        yield return new WaitForSeconds(.1f);
        audioManager.Play("PlayerSpell");

        //move position to above the enemy battle station
        transform.position = new Vector3(1.219f, 2.41f, -10.919f);
        for (int i = 0; i < 20; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(-transform.up * 8f, ForceMode.Impulse);
        }

        enemyAnimator.Play("fighter_big_hit");
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

    IEnumerator PlayerSpell2()
    {
        yield return new WaitForSeconds(1f);
        playerUnit.bp -= 1;
        playerHud.SetBP(playerUnit.bp);
        if (enemyUnit.damage > 5)
        {
            enemyUnit.damage -= 5;
        }
        dialogueText.text = "BONE SHIELD!";
        playerAnimator.SetBool("isSpell", true);
        yield return new WaitForSeconds(1f);
        audioManager.Play("PlayerShieldSound");
        yield return new WaitForSeconds(.5f);
        for(int i = 0; i <= 3; i++){
            yield return new WaitForSeconds(.3f);
            audioManager.Play("PlayerSpell");
        }
        playerAnimator.SetBool("isSpell", false);
        enemyHud.SetHP(enemyUnit.currentHP);

        Rigidbody bd = Instantiate(boneshield, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        bd.AddForce(transform.up * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }


    IEnumerator EnemyTurn()
    {
        bool isDead = false;
        int randomNumber = Random.Range(1, 3);

        if (randomNumber ==1)
        {
            player.LookAt(transform);
            dialogueText.text = enemyUnit.unitName + ": HOOOOWAHHHHH!";
            enemyAnimator.SetBool("fireball", true);
            audioManager.Play("EnemyAttack");
            yield return new WaitForSeconds(.7f);
            audioManager.Play("EnemyFireBall");
            yield return new WaitForSeconds(.3f);
            enemyAnimator.SetBool("fireball", false);
            Rigidbody rb = Instantiate(fireball, player.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(-transform.forward * 7f, ForceMode.Impulse);
            rb.AddForce(transform.up * 3f, ForceMode.Impulse);

            isDead = playerUnit.TakeDamage(enemyUnit.damage-5);

            yield return new WaitForSeconds(.2f);
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            Rigidbody bd = Instantiate(fireballhit, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            bd.AddForce(transform.up * 5f, ForceMode.Impulse);

            yield return new WaitForSeconds(.5f);
            playerAnimator.SetBool("getHit", false);
            
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            
        }


        else
        {
            dialogueText.text = enemyUnit.unitName + ": RARARARWARWRAR!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.SetBool("kick", true);
            for(int i = 0; i<4; i++)
            { 
                audioManager.Play("EnemyAttack2");
                yield return new WaitForSeconds(.5f);
                audioManager.Play("EnemyTornado");
                Rigidbody nd = Instantiate(tornado, player.position, Quaternion.identity).GetComponent<Rigidbody>();

                nd.AddRelativeTorque(Vector3.up * 20f);

                nd.AddForce(-transform.forward * 7f, ForceMode.Impulse);
                nd.AddForce(transform.up * 3f, ForceMode.Impulse);

                yield return new WaitForSeconds(.2f);
                Rigidbody rb = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.up * 5f, ForceMode.Impulse);
                playerAnimator.SetBool("getHit", true);
                audioManager.Play("PlayerHit");
                isDead = playerUnit.TakeDamage(enemyUnit.damage/4);
                playerHud.SetHP(playerUnit.currentHP);
                yield return new WaitForSeconds(.5f);
                playerAnimator.SetBool("getHit", false);
            }
            
            enemyAnimator.SetBool("kick", false);
            yield return new WaitForSeconds(1f);
            
        }


        if (isDead)
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
            enemyAnimator.Play("fighter_death");
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
            switchScene();
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
    public void OnSpellButton2()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        else if (playerUnit.bp < 1)
        {
            dialogueText.text = "Not enough bone power";
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(PlayerSpell2());
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

}
