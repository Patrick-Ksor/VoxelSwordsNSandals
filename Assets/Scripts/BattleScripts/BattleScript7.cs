using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleScript7 : MonoBehaviour

{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject partnerPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    public Transform partnerBattleStation;

    Unit playerUnit;
    Unit enemyUnit;
    Unit partnerUnit;

    public BattleState state;
    public Text dialogueText;

    Animator playerAnimator;
    Animator enemyAnimator;
    Animator partnerAnimator;

    public BattleScriptHUD playerHud;
    public BattleScriptHUD enemyHud;

    public GameObject projectile;
    public GameObject slash;
    public GameObject blood;
    public GameObject shuriken;
    public GameObject fireball;
    public GameObject fireballhit;
    public GameObject boneshield;
    public GameObject healEffect;

    public Transform player;
    public Transform partner;
    public Transform projectileShooter;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        player = GameObject.Find("Enemy(Clone)").transform;
        partner = GameObject.Find("Mango(Clone)").transform;
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGo.GetComponent<Unit>();
        playerAnimator = playerGo.GetComponent<Animator>();

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();
        enemyAnimator = enemyGo.GetComponent<Animator>();

        GameObject partnerGo = Instantiate(partnerPrefab, partnerBattleStation);
        partnerUnit = partnerGo.GetComponent<Unit>();
        partnerAnimator = partnerGo.GetComponent<Animator>();


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
        enemyAnimator.Play("ninja_hit");
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

        enemyAnimator.Play("ninja_hit");
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
        audioManager.Play("PlayerShieldEffect");
        yield return new WaitForSeconds(.5f);
        for(int i = 0; i <= 3; i++){
            yield return new WaitForSeconds(.3f);
            audioManager.Play("PlayerSpell");
        }
        playerAnimator.SetBool("isSpell", false);
        enemyHud.SetHP(enemyUnit.currentHP);

        Rigidbody bd = Instantiate(boneshield, new Vector3(transform.position.x, .5f, transform.position.z), Quaternion.identity).GetComponent<Rigidbody>();
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
            dialogueText.text = enemyUnit.unitName + " throws Shuriken!";
            enemyAnimator.Play("ninja_attack");   
            audioManager.Play("EnemyAttack");
            yield return new WaitForSeconds(.7f);
            audioManager.Play("EnemyShurikenSound");
            yield return new WaitForSeconds(.3f);
            for(int i = 1; i<4; i++){
                yield return new WaitForSeconds(.07f);
                Rigidbody rb = Instantiate(shuriken, projectileShooter.position, projectileShooter.rotation).GetComponent<Rigidbody>();
                rb.AddForce(-transform.forward * 10f, ForceMode.Impulse);
                rb.AddForce(transform.up * 1.5f, ForceMode.Impulse);
            }

            isDead = playerUnit.TakeDamage(enemyUnit.damage+5);
            yield return new WaitForSeconds(.1f);
            playerAnimator.SetBool("getHit", true);
            audioManager.Play("PlayerHit");
            Rigidbody bd = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            bd.AddForce(transform.up * 5f, ForceMode.Impulse);

            yield return new WaitForSeconds(.5f);
            playerAnimator.SetBool("getHit", false);
            
            playerHud.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);
            //reset projectile shooter
            
        }


        else
        {
            projectileShooter.position = new Vector3(projectileShooter.position.x, projectileShooter.position.y + .5f, projectileShooter.position.z);
            dialogueText.text = enemyUnit.unitName + " uses Shuriken Storm!";
            yield return new WaitForSeconds(1f);
            enemyAnimator.Play("ninja_spell");
            audioManager.Play("EnemyAttack2");
            yield return new WaitForSeconds(.5f);
            audioManager.Play("EnemyStorm");
            yield return new WaitForSeconds(.5f);
            for(int i = 1; i<20; i++)
            { 
                audioManager.Play("EnemyShurikenSound");
                Rigidbody nd = Instantiate(shuriken, projectileShooter.position, projectileShooter.rotation).GetComponent<Rigidbody>();
                nd.AddForce(-transform.forward * 18f, ForceMode.Impulse);
                nd.AddForce(-transform.up * 5f, ForceMode.Impulse);
                yield return new WaitForSeconds(.05f);
                Rigidbody rb = Instantiate(slash, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.up * 5f, ForceMode.Impulse);
                playerAnimator.SetBool("getHit", true);
                audioManager.Play("PlayerHit");
                isDead = playerUnit.TakeDamage(enemyUnit.damage/5);
                playerHud.SetHP(playerUnit.currentHP);
                yield return new WaitForSeconds(.05f);
                playerAnimator.SetBool("getHit", false);
            }
            
            yield return new WaitForSeconds(1f);
            projectileShooter.position = new Vector3(projectileShooter.position.x, -0.123f, projectileShooter.position.z);
        }


        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            //Partner's turn I would add another state but too lazy to change that script
            randomNumber = Random.Range(1, 3);
            if (randomNumber == 1)
            {
                partner.LookAt(player.transform.position);
                dialogueText.text = partnerUnit.unitName + " casts fireball!";

                partnerAnimator.Play("mango_spell");   
                audioManager.Play("PartnerSpell");
                yield return new WaitForSeconds(.7f);
                audioManager.Play("PartnerFireball");
                yield return new WaitForSeconds(.3f);
 
                Rigidbody rb = Instantiate(fireball, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 7f, ForceMode.Impulse);
                rb.AddForce(transform.up * 3f, ForceMode.Impulse);
                isDead = enemyUnit.TakeDamage(enemyUnit.damage-5);
                yield return new WaitForSeconds(.2f);
                enemyAnimator.Play("ninja_hit");
                audioManager.Play("EnemyHit");
                Rigidbody bd = Instantiate(fireballhit, player.position, Quaternion.identity).GetComponent<Rigidbody>();
                bd.AddForce(transform.up * 5f, ForceMode.Impulse);
                yield return new WaitForSeconds(.5f);
                isDead = enemyUnit.TakeDamage(partnerUnit.damage);
                enemyHud.SetHP(enemyUnit.currentHP);
                yield return new WaitForSeconds(2f);
            }
            else
            {
                int heal = 20;
                //heal tops him off
                if(playerUnit.maxHP - playerUnit.currentHP < 20)
                {
                    heal = playerUnit.maxHP- playerUnit.currentHP;
                }
                dialogueText.text = partnerUnit.unitName + " heals you.";
                yield return new WaitForSeconds(1f);
                audioManager.Play("PartnerSpell2");
                partnerAnimator.Play("mango_heal");
                yield return new WaitForSeconds(1f);
                //change position above the enemy
 
                Rigidbody rb = Instantiate(healEffect, new Vector3(transform.position.x, .01f, transform.position.z), Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.up * 5f, ForceMode.Impulse);
                audioManager.Play("PartnerHeal");
                playerUnit.currentHP += heal;
                playerHud.SetHP(playerUnit.currentHP);
                yield return new WaitForSeconds(2f);
            
            }

            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
            
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
            enemyAnimator.Play("ninja_death");
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
        Rigidbody bd = Instantiate(blood, new Vector3(player.position.x, .1f, player.position.z), Quaternion.identity).GetComponent<Rigidbody>();
        bd.AddForce(transform.up * 1f, ForceMode.Impulse);
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

    IEnumerator wait1Seconds(){
        yield return new WaitForSeconds(1f);
    }

}
