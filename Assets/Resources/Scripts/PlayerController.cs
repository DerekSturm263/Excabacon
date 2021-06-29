using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public Controls controls;
    public LayerMask ground;

    public PlayerType playerClass;
    public AbilityType abilityClass;
    public WeaponType weaponClass;

    private AlterableStats hpMana;

    public ItemType currentItem;

    private float currentSpeed;

    private Rigidbody2D rb2D;
    private Animator anim;

    private GameObject weapon;
    private GameObject weaponPivot;

    [HideInInspector] public int playerNum;

    private UnityEngine.UI.Image playerIcon;
    private UnityEngine.UI.Image healthBar;
    private UnityEngine.UI.Image manaBar;

    public GameObject spellBolt;
    public GameObject arrow;

    private void Awake()
    {
        controls = new Controls();

        controls.Player.Aiming.performed += ctx => Aim(ctx.ReadValue<Vector2>().normalized);
        controls.Player.Run.performed += ctx => Run(ctx.ReadValue<float>() == 1f);
        controls.Player.Jump.performed += _ => Jump();

        controls.Player.UseWeapon.performed += _ => StartUseWeapon();
        controls.Player.UseWeapon.canceled += _ => EndUseWeapon();
        controls.Player.UseAbility.performed += _ => UseAbility();
        controls.Player.UseItem.performed += _ => UseItem();

        playerClass = PlayerTypes.RobinHog;
        weaponClass = WeaponTypes.Bow;
        abilityClass = AbilityTypes.SuperShot;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        weapon = GetComponentInChildren<Weapon>().gameObject;
        weaponPivot = weapon.transform.parent.gameObject;

        currentSpeed = playerClass.walkSpeed;
        hpMana = new AlterableStats(playerClass.hp, playerClass.mana);

        UnityEngine.UI.Image[] hud = GameObject.FindGameObjectsWithTag("HUD")[playerNum].GetComponentsInChildren<UnityEngine.UI.Image>();

        healthBar = hud[0];
        manaBar = hud[1];
        playerIcon = hud[2];
        
        if (playerClass.mana == 0)
        {
            manaBar.enabled = false;
        }

        playerIcon.sprite = playerClass.icon;
    }

    private void Update()
    {
        Move(controls.Player.Movement.ReadValue<Vector2>());
    }

    #region Movement

    private void Move(Vector2 input)
    {
        rb2D.velocity = new Vector2(input.x * currentSpeed, rb2D.velocity.y);
    }

    private void Aim(Vector2 input)
    {
        weaponPivot.transform.right = input;
    }

    private void Run(bool input)
    {
        this.currentSpeed = input ? playerClass.runSpeed : playerClass.walkSpeed;
    }

    private void Jump()
    {
        if (!IsGrounded())
            return;

        rb2D.AddForce(new Vector2(0f, playerClass.jumpHeight), ForceMode2D.Impulse);
    }

    private void StartUseWeapon()
    {
        if (weaponClass.manaUse > hpMana.currentMana)
            return;

        weaponClass.startUseAction.Invoke(this);
        UseMana(weaponClass.manaUse);
    }

    private void EndUseWeapon()
    {
        if (weaponClass.endUseAction != null)
        {
            weaponClass.endUseAction.Invoke(this);
        }
    }

    private void UseAbility()
    {
        if (abilityClass.manaUse > hpMana.currentMana)
            return;

        UseMana(abilityClass.manaUse);
        abilityClass.useAction.Invoke(this);
    }

    private void UseItem()
    {
        if (currentItem == null)
            return;

        currentItem.useAction.Invoke();
    }

    #endregion

    #region Attacks

    public void Mine()
    {

    }

    public void UseDrill()
    {

    }

    public void Swing()
    {

    }

    public void ShootSpell()
    {
        GameObject newBolt = Instantiate(spellBolt);
        newBolt.transform.position = transform.position;

        newBolt.GetComponent<Rigidbody2D>().velocity = weaponPivot.transform.right;
    }

    public void PullBow()
    {

    }

    public void ReleaseBow()
    {
        GameObject newArrow = Instantiate(arrow);
        newArrow.transform.position = transform.position;

        newArrow.GetComponent<Rigidbody2D>().AddForce(weaponPivot.transform.right * 10f, ForceMode2D.Impulse);
    }

    #endregion

    #region Stats

    public float CalcDamage()
    {
        return playerClass.damage * weaponClass.damage;
    }

    public void DealDamage(PlayerController target, float damage)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        hpMana.currentHP -= (damage > hpMana.currentHP ? hpMana.currentHP : damage);

        healthBar.fillAmount = hpMana.currentHP / playerClass.hp;
    }

    public void UseMana(float manaUse)
    {
        hpMana.currentMana -= manaUse;

        manaBar.fillAmount = hpMana.currentMana/ playerClass.mana;
    }

    #endregion

    private bool IsGrounded()
    {
        return Physics2D.BoxCast((Vector2) transform.position + Vector2.down, Vector2.one, 0, Vector2.down, 0f, ground);
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
