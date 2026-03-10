using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleScript : MonoBehaviour
    
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
    int alreadyHealed = 1;
    public GameObject projectile;
    public GameObject slash;
    public GameObject blood;
    public GameObject powerup;
    public GameObject slashproj;

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

        dialogueText.text = "A " + enemyUnit.unitName+" Stops You";
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
        rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        rb.AddForce(transform.up * 3f, ForceMode.Impulse);

        enemyAnimator.SetBool("getHit", true);
        //blood effects
        audioManager.Play("EnemyHit");
        audioManager.Play("EnemyMetal");

        yield return new WaitForSeconds(.3f);
        bleed();
        
        playerAnimator.SetBool("isAttack", false);
        enemyHud.SetHP(enemyUnit.currentHP);
        enemyAnimator.SetBool("getHit", false);
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
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage *2);
        yield return new WaitForSeconds(1f);
        
        dialogueText.text = "BONE STORMMMMMM!";
        playerAnimator.SetBool("isSpell", true);
        yield return new WaitForSeconds(1f);
        audioManager.Play("PlayerSpell");

        //move position to above the enemy battle station
        transform.position = new Vector3(1.219f, 2.41f, -10.919f);
        for(int i =0; i< 10; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(-transform.up * 8f, ForceMode.Impulse);
        }

        enemyAnimator.SetBool("isHit", true);
        bleed();
        yield return new WaitForSeconds(.2f);
        audioManager.Play("EnemyHit");
        audioManager.Play("EnemyMetal");
        bleed();
        yield return new WaitForSeconds(2f);
        playerAnimator.SetBool("isSpell", false);
        enemyHud.SetHP(enemyUnit.currentHP);

        enemyAnimator.SetBool("isHit", false);

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
        
        if (enemyUnit.currentHP <= 40 && alreadyHealed!=0)
        {
            dialogueText.text = enemyUnit.unitName + " eats an apple";
            audioManager.Play("EnemyEat");
            yield return new WaitForSeconds(1f);
            dialogueText.text = "He heals and increased his damage!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.Play("knight_powerup");
            audioManager.Play("PowerUp");
            //yield return new WaitForSeconds(1f);
            enemyUnit.currentHP += 50;
            enemyUnit.damage *= 2;
            enemyHud.SetHP(enemyUnit.currentHP);

            alreadyHealed = 0;
            yield return new WaitForSeconds(.5f);
            Rigidbody bd = Instantiate(powerup, new Vector3(player.position.x, .01f, player.position.z), Quaternion.identity).GetComponent<Rigidbody>();
            bd.AddForce(transform.up * 5f, ForceMode.Impulse);
        }
        else
        {
            isDead = playerUnit.TakeDamage(enemyUnit.damage);
            dialogueText.text = enemyUnit.unitName + " attacks";
            yield return new WaitForSeconds(1f);
            enemyAnimator.Play("Sword_strike");
            
            yield return new WaitForSeconds(1f);
            audioManager.Play("Slash");
            slashAttack();

            yield return new WaitForSeconds(.2f);
            Rigidbody rb = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);

            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            playerAnimator.SetBool("getHit", false);
            
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
        if(state == BattleState.WON)
        {
            playerUnit.xp += 100;
            playerHud.SetXP(playerUnit.xp);
            //audioManager.Stop("Theme");
            audioManager.Play("PlayerWin");
            audioManager.Play("EnemyDeath");
            enemyAnimator.Play("knight_kneel");
            dialogueText.text = "YOU WON!";

            if (playerUnit.xp >= playerUnit.xpToLevel)
            {
                StartCoroutine(levelUp());
            }
            switchScene();
            
        }
        else if(state == BattleState.LOST)
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
    public void slashAttack()
    {
        //transform above enemy
        player.position = new Vector3(1.219f, .3f, -10.919f);

        Rigidbody slashattack = Instantiate(slashproj, player.position, Quaternion.identity).GetComponent<Rigidbody>();
        var rotationVector = transform.rotation.eulerAngles;
        rotationVector.y = 180;  //this number is the degree of rotation around Z Axis
        slashattack.rotation = Quaternion.Euler(rotationVector);

        slashattack.AddForce(-transform.forward * 7f, ForceMode.Impulse);
        slashattack.AddForce(transform.up * 2f, ForceMode.Impulse);

        //reset position
        player.position = new Vector3(1.219f, -0.41f, -10.919f);
    }

}
