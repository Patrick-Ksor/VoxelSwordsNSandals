using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleScript3 : MonoBehaviour

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
    public GameObject darkSlash;
    public GameObject powerUp;
    public GameObject sword;
    public GameObject darkSword;

    int darkMode = 0;


    public Transform player;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        player = GameObject.Find("Enemy(Clone)").transform;
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
        enemyAnimator.Play("paladin_hit");
        audioManager.Play("EnemyHit");

        yield return new WaitForSeconds(.3f);
        bleed();
        yield return new WaitForSeconds(.2f);
        audioManager.Play("EnemyFall");

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
        for (int i = 0; i < 20; i++)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(-transform.up * 8f, ForceMode.Impulse);
        }

        enemyAnimator.Play("paladin_hit");
        bleed();
        yield return new WaitForSeconds(.2f);
        bleed();
        audioManager.Play("EnemyHit");
        yield return new WaitForSeconds(.2f);
        audioManager.Play("EnemyFall");
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

        if (enemyUnit.currentHP <= 100 && darkMode!=1)
        {
            dialogueText.text = enemyUnit.unitName + ": All children of God shalled be healed!";
            enemyAnimator.SetBool("spell", true);
            playerUnit.currentHP += 50;
            playerHud.SetHP(playerUnit.currentHP);
            audioManager.Play("PlayerHeal");
            yield return new WaitForSeconds(3f);
            enemyAnimator.SetBool("spell", false);
            dialogueText.text = enemyUnit.unitName + ": What! This can't be!";
            yield return new WaitForSeconds(2f);
            audioManager.Play("Drums");
            dialogueText.text = enemyUnit.unitName + ": NOOOO!";
            enemyAnimator.Play("paladin_powerup");
            audioManager.Play("EnemyScream");
            audioManager.Play("EnemyPowerUp");
            Rigidbody bd = Instantiate(powerUp, player.position, Quaternion.identity).GetComponent<Rigidbody>();
            bd.AddForce(transform.up * 5f, ForceMode.Impulse);
            yield return new WaitForSeconds(2f);
            enemyUnit.damage *= 3;

            darkMode = 1;
            yield return new WaitForSeconds(1f);
            
        }


        else if (darkMode == 0)
        {
            dialogueText.text = enemyUnit.unitName + ": Be judged by the light!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.SetBool("cast", true);
            audioManager.Play("EnemyMagic");
            yield return new WaitForSeconds(2.7f);
            swordAttack(0);
            audioManager.Play("EnemySwordSound");
            yield return new WaitForSeconds(.5f);
            audioManager.Play("EnemySwordDrop");
            yield return new WaitForSeconds(.3f);
            audioManager.Play("EnemySwordExplosion");
            Rigidbody rb = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);
            enemyAnimator.SetBool("cast", false);
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            
            playerAnimator.SetBool("getHit", false);
        }
        else if (darkMode == 1)
        {
            dialogueText.text = enemyUnit.unitName + ": Darkness consume you!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.SetBool("cast", true);
            audioManager.Play("EnemyMagic");
            yield return new WaitForSeconds(2.7f);
            swordAttack(1);
            audioManager.Play("EnemySwordSound");
            yield return new WaitForSeconds(.5f);
            audioManager.Play("EnemySwordDrop");
            yield return new WaitForSeconds(.3f);
            audioManager.Play("EnemySwordExplosion");
            Rigidbody rb = Instantiate(darkSlash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);
            enemyAnimator.SetBool("cast", false);
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            
            playerAnimator.SetBool("getHit", false);
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
            enemyAnimator.Play("paladin_death");
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
        yield return new WaitForSeconds(1f);
        dialogueText.text = "You Got bone shield!";
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
    public void swordAttack(int flag)
    {
        //transform above enemy
        player.position = new Vector3(-0.343f, 1.412f, -10.919f);
        Rigidbody swordattack;
        if (flag == 0)
        {
            swordattack = Instantiate(sword, player.position, Quaternion.identity).GetComponent<Rigidbody>();

        }
        else
        {
            swordattack = Instantiate(darkSword, player.position, Quaternion.identity).GetComponent<Rigidbody>();
        }
        var rotationVector = transform.rotation.eulerAngles;
        rotationVector.z = 180;  //this number is the degree of rotation around Z Axis
        swordattack.rotation = Quaternion.Euler(rotationVector);

        //swordattack.AddForce(-transform.forward * 9f, ForceMode.Impulse);
        swordattack.AddForce(-transform.up * 2f, ForceMode.Impulse);

        //reset position
        player.position = new Vector3(1.219f, -0.41f, -10.919f);
    }
}
